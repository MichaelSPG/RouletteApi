using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserServiceImpl : IUserService
    {
        BetDBContext Context;
        public UserServiceImpl(IConfiguration configuration)
        {
            this.Context = new BetDBContext(configuration.GetConnectionString("TestingDB"));
        }
        public async Task<User> Authenticate(string username, string password)
        {
            User user = new User()
            {
                User_Name = username,
                User_Password = password
            };
            user = await Task.Run(() => Context.AuthUser(user));
            if (user == null)
                return null;
            user.User_Password = null;

            return user;
        }
        public async Task<User> Get(long id)
        {
            return await Task.Run(() => Context.GetUser(id));
        }
    }
}
