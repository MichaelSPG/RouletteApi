using Core.Entities;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Utlilities
{
    public class BetUtils
    {
        public static string GetBetColorFromByte(byte color)
        {
            return NumbersUtil.IsPair(color) ? "RED" : "BLACK"; 
        }
        public static string ComposeBetInfo(Bet bet)
        {
            switch ((BetTypeEnum)bet.Bet_Type)
            {
                case BetTypeEnum.BET_TYPE_COLOR:
                    return $"color {GetBetColorFromByte(bet.Bet_Number)}";
                case BetTypeEnum.BET_TYPE_NUMBER:
                    return $"number {bet.Bet_Number}";
                default:
                    return "?";
            }
        }
    }
}
