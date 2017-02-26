using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Web;
using ViewModelTemplate.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace ViewModelTemplate.Models
{
    public class DBRepository
    {
      /*************  Repository Methods ******************/

        public List<Customer> getCustomers()
        {
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<Customer> customers = new List<Customer>();
            try
            {
                customers = db.customers.ToList();
            } catch (Exception ex)
            { Console.WriteLine(ex.Message); }
            return customers;
        }

        /***** Use EF to get the customer orders *****/
        public CustomerOrders getCustomerOrdersEF(string custNo)
        {
            CustomerOrders customerOrders = new CustomerOrders();
            OrderEntryDbContext db = new OrderEntryDbContext();
            try
            {
                customerOrders.customer = db.customers.Find(custNo);
                var query = (from ot in db.orders where ot.CustNo == custNo select ot);
                customerOrders.orders = query.ToList();
            } catch (Exception ex) { Console.WriteLine(ex.Message); }

            return customerOrders;
        }

        /***** Use SQL to get the customer orders *****/
        public CustomerOrders getCustomerOrdersSQL(string custNo)
        {
            CustomerOrders customerOrders = new CustomerOrders();
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CustNo", custNo));

            try
            {
                string sql = "SELECT * FROM Customer WHERE CustNo = @CustNo";
                customerOrders.customer =
                    db.customers.SqlQuery(sql, sqlParams.ToArray()).First();

                sqlParams.Clear();
                sqlParams.Add(new SqlParameter("@CustNo", custNo));
                sql = "SELECT * FROM OrderTbl WHERE CustNo = @CustNo";
                customerOrders.orders =
                    db.orders.SqlQuery(sql, sqlParams.ToArray()).ToList();
            } catch (Exception ex) { Console.WriteLine(ex.Message); }
            return customerOrders;
        }

		public CustomerOrderDetails getDetails(string custNo, string ordNo){
			CustomerOrderDetails od= new CustomerOrderDetails();
			OrderEntryDbContext db= new OrderEntryDbContext();
			List<SqlParameter> sqlparams= new List<SqlParameter>();

			string findCust= "SELECT * FROM Customer WHERE CustNo = @CustNo";
			sqlparams.Add(new SqlParameter("@CustNo", custNo));
			try {
				od.customer= db.customers.SqlQuery(findCust, sqlparams.ToArray()).First();
			}
			catch (Exception e){
				Console.WriteLine(e.Message);
			}

			sqlparams.Clear();
			string findOrder= "SELECT * FROM OrderTbl WHERE OrdNo = @OrdNo";
			sqlparams.Add(new SqlParameter("@OrdNo", ordNo));
			try {
				OrderTbl ot= db.orders.SqlQuery(findOrder, sqlparams.ToArray()).First();
				od.OrdNo= ot.OrdNo;
				od.OrdDate= ot.OrdDate;
			}
			catch (Exception e){
				Console.WriteLine(e.Message);
			}

			//od.lines= findLines(ordNo);
			od.lines= findLinesMan(ordNo);
			return od;
		}

		private List<OrderLine> findLines(string ordNo){
			List<OrderLine> lin= new List<OrderLine>();
			OrderEntryDbContext db= new OrderEntryDbContext();
			List<SqlParameter> sqlparams= new List<SqlParameter>();

			string sql =
				"SELECT ol.OrdNo, ol.ProdNo, p.ProdName, p.ProdPrice, ol.Qty "+
				"FROM OrdLine AS ol, Product AS p "+
				"WHERE ol.ProdNo = p.ProdNo "+
				"AND ol.OrdNo = @OrdNo";
			sqlparams.Add(new SqlParameter("@OrdNo", ordNo));
			try {
				lin = db.lines.SqlQuery(sql, sqlparams.ToArray()).ToList();
			}
			catch (Exception e){
				Console.WriteLine(e.Message);
			}

			return lin;
		}

		private List<OrderLine> findLinesMan(string ordNo){
			List<OrderLine> oLines= new List<OrderLine>();
			string connStr, sql;
			connStr= //"Provider=SQLOLEDB;"+
				"Server=(LocalDB)\\MSSQLLocalDB;"+
				"AttachDbFilename=|DataDirectory|OrderEntry.mdf;"+
				"Integrated Security=True;Connect Timeout=30";
			sql= String.Format("SELECT ol.OrdNo, ol.ProdNo, p.ProdName, p.ProdPrice, ol.Qty "+
				"FROM OrdLine AS ol, Product AS p "+
				"WHERE ol.ProdNo = p.ProdNo AND ol.OrdNo = '{0}'", ordNo);
			//The above is BAD practice. How to use SqlParameter analogue?

			try {
				DataSet ds= new DataSet();
				SqlConnection conn= new SqlConnection(connStr);
				SqlDataAdapter da= new SqlDataAdapter(sql , conn);
				da.Fill(ds);
				oLines = mapOrderLines(ds);
				conn.Close();
			}
			catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			return oLines;
		}

		private List<OrderLine> mapOrderLines(DataSet ds) {
			List<OrderLine> list= new List<OrderLine>();
			
			try {
                foreach (DataRow row in ds.Tables[0].Rows){
					OrderLine line= new OrderLine();
					line.OrdNo= row["OrdNo"].ToString();
					line.ProdNo= row["ProdNo"].ToString();
					line.ProdName= row["ProdName"].ToString();
					if (row["ProdPrice"] != DBNull.Value)
						line.ProdPrice= Decimal.Parse(row["ProdPrice"].ToString());
					if (row["Qty"] != DBNull.Value)
						line.Qty= int.Parse(row["Qty"].ToString());
					list.Add(line);
                }
            }
			catch (Exception e){
				Console.WriteLine(e.Message);
            }

			return list;
		}
    }
    /***************** View Models **********************/

    public class CustomerOrders
    {
        public CustomerOrders()
        {
            this.customer = new Customer();
            this.orders = new List<OrderTbl>();
        }

        [Key]
        public string custNo { get; set; }
        public Customer customer { get; set; }
        public List<OrderTbl> orders { get; set; }
    }

	public class CustomerOrderDetails {

		public CustomerOrderDetails() {
			lines = new List<OrderLine>();
		}

		[Key]
		[Display(Name = "Order Number")]
		public string OrdNo { get; set; }

		[Display(Name= "Order Date")]
		[DisplayFormat(ApplyFormatInEditMode= true,
			DataFormatString= "{0:MM/dd/yyyy}")]
		public DateTime OrdDate { get; set; }

		public Customer customer { get; set; }

		public List<OrderLine> lines { get; set; }
	}

	public class OrderLine {

		[Key]
		public string OrdNo { get; set; }

		[Display(Name= "Product Number")]
		public string ProdNo { get; set; }

		[Display(Name= "Product Name")]
		public string ProdName { get; set; }

		[Display(Name= "Product Price")]
		public decimal? ProdPrice { get; set; }

		public int? Qty { get; set; }
	}

}
