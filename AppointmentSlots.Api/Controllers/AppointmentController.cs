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

        private void Setup()
        {
            apptAvailability = GetApptAvailability();
            apptExceptions = GetApptExceptions();
            appointmentTypes = GetApptTypes();
        }

        public int CreateCustomer(CustomerRequest request)
        {
            using (ApptEntities db = new ApptEntities())
            {
                Customer cust = new Customer
                {
                     Email = request.Email,
                     FirstName = request.FirstName,
                     LastName = request.LastName,
                     Password = request.Password,
                     Phone = request.Phone,
                     UserName = request.UserName,
                     IsRegistered = request.UserName != null && request.Password != null? true:false
                         
                };
                db.Customers.Add(cust);
                db.Entry(cust).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();

                return cust.CustId;
            }
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
                                       where a.StartDateTime == request.StartDateTime
                                       select a).FirstOrDefault();

                    if (isAvailable != null)
                    {
                        throw new Exception("This appointment slot has already been reserved");
                    }


                    Customer cust = (from c in db.Customers
                                     where c.Email == request.Email
                                     && c.FirstName == request.FirstName
                                     && c.LastName == request.LastName
                                     select c).FirstOrDefault();

                    if (cust == null)
                    {
                        cust = new Customer()
                        {
                            Email = request.Email,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Phone = request.Phone
                        };
                        db.Customers.Add(cust);
                        db.SaveChanges();
                    }

                    Appointment appt = new Appointment
                    {
                        AppointmentType = request.AppointmentType,
                        StartDateTime = request.StartDateTime,
                        EndDateTime = request.EndDateTime,
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
        public List<SlotModel> GetSlots(int duration)
        {
            using (ApptEntities db = new ApptEntities())
            {
                List<SlotModel> slots = new List<SlotModel>();
                slots = (from a in db.usp_GetAppointmentSlots(new TimeSpan(6, 0, 0),
                             new TimeSpan(20, 0, 0), duration, DateTime.Now)
                         select new SlotModel
                         {
                             StartSlot = (DateTime)a.start,
                             EndSlot = (DateTime)a.end,
                         }).ToList();

                //slots = slots.Where(x => x.StartSlot.TimeOfDay >= DateTime.Now.TimeOfDay).ToList();
                return slots;
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
                         select new ApptModel
                         {
                             StartTime = (DateTime)a.StartDateTime,
                             EndTime = (DateTime)a.EndDateTime,
                             CustId = c.CustId,
                             Email = c.Email,
                             FirstName = c.FirstName,
                             LastName = c.LastName,
                             Phone = c.Phone,
                         }).ToList();

                foreach(var a in appts)
                {
                    a.StartTimeString = a.StartTime.ToShortTimeString();
                    a.EndTimeString = a.EndTime.ToShortTimeString();
                }

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
