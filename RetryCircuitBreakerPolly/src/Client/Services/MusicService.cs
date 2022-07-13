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
            httpClient.BaseAddress = new Uri("https://httpstat.us/500");
            _httpClient = httpClient;
            _pollyService = cbService.CreatePolicyManager();

        }
        public async Task<List<Music>> GetGoodSongs()
        {
            var response = await _pollyService.ExecuteAsync(
                    async() => await _httpClient.GetAsync("")
                );

            TratarErrosResponse(response);
            return await Deserialize<List<Music>>(response);
        }
    }
}
