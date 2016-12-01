using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class CustomerRequest
    {
        public int CustId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public Boolean? MyProIsRegisteredperty { get; set; }
        public String PIN { get; set; }

    }
}