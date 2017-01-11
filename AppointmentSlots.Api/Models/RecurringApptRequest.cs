using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class RecurringApptRequest
    {
   
        public int dayOfWeek { get; set; } //1=Mon, 2=Tue, 4=Wed, 8=Thur, 16=Fri, 32=Sat
        public int dayFrequency { get; set; } //1 = All, 2=every second, ...
        public int weekFrequency { get; set; } //1=Every week, 2=Every second, etc
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime currentDate { get; set; }

    }
}