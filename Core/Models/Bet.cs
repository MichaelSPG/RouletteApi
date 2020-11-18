using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Bet
    {
        public Int64 Bet_Id { get; set; }
        public Int64 User_Id { get; set; }
        public string Roulette_Id { get; set; }
        public decimal Bet_Amount { get; set; }
        public byte Bet_Number { get; set; }
        public byte Bet_Type { get; set; }
        public DateTime Bet_ModifyDate { get; set; }
        public Bet GetCopy()
        {
            return new Bet()
            {
                Bet_Amount = this.Bet_Amount,
                Bet_Id = this.Bet_Id,
                Bet_Number = this.Bet_Number,
                Bet_ModifyDate = this.Bet_ModifyDate,
                Bet_Type = this.Bet_Type,
                Roulette_Id = this.Roulette_Id,
                User_Id = this.User_Id
            };
        }
    }
}
