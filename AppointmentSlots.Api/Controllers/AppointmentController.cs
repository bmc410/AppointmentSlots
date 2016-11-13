using AppointmentSlots.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentSlots.Api.Controllers
{
    public class AppointmentController : ApiController
    {

        List<ApptModel> appts;
        List<SlotModel> slots;
        List<SlotModel> availableslots;
        List<ApptException> apptExceptions;
        List<ApptAvailability> apptAvailability;
        List<AppointmentType> appointmentTypes;

        public List<ApptModel> GetAppointments()
        {
            using (ApptEntities db = new ApptEntities())
            {
                List<ApptModel> appts = new List<ApptModel>();
                appts = (from a in db.Appointments
                         select new ApptModel
                         {
                             StartTime = (DateTime)a.StartDateTime,
                             EndTime = (DateTime)a.EndDateTime
                         }).ToList();


                return appts;
            }
        }

        private List<SlotModel> FillAvailableSlots(int duration)
        {
            availableslots = new List<SlotModel>();
            foreach (var s in slots)
            {
                ApptException recEx = null;
                var taken = appts.Where(x => x.StartTime.TimeOfDay >= s.StartSlot.TimeOfDay && x.EndTime.TimeOfDay <= s.EndSlot.TimeOfDay).FirstOrDefault();

                recEx = (from ae in apptExceptions
                         where (s.StartSlot.TimeOfDay >= ae.ExceptionStartDateTime.TimeOfDay
                         && s.EndSlot.TimeOfDay <= ae.ExceptionEndDateTime.TimeOfDay)
                         || (s.StartSlot.TimeOfDay <= ae.ExceptionStartDateTime.TimeOfDay
                         && s.EndSlot.TimeOfDay > ae.ExceptionStartDateTime.TimeOfDay)
                         && (ae.IsRecurring || ae.ExceptionEndDateTime.Date == s.StartSlot.Date)
                         select ae).FirstOrDefault();

                if (taken == null && recEx == null)
                {
                    availableslots.Add(s);
                }

            }
            return availableslots;

        }
    }
}
