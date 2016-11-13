using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSlots.Api.Models
{
    public class SlotModel
    {
        public DateTime StartSlot { get; set; }
        public DateTime EndSlot { get; set; }
    }
}
