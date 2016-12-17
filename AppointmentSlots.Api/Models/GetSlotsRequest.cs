using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class GetSlotsRequest
    {
        public int duration { get; set; }
        public int apptLength { get; set; }
        public int daysToGet { get; set; }
    }
}