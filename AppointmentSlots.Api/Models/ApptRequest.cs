using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class ApptRequest
    {
        public int CustId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int AppointmentType { get; set; }
        public String Comment { get; set; }


    }
}