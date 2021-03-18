using Client.Models;
using Client.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [ApiController]
    public class Musics : Controller
    {

        private readonly IMusicService _musicService;
     
        public Musics(IMusicService musicService, IRetryCircuitBreakerService retryCBService)
        {
            _musicService = musicService;
        }

        [HttpGet("music/good-songs")]
        public async Task<IActionResult> Index()
        {

            var goodSongs = await _musicService.GetGoodSongs();

            return Ok(goodSongs);
        }
    }
}
