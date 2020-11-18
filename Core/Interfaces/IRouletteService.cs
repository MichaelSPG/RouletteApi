using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRouletteService
    {
        Task<string> CreateRoulette();
        Task<string> OpenRoulette(string id);
        Task<Response> ListRoulettes();
        Task<Response> BetToRoulette(Bet bet);
        Task<Response> CloseRoulette(string id);
    }
}
