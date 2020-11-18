using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class BetResult
    {
        public string WinnerNumberAndColor { get; set; }
        public List<Bet> BetsPerformed { get; set; }
        public List<UserBetResult> UsersBetResults { get; set; }        
    }
}
