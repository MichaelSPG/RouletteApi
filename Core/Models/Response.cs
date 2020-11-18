using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Response
    {
        public Response()
        {
            Successfull = false;
        }
        public Response(bool success, string msg, object obj)
        {
            Successfull = success;
            Message = msg;
            ResponseObject = obj;
        }
        public bool Successfull { get; set; }
        public string Message { get; set; }
        public object ResponseObject { get; set; }
    }
}
