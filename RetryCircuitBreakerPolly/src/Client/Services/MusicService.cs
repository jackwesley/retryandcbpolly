using Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Services
{
    public class MusicService : Service, IMusicService
    {
        private readonly HttpClient _httpClient;
        private readonly IRetryCircuitBreakerService _cbService;
        public MusicService(HttpClient httpClient, IRetryCircuitBreakerService cbService)
        {
            httpClient.BaseAddress = new Uri("https://httpstat.us/500");
            _httpClient = httpClient;
            _cbService = cbService;

        }
        public async Task<List<Music>> GetGoodSongs()
        {
            var policyManager = _cbService.CreatePolicyManager();
            var response = await policyManager.ExecuteAsync(async () => await _httpClient.GetAsync(""));

            TratarErrosResponse(response);
            return await Deserialize<List<Music>>(response);
        }
    }
}
