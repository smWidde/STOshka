using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    public class Make
    {
        public int MakeId { get; set; }
        public string Name { get; set; }
    }
    public class Year
    {
        public int YearId { get; set; }
        public short Value { get; set; }
    }
    public class Model
    {
        public int ModelId { get; set; }
        public string Name { get; set; }
        public virtual Make Make { get; set; }
        public virtual Year Year { get; set; }
    }
    public class Engine
    {
        public int EngineId { get; set; }
        public string Value { get; set; }
    }
    public class Modification
    {
        public int ModificationId { get; set; }
        public string Value { get; set; }
    }
    public class BodyType
    {
        public int BodyTypeId { get; set; }
        public string Value { get; set; }
    }
    public class Car
    {
        public int CarId { get; set; }
        public virtual Model Model { get; set; }
        public virtual Engine Engine { get; set; }
        public virtual Modification Modification { get; set; }
        public virtual BodyType BodyType { get; set; }
        public virtual List<Buyer> Buyers { get; set; }
    }
    public class CarPart
    {
        public int CarPartId { get; set; }
        public string Name { get; set; }
    }
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual CarPart CarPart { get; set; }
    }
    public class Subcategory
    {
        public int SubcategoryId { get; set; }
        public string Name { get; set; }
        public virtual Category Category { get; set; }
    }
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public virtual Car Car { get; set; }
        public virtual Subcategory Subcategory { get; set; }
    }
    public class Good : Product
    {
        public virtual List<Service> Services { get; set; }
    }
    public class Service : Product
    {
        public virtual List<Good> Goods { get; set; }
        public virtual List<Master> Masters { get; set; }
    }
    public class Password
    {
        public int PasswordId { get; set; }
        public string HashPass { get; set; }
    }
    public class User
    {
        public int UserId { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
        public virtual Password Password { get; set; }
    }
    public class Buyer : User
    {
        public virtual List<Car> Cars { get; set; }
    }
    public class Admin : User
    {
    }
    public class Master : User
    {
        public List<OrderLine> Orders { get; set; }
        public List<Service> Services { get; set; }
    }
    public class Order
    {
        public int OrderId { get; set; }
        public virtual List<OrderLine> OrderLines { get; set; }
        public virtual Buyer Buyer { get; set; }
    }
    public class Status
    {
        public int StatusId { get; set; }
        public string Value { get; set; }
    }
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int Quantity { get; set; }
        public virtual Product Product { get; set; }
        public Order Order { get; set; }
        public virtual Master Master { get; set; }
        public virtual Status Status { get; set; }
    }
    public class EFDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<Good>()
                .HasMany(g => g.Services)
                .WithMany(s => s.Goods)
                .Map(cs =>
                {
                    cs.MapLeftKey("GoodId");
                    cs.MapRightKey("ServiceId");
                    cs.ToTable("G_Ss");
                });
            modelBuilder
                .Entity<Buyer>()
                .HasMany(b => b.Cars)
                .WithMany(c=>c.Buyers)
                .Map(cs =>
                {
                    cs.MapLeftKey("BuyerId");
                    cs.MapRightKey("CarId");
                    cs.ToTable("BuyerCar");
                });
            modelBuilder
                .Entity<Master>()
                .HasMany(m=>m.Services)
                .WithMany(s=>s.Masters)
                .Map(cs =>
                {
                    cs.MapLeftKey("MasterId");
                    cs.MapRightKey("ServiceId");
                    cs.ToTable("MasterServices");
                });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EFDbContext cont = new EFDbContext();
            //var objs = cont.Orders.Include(c => c.OrderLines.Select(f => f.Order));
            var objs = cont.Orders.Where(o => o.OrderId == 1).FirstOrDefault();
            foreach(var item in cont.Goods)
            {
                Console.WriteLine(item.Name);
            }
            foreach (var item in objs.OrderLines)
            {
                Console.WriteLine(item.Product.GetType().Name.Split('_')[0]);
                if(item.Product.GetType().Name.Split('_')[0] == "Service")
                {
                    Service service = (Service)item.Product;
                    foreach(var subitem in service.Goods)
                    {
                        Console.WriteLine(subitem);
                    }
                }
                Console.WriteLine(item.Master==null);
                foreach(var subitem in item.Master.Orders)
                {
                    Console.WriteLine(subitem.Order.Buyer.FIO);
                    Console.WriteLine(subitem.Order.Buyer.Cars[0].Model.Name);
                }
            }
            Car car = cont.Cars.Where(c => c.CarId == 1).FirstOrDefault();
            Console.WriteLine(car.Model.Name);
            foreach(var item in car.Buyers)
            {
                Console.WriteLine(item.FIO);
            }
        }
    }
}
