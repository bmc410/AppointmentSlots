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
        List<ApptTypeDescription> appointmentTypes;


        public List<ApptModel> GetAppointments(DateTime apptDate)
        {
            AppointmentController ac = new AppointmentController();
            var a = ac.GetAppointments();
            return a.Where(x => x.StartTime.Date == apptDate.Date).ToList();

        }

        public List<ApptModel> GetAppointmentsByCustId(int custId)
        {
            AppointmentController ac = new AppointmentController();
            var a = ac.GetAppointments();
            return a.Where(x => x.CustId == custId).ToList();

        }

        private void Setup()
        {
            apptAvailability = GetApptAvailability();
            apptExceptions = GetApptExceptions();
            appointmentTypes = GetApptTypes();
        }

      


      
        public ScheduleAppointmentResponse ScheduleAppointment(ApptRequest request)
        {
            ScheduleAppointmentResponse resp = new ScheduleAppointmentResponse
            {
                IsSussessful = false
            };

            try
            {
                using (ApptEntities db = new ApptEntities())
                {
                    var isAvailable = (from a in db.Appointments
                                       where a.ApptDateTime == request.ApptDateTime
                                       select a).FirstOrDefault();

                    if (isAvailable != null)
                    {
                        throw new Exception("This appointment slot has already been reserved");
                    }


                    Customer cust = (from c in db.Customers
                                     where c.CustId == request.CustId
                                     select c).FirstOrDefault();

                    Appointment appt = new Appointment
                    {
                        AppointmentType = request.AppointmentType,
                        ApptDateTime = request.ApptDateTime,
                        Duration = request.Duration,
                        Comment = request.Comment,
                        CustId = cust.CustId
                    };
                    db.Appointments.Add(appt);
                    db.Entry(appt).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                    resp.IsSussessful = true;
                    resp.ApptID = appt.ApptId;

                }
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
            }

            return resp;
        }

        // GET api/Appointment/GetSlots/30
        [HttpPost]
        public List<SlotModel> GetSlots(GetSlotsRequest request)
        {
            int rowsAhead = 0;
            using (ApptEntities db = new ApptEntities())
            {
                var myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);
                List<SlotModel> slotTimes = new List<SlotModel>();
                DateTime startDate = RoundUp(currentDateTime, new TimeSpan(0, request.duration, 0));
                DateTime endDate = DateTime.Now.AddDays(request.daysToGet);
                TimeSpan tsStart = new TimeSpan(8, 0, 0);
                TimeSpan tsEnd = new TimeSpan(17, 0, 0);
                switch (request.apptLength)
                {
                    case 30:
                        rowsAhead = 1;
                        break;
                    case 45:
                        rowsAhead = 2;
                        break;
                    case 60:
                        rowsAhead = 3;
                        break;
                    default:
                        break;
                }


                var s = db.usp_GetSlotsForDates(startDate, endDate,
                            tsStart, tsEnd, request.duration).ToList();

                int i = 0;
                foreach (var t in s)
                {
 
                    if ((i + rowsAhead < s.Count) &&
                        DateTime.Compare(DateTime.Parse(t.StartDate.ToString()).AddMinutes(request.apptLength),
                        (DateTime)s.ElementAt(i + rowsAhead).EndDate) == 0)
                    {
                        String message = String.Format("{0} - {1}", (DateTime)t.StartDate,
                            DateTime.Parse(t.StartDate.ToString()).AddMinutes(request.apptLength));
                        Console.WriteLine(message);
                        slotTimes.Add(new SlotModel
                        {
                            StartSlot = (DateTime)t.StartDate,
                            EndSlot = DateTime.Parse(t.StartDate.ToString()).AddMinutes(request.apptLength)
                        });
                    }
                    i++;
                }
                return slotTimes;
            }
        }

        // GET api/Appointment/GetApptTypes
        public List<ApptTypeDescription> GetApptTypes()
        {
            using (ApptEntities db = new ApptEntities())
            {
                return (from a in db.AppointmentTypes
                        select new ApptTypeDescription
                        {
                            ApptDescription = a.ApptDescription,
                            ApptLength = (int)a.ApptLength,
                            ApptType = a.ApptType,
                            ApptTypeID = a.ApptTypeId,
                            ApptPrice = (double)a.ApptPrice
                        }).ToList();
            }
        }

        public  List<ApptAvailability> GetApptAvailability()
        {
            using (ApptEntities db = new ApptEntities())
            {
                return (from a in db.usp_GetAvailability()
                        select new ApptAvailability
                        {
                            AvailabilityStartDateTime = (DateTime)a.StartDateTime,
                            AvailabilityEndDateTime = (DateTime)a.EndDateTime,
                            AvailabilityId = a.AvailabilityId,
                            DayOfWeek = (int)a.DayOfWeek
                        }).ToList();
            }
        }

        public  List<ApptException> GetApptExceptions()
        {
            using (ApptEntities db = new ApptEntities())
            {
                return (from a in db.usp_GetExceptionTimes()
                        select new ApptException
                        {
                            ExceptionEndDateTime = (DateTime)a.ExceptionEndDate,
                            ExceptionStartDateTime = (DateTime)a.ExceptionStartDate,
                            ExceptionId = a.ExceptionId,
                            IsRecurring = (bool)a.IsRecurring
                        }).ToList();
            }
        }

        // GET api/Appointment/GetAppointments
        public  List<ApptModel> GetAppointments()
        {
            using (ApptEntities db = new ApptEntities())
            {
                List<ApptModel> appts = new List<ApptModel>();
                appts = (from a in db.Appointments
                         join c in db.Customers on a.CustId equals c.CustId
                         orderby a.ApptDateTime descending
                         select new ApptModel
                         {
                             StartTime = (DateTime)a.ApptDateTime,
                             EndTime = DateTime.Parse(a.ApptDateTime.ToString()).AddMinutes(Double.Parse(a.Duration.ToString())),
                             CustId = c.CustId,
                             Email = c.Email,
                             FirstName = c.FirstName,
                             LastName = c.LastName,
                             Phone = c.Phone,
                             PIN = c.PIN
                         }).ToList();

                foreach(var a in appts)
                {
                    a.StartTimeString = a.StartTime.ToShortTimeString();
                    a.EndTimeString = a.EndTime.ToShortTimeString();
                }

                return appts;
            }
        }

        static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
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
