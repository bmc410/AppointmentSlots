using AppointmentSlots.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSlots
{
    class Program
    {
        static List<ApptModel> appts;
        static List<SlotModel> slots;
        static List<SlotModel> availableslots;
        static List<ApptException> apptExceptions;
        static List<ApptAvailability> apptAvailability;
        static List<AppointmentType> appointmentTypes;

        static void Main(string[] args)
        {
            Setup();

            DateTime today = DateTime.Now;
            int duration = appointmentTypes.Where(x => x.ApptTypeId == 1).FirstOrDefault().ApptLength;
            slots = GetSlots(duration);
            appts = GetAppointments(today);
            availableslots = FillAvailableSlots(duration);

            if (Boolean.Parse(ConfigurationManager.AppSettings["ShouldPrint"]))
            {
                PrintSlots(availableslots);
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }

        }

        static void Setup()
        {
            apptAvailability = GetApptAvailability();
            apptExceptions = GetApptExceptions();
            appointmentTypes = GetApptTypes();
        }

        static void PrintSlots(List<SlotModel> availableslots)
        {
            foreach(SlotModel slot in availableslots)
            {
                Console.WriteLine(String.Format("{0} - {1}", slot.StartSlot.ToShortTimeString(), slot.EndSlot.ToShortTimeString()));
            }

        }


        static List<SlotModel> FillAvailableSlots(int duration)
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

        static List<ApptModel> GetAppointments(DateTime apptDate)
        {
            using (SlotsTableEntities db = new SlotsTableEntities())
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

        static List<SlotModel> GetSlots(int duration)
        {
            using (SlotsTableEntities db = new SlotsTableEntities())
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

        static List<AppointmentType> GetApptTypes()
        {
            using (SlotsTableEntities db = new SlotsTableEntities())
            {
                return (from a in db.usp_GetApptTypes()
                        select new AppointmentType
                        {
                            ApptDesc = a.ApptDescription,
                            ApptLength = (int)a.ApptLength,
                            ApptType = a.ApptType,
                            ApptTypeId = a.ApptTypeId
                        }).ToList();
            }
        }

        static List<ApptAvailability> GetApptAvailability()
        {
            using (SlotsTableEntities db = new SlotsTableEntities())
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
            using (SlotsTableEntities db = new SlotsTableEntities())
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

    }
}
