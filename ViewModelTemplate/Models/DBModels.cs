using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;


namespace ViewModelTemplate.Models
{

    [Table("Customer")]
    public class Customer
    {
        [Key]
        [Display(Name ="Customer Number")]
        public string CustNo { get; set; }
        [Display(Name = "First Name")]
        public string CustFirstName { get; set; }
        [Display(Name = "Last Name")]
        public string CustLastName { get; set; }
        public string CustStreet { get; set; }
        public string CustCity { get; set; }
        public string CustState { get; set; }
        public string CustZip { get; set; }
        public decimal? CustBal { get; set; }
    }

    [Table("OrderTbl")]
    public class OrderTbl
    {
        [Key]
        [Display(Name ="Order Number")]
        public string OrdNo { get; set; }
        [Display(Name = "Order Date")]
        public DateTime OrdDate { get; set; }
        public string CustNo { get; set; }
        public string EmpNo { get; set; }
        [Display(Name = "Order Name")]
        public string OrdName { get; set; }
        public string OrdStreet { get; set; }
        public string OrdCity { get; set; }
        public string OrdState { get; set; }
        public string OrdZip { get; set; }
    }

    [Table("Employee")]
    public class Employee
    {
        [Key]
        public string EmpNo { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpPhone { get; set; }
        public string SupEmpNo { get; set; }
        public decimal? EmpCommRate { get; set; }
        public string EmpEmail { get; set; }
    }

    [Table("OrdLine")]
    public class OrdLine
    {
        [Key]
		[Column(Order = 1)]
        public string OrdNo { get; set; }
		[Key]
		[Column(Order = 2)]
        public string ProdNo { get; set; }
        public int? Qty { get; set; }
    }

    [Table("Product")]
    public class Product
    {
        [Key]
        public string ProdNo { get; set; }
        public string ProdName { get; set; }
        public string ProdMfg { get; set; }
        public int? ProdQOH { get; set; }
        public decimal? ProdPrice { get; set; }
        public DateTime? ProdNextShipDate { get; set; }
    }

    public class OrderEntryDbContext : DbContext
    {
        public DbSet<Customer> customers { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<OrderTbl> orders { get; set; }
        public DbSet<OrdLine> orderLines { get; set; }
        public DbSet<Product> products { get; set; }

        public DbSet<CustomerOrders> CustomerOrders { get; set; }

		//this only allows db access.
		public DbSet<OrderLine> lines { get; set; }
	}
}