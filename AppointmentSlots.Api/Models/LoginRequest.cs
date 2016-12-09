using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class LoginRequest
    {
        public String Username { get; set; }
        public String Password { get; set; }
    }
}