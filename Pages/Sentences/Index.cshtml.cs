using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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

        private static SpotifyWebAPI _spotify = null;

        private void CompletionConfirmation()
        {
            postSentence = "Check your spotify for the playlist!";
        }

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public void spotifyConnect()
        {
            SpotifyController spotifyController = new SpotifyController();
            ContentResult result = spotifyController.GetSpot();
            tempVar = HttpContext.Request.GetEncodedUrl().ToString().Split("=")[1];
            ContentResult someThing = spotifyController.Get(tempVar);
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
        }

        public void BuildPlaylist()
        {
            if (_spotify == null)
            {
                spotifyConnect();
            }
            songNames = Sentences.Split(" ");

            FullTrack nullTrack = _spotify.GetTrack("5QeUZ1Z4xSCegBXutW3IIt");
            PrivateProfile userProfile = _spotify.GetPrivateProfile();
            FullPlaylist playlistName = _spotify.CreatePlaylist(userProfile.Id, Sentences);

            foreach (string song in songNames)
            {
                SearchItem searchResults = _spotify.SearchItems(song, SearchType.Track, 30);

                if (searchResults != null)
                {
                    Paging<FullTrack> trackList = searchResults.Tracks;

                    if (trackList != null)
                    {
                        Boolean found = false;

                        for (int i = 0; i < trackList.Items.Count; i++)
                        {
                            String testName = trackList.Items[i].Name;
                            Match isMatch = Regex.Match(testName.ToLower(), "^" + song.ToLower());

                            if (isMatch.Success)
                            {
                                FullTrack trackName = trackList.Items[i];
                                _spotify.AddPlaylistTrack(playlistName.Id, trackName.Uri);
                                found = true;
                                break;
                            }
                        }

                        if (!found && trackList.Items.Count > 0)
                        {
                            FullTrack trackName = trackList.Items[0];
                            _spotify.AddPlaylistTrack(playlistName.Id, trackName.Uri);
                        }
                        else
                        {
                            // debug track
                            //_spotify.AddPlaylistTrack(playlistName.Id, nullTrack.Uri);
                        }

                    }
                }
            }
            tempVar = nullTrack.Name;
            Sentences = "";

        }

        public async Task<IActionResult> OnPostAsync()
        {
            BuildPlaylist();
            CompletionConfirmation();



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
