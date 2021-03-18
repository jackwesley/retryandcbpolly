using Client.Models;
using Polly;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IMusicService
    {
        Task<List<Music>> GetGoodSongs();
        
    }
}
