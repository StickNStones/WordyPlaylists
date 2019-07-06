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

        public IList<Class> Class { get;set; }

        [BindProperty]
        [Required(ErrorMessage = "Have to search something!")]
        public string Sentences { get; set; } = "";

        [ViewData]
        public string postSentence { get; set; } = "";

        public void Orange()
        {
            postSentence = "orangze " + Sentences;
        }

        public async Task<IActionResult> OnGet()
        {
            if (Sentences != "")
            {
             //   Sentences = await HttpContext.GetTokenAsync("Spotify", "access_token");
            }
            Orange();
            return Page();
        }

        private static SpotifyWebAPI _spotify;

        public async void aMain()
        {

            SpotifyController orange = new SpotifyController();
            ContentResult result = orange.GetSpot();
            Sentences = HttpContext.Request.GetEncodedUrl().ToString().Split("=")[1];
            ContentResult someThing = orange.Get(Sentences);
            Sentences = someThing.Content.ToString();
            if (Sentences != null)
            {
                dynamic stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject(Sentences);
                string thisText = stuff1.access_token;
                Sentences = thisText;
            }

            _spotify = new SpotifyWebAPI()
            {
                AccessToken = Sentences,
                TokenType = "Bearer"
            };
            FullTrack track = _spotify.GetTrack("77oU2rjC5XbjQfNe3bD6so?si=1WJPdJrMRcCrScbjZqpFvg");
            Sentences = track.Name;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            aMain();

            if (!ModelState.IsValid)
            {
                return Page();
            }


            return await OnGet();
// return Redirect("/Sentences");
        }
        
    }
}
