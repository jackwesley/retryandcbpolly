using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Server.Controllers
{
    [ApiController]
    public class MusicController : Controller
    {
        [HttpGet("music/good-songs")]
        public IActionResult Index()
        {
            var random = new Random();
            if (random.Next(1, 4) == 1)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(GetMusics());
        }


        private List<Music> GetMusics()
        {
            List<Music> songs = new List<Music>();
            var cryingShame = new Music
            {
                Name = "Crying Shame",
                Artist = "The Teskey Brothers",
                Genre = "Blues"
            };
            var rain = new Music
            {
                Name = "Rain",
                Artist = "The Teskey Brothers",
                Genre = "Blues"
            };
            var honeyMoon = new Music
            {
                Name = "Honey Moon",
                Artist = "The Teskey Brothers",
                Genre = "Blues"
            };
            var slowDancing = new Music
            {
                Name = "Slow Dancing in a Burning Room",
                Artist = "John Mayer",
                Genre = "Blues"
            };
            var belief = new Music
            {
                Name = "Belief",
                Artist = "John Mayer",
                Genre = "Blues"
            };

            songs.Add(cryingShame);
            songs.Add(rain);
            songs.Add(honeyMoon);
            songs.Add(slowDancing);
            songs.Add(belief);

            return songs;
        }
    }
}
