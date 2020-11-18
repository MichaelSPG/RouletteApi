using Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infrastructure.Utlilities
{
    public class SqlConvertUtils
    {
        public static Roulette ReadRoulette(SqlDataReader reader)
        {
            return new Roulette
            {
                Roulette_Id = reader.GetString(0),
                Roulette_State = reader.GetByte(1),
                Roulette_CreationDate = reader.GetDateTime(2),
                Roulette_ModifyDate = reader.GetDateTime(3),
            };
        }
        public static User ReadUser(SqlDataReader reader)
        {
            return new User
            {
                User_Id = reader.GetInt64(0),
                User_Name = reader.GetString(1),
                User_Money = reader.GetDecimal(2),
                User_Password = reader.GetString(3),
                User_ModifyDate = reader.GetDateTime(4),
            };
        }
        public static Bet ReadBet(SqlDataReader reader)
        {
            return new Bet
            {
                Bet_Id = reader.GetInt64(0),
                User_Id = reader.GetInt64(1),
                Roulette_Id = reader.GetString(2),
                Bet_Amount = reader.GetDecimal(3),
                Bet_Number = reader.GetByte(4),
                Bet_Type = reader.GetByte(5),
                Bet_ModifyDate = reader.GetDateTime(6),
            };
        }
    }
}
