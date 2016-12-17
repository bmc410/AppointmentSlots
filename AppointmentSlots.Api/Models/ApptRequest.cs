using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class ApptRequest: CustomerRequest
    {
        public DateTime ApptDateTime { get; set; }
        public int Duration { get; set; }
        public int AppointmentType { get; set; }
        public String Comment { get; set; }


    }
}