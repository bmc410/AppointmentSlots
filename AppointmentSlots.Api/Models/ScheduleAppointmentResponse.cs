using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class ScheduleAppointmentResponse
    {
        public int ApptID { get; set; }
        public bool IsSussessful { get; set; }
        public String Message { get; set; }
    }
}