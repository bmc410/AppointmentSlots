using AppointmentSlots.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppointmentSlots.Api.Controllers
{

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        [HttpPost]
        public Customer LogIn(LoginRequest request)
        {
            using (ApptEntities db = new ApptEntities())
            {
                Customer cust = (from c in db.Customers
                                 where c.UserName == request.Username
                                 && c.Password == request.Password
                                 && c.IsEnabled == true
                                 select c).FirstOrDefault();
                return cust;
            }
        }

        [HttpPost]
        public Customer SaveCustomer(CustomerRequest request)
        {
            using (ApptEntities db = new ApptEntities())
            {
                Customer cust = (from c in db.Customers
                                 where request.CustId == c.CustId
                                 select c).FirstOrDefault();

                if (cust == null)
                {
                    cust = new Customer();
                }

                if (!string.IsNullOrEmpty(request.PIN))
                    cust.PIN = request.PIN;

                if (!string.IsNullOrEmpty(request.Email))
                    cust.Email = request.Email;

                if (!string.IsNullOrEmpty(request.FirstName))
                    cust.FirstName = request.FirstName;

                if (!string.IsNullOrEmpty(request.LastName))
                    cust.LastName = request.LastName;

                if (!string.IsNullOrEmpty(request.Phone))
                    cust.Phone = request.Phone;

                if (!string.IsNullOrEmpty(request.UserName))
                    cust.UserName = request.UserName;

                if (!string.IsNullOrEmpty(request.Password))
                    cust.Password = request.Password;

                cust.IsEnabled = request.IsEnabled;

                var myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);

                cust.LastModified = currentDateTime;

                if (cust.CustId == 0)
                {
                    db.Customers.Add(cust);
                    db.Entry(cust).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    db.Entry(cust).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();

                return (from c in db.Customers
                        where c.CustId == cust.CustId
                        select c).FirstOrDefault();

            }
        }


        [HttpPost]
        public Customer GetCustomerInfo(CustomerRequest request)
        {
            using (ApptEntities db = new ApptEntities())
            {
                Customer cust = (from c in db.Customers
                                 where c.CustId == request.CustId
                                 select c).FirstOrDefault();
                return cust;
            }
        }

    }
}
