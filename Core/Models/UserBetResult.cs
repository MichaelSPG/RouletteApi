using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class UserBetResult
    {
        public Int64 Bet_Id { get; set; }
        public Int64 User_Id { get; set; }
        public decimal UserBet_GainedMoney { get; set; }
        public string Bet_Description { get; set; }
    }
}
