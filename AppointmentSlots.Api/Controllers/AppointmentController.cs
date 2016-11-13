﻿using AppointmentSlots.Api.Models;
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


        static List<ApptModel> GetAppointments(DateTime apptDate)
        {
            using (ApptEntities db = new ApptEntities())
            {
                List<ApptModel> appts = new List<ApptModel>();
                appts = (from a in db.usp_GetAppointmentsByDay(apptDate)
                         select new ApptModel
                         {
                             StartTime = (DateTime)a.StartTime,
                             EndTime = (DateTime)a.EndTime
                         }).ToList();


                return appts;
            }
        }

        private void Setup()
        {
            apptAvailability = GetApptAvailability();
            apptExceptions = GetApptExceptions();
            appointmentTypes = GetApptTypes();
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

                slots = slots.Where(x => x.StartSlot.TimeOfDay >= DateTime.Now.TimeOfDay).ToList();
                return slots;
            }

        }

        // GET api/Appointment/GetApptTypes
        public List<AppointmentType> GetApptTypes()
        {
            using (ApptEntities db = new ApptEntities())
            {
                return (from a in db.usp_GetApptTypes()
                        select new AppointmentType
                        {
                            ApptDescription = a.ApptDescription,
                            ApptLength = (int)a.ApptLength,
                            ApptType = a.ApptType,
                            ApptTypeId = a.ApptTypeId
                        }).ToList();
            }
        }

        static List<ApptAvailability> GetApptAvailability()
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

        static List<ApptException> GetApptExceptions()
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