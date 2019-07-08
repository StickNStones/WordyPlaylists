using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WordyPlaylist1._0.Models;
using WordyPlaylist1._0.Sentence;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using dotnet_core_spotify_authentication.Controllers;

namespace WordyPlaylist1._0.Pages.Sentences
{
    public class IndexModel : PageModel
    {
        private readonly WordyPlaylist1._0.Models.WordyPlaylist1_0Context _context;

        public IndexModel(WordyPlaylist1._0.Models.WordyPlaylist1_0Context context)
        {
            _context = context;
        }

        public IList<Class> Class { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Have to search something!")]
        public string Sentences { get; set; } = "";

        public string tempVar { get; set; } = "";

        [ViewData]
        public string postSentence { get; set; } = "";

        private string[] songNames { get; set; }

        private static SpotifyWebAPI _spotify;

        private void Orange()
        {
            postSentence = "orangze " + Sentences;
        }

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public void BuildPlaylist()
        {

            SpotifyController orange = new SpotifyController();
            ContentResult result = orange.GetSpot();
            tempVar = HttpContext.Request.GetEncodedUrl().ToString().Split("=")[1];
            ContentResult someThing = orange.Get(tempVar);
            tempVar = someThing.Content.ToString();
            if (tempVar != null)
            {
                dynamic stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject(tempVar);
                string thisText = stuff1.access_token;
                tempVar = thisText;
            }

            _spotify = new SpotifyWebAPI()
            {
                AccessToken = tempVar,
                TokenType = "Bearer"
            };
            songNames = Sentences.Split(" ");
            string asong = songNames[1];

            FullTrack track = _spotify.GetTrack("77oU2rjC5XbjQfNe3bD6so?si=1WJPdJrMRcCrScbjZqpFvg");
            PrivateProfile userProfile = _spotify.GetPrivateProfile();
            FullPlaylist playlistName = _spotify.CreatePlaylist(userProfile.Id, Sentences);

            foreach (string song in songNames)
            {
                SearchItem x = _spotify.SearchItems(song, SearchType.Track, 1);
                Paging<FullTrack> yes = x.Tracks;
                FullTrack trackName = yes.Items[0];

                _spotify.AddPlaylistTrack(playlistName.Id, trackName.Uri); //(_spotify.SearchItems(song, SearchType.Track, 1, 0).Tracks.Items[0].Id));
            }
            tempVar = track.Name;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            BuildPlaylist();
            Orange();



            ModelState.Clear();






            if (!ModelState.IsValid)
            {
                return Page();
            }


            return await OnGet();
            // return Redirect("/Sentences");
        }

    }
}
