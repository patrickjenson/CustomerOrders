using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ViewModelTemplate.Models;

namespace ViewModelTemplate.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult customer()
        {
            DBRepository dbr = new DBRepository();
            List<Customer> customers = dbr.getCustomers();
            return View("customer",customers);
        }

        public ActionResult orders(string id)
        {
            DBRepository dbr = new DBRepository();
            CustomerOrders customerOrders = dbr.getCustomerOrdersEF(id);
            /*** or use SQL 
            CustomerOrders customerOrders = dbr.getCustomerOrdersSQL(id);
            /***/

            return View(customerOrders);
        }

		public ActionResult details(string cid, string oid) {
			DBRepository dbr = new DBRepository();
			CustomerOrderDetails od = dbr.getDetails(cid, oid);
			return View(od);
		}

    }
}
