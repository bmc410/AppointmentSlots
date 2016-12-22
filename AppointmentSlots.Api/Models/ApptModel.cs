using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSlots.Api.Models
{
    public class ApptModel:Customer
    {
        public int ApptId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String StartTimeString { get; set; }
        public String EndTimeString { get; set; }
        public int? Duration { get; set; }

    }
}
