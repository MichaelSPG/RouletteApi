using Core;
using Core.Entities;
using Core.Models;
using Infrastructure.Utilities;
using Infrastructure.Utlilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class BetDBContext
    {
        SqlConnection Connection;
        public BetDBContext(string ConnectionString)
        {
            Connection = new SqlConnection(ConnectionString);
        }   
        public async Task<Roulette> InsertRoulette()
        {
            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            Roulette roulette = new Roulette();
            bool succeeded = false;
            try
            {
                cmd = new SqlCommand("insert into Roulette(Roulette_Id, Roulette_State) VALUES(@id, @state)", Connection);
                roulette = new Roulette { Roulette_Id = RandomsUtil.RandomString(20), Roulette_State = (byte)RouletteStateEnum.ROULETTE_NEW };
                while(await GetRoulette(roulette.Roulette_Id) != null)
                {
                    roulette.Roulette_Id = RandomsUtil.RandomString(20);
                }
                cmd.Parameters.AddWithValue("@id", roulette.Roulette_Id);
                cmd.Parameters.AddWithValue("@state", roulette.Roulette_State);
                Connection.Open();
                transaction = Connection.BeginTransaction();
                cmd.Transaction = transaction;
                succeeded = await cmd.ExecuteNonQueryAsync() > 0;        
                transaction.Commit();          
                if(!succeeded)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                roulette.Roulette_Id = "ERROR";
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return roulette;
        }
        public async Task<bool> EditRoulette(Roulette EditedRoulette)
        {
            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            bool succeeded = false;
            try
            {
                cmd = new SqlCommand("update Roulette SET Roulette_State=@state, Roulette_ModifyDate = @modifyDate where Roulette_Id = @id", Connection);
                cmd.Parameters.AddWithValue("@id", EditedRoulette.Roulette_Id);
                cmd.Parameters.AddWithValue("@modifyDate", EditedRoulette.Roulette_ModifyDate);
                cmd.Parameters.AddWithValue("@state", EditedRoulette.Roulette_State);
                Connection.Open();
                transaction = Connection.BeginTransaction();
                cmd.Transaction = transaction;
                succeeded = await cmd.ExecuteNonQueryAsync() > 0;
                transaction.Commit();
            }
            catch (Exception)
            {
                succeeded = false;
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return succeeded;
        }
        public async Task<Roulette> GetRoulette(string id)
        {
            SqlCommand cmd = null;
            Roulette roulette = null;
            SqlDataReader reader = null;
            try
            {
                cmd = new SqlCommand("select * from Roulette where Roulette_Id = @id", Connection);
                cmd.Parameters.AddWithValue("@id", id);
                Connection.Open();
                reader = await cmd.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                {
                    roulette = SqlConvertUtils.ReadRoulette(reader);
                }
                reader.Close();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return roulette;
        }       
        public async Task<List<Roulette>> GetRouletteList()
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            List<Roulette>  rouletteList = new List<Roulette>();
            try
            {
                cmd = new SqlCommand("select * from Roulette", Connection);
                Connection.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    rouletteList.Add(SqlConvertUtils.ReadRoulette(reader));
                }
                reader.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return rouletteList;
        }

        public async Task<bool> InsertBet(Bet bet)
        {
            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            bool succeeded = false;
            try
            {
                cmd = new SqlCommand("insert into Bet(Bet_Number, Bet_Amount, Bet_Type, User_Id, Roulette_Id) VALUES(@Bet_Number, @Bet_Amount, @Bet_Type, @User_Id, @Roulette_Id)", Connection);
                cmd.Parameters.AddWithValue("@Bet_Number", bet.Bet_Number);
                cmd.Parameters.AddWithValue("@Bet_Amount", bet.Bet_Amount);
                cmd.Parameters.AddWithValue("@Bet_Type", bet.Bet_Type);
                cmd.Parameters.AddWithValue("@User_Id", bet.User_Id);
                cmd.Parameters.AddWithValue("@Roulette_Id", bet.Roulette_Id);
                Connection.Open();
                transaction = Connection.BeginTransaction();
                cmd.Transaction = transaction;
                succeeded = await cmd.ExecuteNonQueryAsync() > 0;
                transaction.Commit();
            }
            catch (Exception)
            {
                succeeded = false;
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return succeeded;
        }
        public async Task<List<Bet>> GetBetsFromRoulette(string roulette_id)
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            List<Bet> betList = new List<Bet>();
            Bet bet = null;
            try
            {
                cmd = new SqlCommand("select * from Bet where Roulette_Id = @roulette_id", Connection);
                cmd.Parameters.AddWithValue("@roulette_id", roulette_id);
                Connection.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    bet = SqlConvertUtils.ReadBet(reader);
                    betList.Add(bet);
                }
                reader.Close();
            }
            catch (Exception)
            {                
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return betList;
        }
        public async Task<bool> EditUserData(User user)
        {
            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            bool succeeded = false;
            try
            {
                cmd = new SqlCommand("update BetUser SET User_Money=@money, User_ModifyDate = @modifyDate where User_Id = @id", Connection);
                cmd.Parameters.AddWithValue("@id", user.User_Id);
                cmd.Parameters.AddWithValue("@money", user.User_Money);
                cmd.Parameters.AddWithValue("@modifyDate", DateTime.Now);
                Connection.Open();
                transaction = Connection.BeginTransaction();
                cmd.Transaction = transaction;
                succeeded = await cmd.ExecuteNonQueryAsync() > 0;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                succeeded = false;
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return succeeded;
        }
        public async Task<User> GetUser(Int64 id)
        {
            SqlCommand cmd = null;
            User user = null;
            SqlDataReader reader = null;
            try
            {
                cmd = new SqlCommand("select * from BetUser where User_Id = @id", Connection);
                cmd.Parameters.AddWithValue("@id", id);
                Connection.Open();
                reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    user = SqlConvertUtils.ReadUser(reader);
                }
                reader.Close();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return user;
        }
        public async Task<User> AuthUser(User user)
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = new SqlCommand("select * from BetUser where User_Name = @user AND User_Password= @pass", Connection);
                cmd.Parameters.AddWithValue("@user", user.User_Name);
                cmd.Parameters.AddWithValue("@pass", user.User_Password);
                Connection.Open();
                reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    user = SqlConvertUtils.ReadUser(reader);
                }
                reader.Close();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }

            return user;
        }
    }
}
