using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class User
    {
        public Int64 User_Id { get; set; }
        public string User_Name { get; set; }
        public decimal User_Money { get; set; }
        public string User_Password { get; set; }
        public DateTime User_ModifyDate { get; set; }
    }
}
