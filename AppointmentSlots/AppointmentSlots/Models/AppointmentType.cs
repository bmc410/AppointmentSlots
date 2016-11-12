using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSlots.Models
{
    public class AppointmentType
    {
        public int ApptTypeId { get; set; }
        public String ApptType { get; set; }
        public String ApptDesc { get; set; }
        public int ApptLength { get; set; }
    }
}
