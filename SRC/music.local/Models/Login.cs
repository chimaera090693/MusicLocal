using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace music.local.Models
{
    public class Login
    {
        public string Identity { get;set;}
        public string Created { get;set;}
        public string Expired { get;set;}
        public string OtherInfor { get;set; }
        public string LastActive { get;set; }


    }
}