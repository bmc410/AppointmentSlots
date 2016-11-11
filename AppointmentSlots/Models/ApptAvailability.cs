using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSlots.Models
{
    public class ApptAvailability
    {
        public int AvailabilityId { get; set; }
        public DateTime AvailabilityStartDateTime { get; set; }
        public DateTime AvailabilityEndDateTime { get; set; }
        public int DayOfWeek { get; set; }
    }
}
