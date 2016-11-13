﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppointmentSlots.Api
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ApptEntities : DbContext
    {
        public ApptEntities()
            : base("name=ApptEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AppointmentType> AppointmentTypes { get; set; }
        public virtual DbSet<Availability> Availabilities { get; set; }
        public virtual DbSet<ExceptionDate> ExceptionDates { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
    
        public virtual ObjectResult<usp_GetAppointmentsByDay_Result> usp_GetAppointmentsByDay(Nullable<System.DateTime> appointmentsDate)
        {
            var appointmentsDateParameter = appointmentsDate.HasValue ?
                new ObjectParameter("AppointmentsDate", appointmentsDate) :
                new ObjectParameter("AppointmentsDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetAppointmentsByDay_Result>("usp_GetAppointmentsByDay", appointmentsDateParameter);
        }
    
        public virtual ObjectResult<usp_GetAppointmentSlots_Result> usp_GetAppointmentSlots(Nullable<System.TimeSpan> start, Nullable<System.TimeSpan> end, Nullable<int> slot, Nullable<System.DateTime> slotDate)
        {
            var startParameter = start.HasValue ?
                new ObjectParameter("start", start) :
                new ObjectParameter("start", typeof(System.TimeSpan));
    
            var endParameter = end.HasValue ?
                new ObjectParameter("end", end) :
                new ObjectParameter("end", typeof(System.TimeSpan));
    
            var slotParameter = slot.HasValue ?
                new ObjectParameter("slot", slot) :
                new ObjectParameter("slot", typeof(int));
    
            var slotDateParameter = slotDate.HasValue ?
                new ObjectParameter("SlotDate", slotDate) :
                new ObjectParameter("SlotDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetAppointmentSlots_Result>("usp_GetAppointmentSlots", startParameter, endParameter, slotParameter, slotDateParameter);
        }
    
        public virtual ObjectResult<usp_GetApptTypes_Result> usp_GetApptTypes()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetApptTypes_Result>("usp_GetApptTypes");
        }
    
        public virtual ObjectResult<usp_GetAvailability_Result> usp_GetAvailability()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetAvailability_Result>("usp_GetAvailability");
        }
    
        public virtual ObjectResult<usp_GetExceptionTimes_Result> usp_GetExceptionTimes()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetExceptionTimes_Result>("usp_GetExceptionTimes");
        }
    }
}
