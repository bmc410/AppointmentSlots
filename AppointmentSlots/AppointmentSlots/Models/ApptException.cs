using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSlots.Models
{
    public class ApptException
    {
        public int ExceptionId { get; set; }
        public DateTime ExceptionStartDateTime { get; set; }
        public DateTime ExceptionEndDateTime { get; set; }
        public Boolean IsRecurring { get; set; }
    }
}
