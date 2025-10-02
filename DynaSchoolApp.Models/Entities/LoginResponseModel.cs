﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.Models.Entities
{
    public class LoginResponseModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public long TokenExpired { get; set; }
        public string RefreshToken { get; set; }
    }
}
