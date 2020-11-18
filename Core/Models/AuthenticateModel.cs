using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class AuthenticateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
