using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Utlilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RouletteServiceImpl : IRouletteService
    {
        BetDBContext Context;
        IUserService userService;
        decimal WinnnerByColorMultiplier = 0.0M;
        decimal WinnnerByNumberMultiplier = 0.0M;
        public RouletteServiceImpl(IConfiguration configuration, IUserService userService)
        {
            this.Context = new BetDBContext(configuration.GetConnectionString("TestingDB"));
            this.userService = userService;
            this.WinnnerByColorMultiplier = configuration.GetValue<decimal>("AppSettings:WinnnerByColorMultiplier");
            this.WinnnerByNumberMultiplier = configuration.GetValue<decimal>("AppSettings:WinnnerByNumberMultiplier");
        }
        protected Response ValidateForNewBet(Bet bet, Roulette roulette, User user)
        {
            Response response = new Response(true, "OK", null);            
            if (roulette == null)
                response = new Response(false, "ERROR: Roulette ID does not exist!", null);
            else if (roulette.Roulette_State != (byte)RouletteStateEnum.ROULETTE_OPEN)
                response = new Response(false, "ERROR: Roulette is not open!", null);
            else if (bet.User_Id <= 0)
                response = new Response(false, "ERROR: Provided user does not exist!", null);
            else if (bet.Bet_Amount <= 0)
                response = new Response(false, "ERROR: Betted money must be greater than 0!", null);
            else if (bet.Bet_Amount >= 10000)
                response = new Response(false, "ERROR: Betted money must not be greater than 10000!", null);
            else if (bet.Bet_Type != (byte)BetTypeEnum.BET_TYPE_COLOR && bet.Bet_Type != (byte)BetTypeEnum.BET_TYPE_NUMBER)
                response = new Response(false, "ERROR: The Bet type must be 0 for bet to a color or 1 to bet to a number!", null);
            else if (bet.Bet_Type != (byte)BetTypeEnum.BET_TYPE_COLOR && (bet.Bet_Number < 0 || bet.Bet_Number > 36))
                response = new Response(false, "ERROR: The number to bet must be from 0 to 36!", null);
            else if (user != null && bet.Bet_Amount >= user.User_Money )
                response = new Response(false, "ERROR: The User has not enough funds to bet!", null);

            return response;
        }
        protected Response ValidateForCloseRoulette(Roulette roulette)
        {
            Response response = new Response(true, "OK", null);
            if (roulette == null)
                response = new Response(false, "ERROR: Roulette ID does not exist", null);
            else if (roulette.Roulette_State == (byte)RouletteStateEnum.ROULETTE_NEW)
                response = new Response(false, "ERROR: Roulette was not already open", null);
            else if (roulette.Roulette_State == (byte)RouletteStateEnum.ROULETTE_CLOSE)
                response = new Response(false, "ERROR: Roulette is already closed", null);

            return response;
        }
        public async Task<Response> BetToRoulette(Bet bet)
        {
            Response res = new Response();
            Roulette roulette = null;
            Roulette storedRoulette = null;
            DateTime roulette_modifyTime;
            DateTime user_modifyTime;
            User user = null;
            User storedUser = null;
            if (bet == null)
                return new Response(false, "ERROR: Bet info is corrupted!", null);
            roulette = await Context.GetRoulette(bet.Roulette_Id);
            user = await userService.Get(bet.User_Id);
            res = ValidateForNewBet(bet, roulette, user);
            if (!res.Successfull)
                return res;
            roulette_modifyTime = roulette.Roulette_ModifyDate;
            user_modifyTime = user.User_ModifyDate;
            storedRoulette = await Context.GetRoulette(bet.Roulette_Id);
            storedUser = await userService.Get(bet.User_Id);
            if (storedRoulette.Roulette_ModifyDate != roulette_modifyTime)
                return new Response(false, "ERROR: Roulette was modified!", null);
            if (storedUser.User_ModifyDate != user_modifyTime)
                return new Response(false, "ERROR: User was modified, retry again!", null);
            user.User_Money -= bet.Bet_Amount;
            res.Successfull = await Context.EditUserData(user);
            if (!res.Successfull)
                return new Response(false, "ERROR: Could not perform payment!", null);
            res.Successfull = await Context.InsertBet(bet);
            res.Message = "SUCCESS! YOU MADE A BET, GOOD LUCK!";
            res.ResponseObject = $"ROULETTE = {roulette.Roulette_Id}";

            return res;
        }
        public async Task<String>  CreateRoulette()
        {
            Roulette roulette = await Context.InsertRoulette();
            if (roulette == null)
                return "ERROR:No roulette was created";

            return roulette.Roulette_Id;
        }
        public async Task<Response> ListRoulettes()
        {
            return new Response(true, "List of Roulettes, Roulette_State == 0 is for new, Roulette_State == 1 for open and Roulette_State == 2 for closed", await Context.GetRouletteList());
        }
        public async Task<string> OpenRoulette(string id)
        {
            Roulette roulette = await Context.GetRoulette(id);
            Roulette StoredRoulette = null;
            bool result = false;
            DateTime modifyTime;
            if (roulette == null)
                return "ERROR: Roulette ID does not exist";
            if(roulette.Roulette_State == (byte)RouletteStateEnum.ROULETTE_OPEN)            
                return "ERROR: Roulette is already open";            
            if (roulette.Roulette_State == (byte)RouletteStateEnum.ROULETTE_CLOSE)            
                return "ERROR: Roulette is currently closed";            
            modifyTime = roulette.Roulette_ModifyDate;
            roulette.Roulette_State = (byte)RouletteStateEnum.ROULETTE_OPEN;
            roulette.Roulette_ModifyDate = DateTime.Now;
            StoredRoulette = await Context.GetRoulette(id);
            if (StoredRoulette.Roulette_ModifyDate != modifyTime)
                return "ERROR: Roulette was modified!";
            result = await Context.EditRoulette(roulette);

            return result ? "Roulette was open successfully!" : "ERROR: Roulette could not be open";
        }
        public async Task<Response> PerformRouletteClosingOnDB(Roulette roulette)
        {            
            Roulette StoredRoulette = null;
            Response response = new Response();
            DateTime modifyTime;            
            modifyTime = roulette.Roulette_ModifyDate;
            roulette.Roulette_State = (byte)RouletteStateEnum.ROULETTE_CLOSE;
            roulette.Roulette_ModifyDate = DateTime.Now;
            StoredRoulette = await Context.GetRoulette(roulette.Roulette_Id);
            if (StoredRoulette.Roulette_ModifyDate != modifyTime)
                return new Response(false, "ERROR: Roulette was modified", null);
            response.Successfull = await Context.EditRoulette(roulette);
            return response;
        }
        protected decimal ComputeWinnerGain(byte WinnerNumber, Bet bet)
        {
            if (bet.Bet_Type == (byte)BetTypeEnum.BET_TYPE_COLOR && NumbersUtil.IsPair(bet.Bet_Number) == NumbersUtil.IsPair(WinnerNumber))
            {
                return bet.Bet_Amount * WinnnerByColorMultiplier;
            }
            else if(bet.Bet_Type == (byte)BetTypeEnum.BET_TYPE_NUMBER && bet.Bet_Number == WinnerNumber)
            {
                return bet.Bet_Amount * WinnnerByNumberMultiplier;
            }
            return 0;
        }
        protected List<UserBetResult> ConvertToUserBetResults(byte WinnerNumber, List<Bet> betList)
        {
            UserBetResult resultBet;
            List<UserBetResult> userBetResults = new List<UserBetResult>();
            decimal moneyGained = 0;
            string winnerOrLoser = "";           
            foreach (var bet in betList)
            {
                moneyGained = ComputeWinnerGain(WinnerNumber, bet);
                winnerOrLoser = moneyGained <= 0 ? $"lost ${bet.Bet_Amount}" : $"gained ${moneyGained - bet.Bet_Amount} (${moneyGained})";
                resultBet = new UserBetResult()
                {
                    Bet_Description = $"User betted ${bet.Bet_Amount} for {BetUtils.ComposeBetInfo(bet)} and { winnerOrLoser }",
                    UserBet_GainedMoney = moneyGained,
                    User_Id = bet.User_Id,
                    Bet_Id = bet.Bet_Id
                };
                userBetResults.Add(resultBet);
            }
            return userBetResults;
        }
        public async Task<Response> CloseRoulette(string id)
        {
            List<Bet> betList = null;
            List<UserBetResult> userBetResults = null;
            Roulette roulette = null;
            byte WinnerNumber = 0;
            Response response = new Response();
            BetResult betResult = null;
            roulette = await Context.GetRoulette(id);
            response = ValidateForCloseRoulette(roulette);
            if (!response.Successfull) 
                return response;            
            betList = await Context.GetBetsFromRoulette(id);
            if(betList == null || betList.Count <= 0)
                return new Response(false, "ERROR: There is not any bet on this roulette!", null);
            WinnerNumber = NumbersUtil.RandomByte(36);
            userBetResults = ConvertToUserBetResults(WinnerNumber, betList);
            response = await PerformRouletteClosingOnDB(roulette);
            if(!response.Successfull)
            {
                return new Response(false, "ERROR: Could not close roulette!", null);
            }
            betResult = new BetResult()
            {
                BetsPerformed = betList,
                UsersBetResults = userBetResults,
                WinnerNumberAndColor = $"{BetUtils.GetBetColorFromByte(WinnerNumber)} {WinnerNumber}",
            };

            return new Response(true, $"Roulette was closed successfully", betResult);
        }
    }
}
