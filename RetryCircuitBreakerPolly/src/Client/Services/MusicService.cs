using Client.Models;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Services
{
    public class MusicService : Service, IMusicService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncPolicyWrap _pollyService;
        public MusicService(HttpClient httpClient, IRetryCircuitBreakerService cbService)
        {
            //httpClient.BaseAddress = new Uri("https://httpstat.us/500");
            httpClient.BaseAddress = new Uri("https://localhost:5002/music/");
            _httpClient = httpClient;
            _pollyService = cbService.CreatePolicyManager();

        }
        public async Task<List<Music>> GetGoodSongs()
        {
            try
            {
                var response = await _pollyService.ExecuteAsync(
                    async () => await _httpClient.GetAsync("good-songs")
                );

                TratarErrosResponse(response);
                return await Deserialize<List<Music>>(response);
            }
            catch (Exception ex)
            {

                return null;
            }            
        }
    }
}
