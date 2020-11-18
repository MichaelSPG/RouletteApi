using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBetService
    {
        Task<Response> BetToNumber(Bet bet);
        Task<Response> CloseBets(Int64 Roulette_Id);
    }
}
