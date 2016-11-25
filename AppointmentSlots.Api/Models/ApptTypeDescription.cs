using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentSlots.Api.Models
{
    public class ApptTypeDescription
    {
        public int ApptLength { get; set; }
        public String ApptDescription { get; set; }
        public int ApptTypeID { get; set; }
        public double ApptPrice { get; set; }
        public String ApptType { get; set; }

    }
}