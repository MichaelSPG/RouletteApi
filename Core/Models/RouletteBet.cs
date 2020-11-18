using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class RouletteBet
    {
        public string Roulette_Id { get; set; }
        public decimal Bet_Amount { get; set; }
        public byte Bet_Number { get; set; }
        public byte Bet_Type { get; set; }
        public Bet ToBet(Int64 userId)
        {
            return new Bet
            {
                Roulette_Id = this.Roulette_Id,
                Bet_Amount = this.Bet_Amount,
                Bet_Type = this.Bet_Type,
                Bet_Number = this.Bet_Number,
                User_Id = userId
            };
        }

    }
}
