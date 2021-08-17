using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DAL.Model;

namespace DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(AppDBContext db)
        {
            if(!db.OrderLines.Any() || !db.OrderLines.Any())
            {
                db.Products.Add(new Product()
                {
                    Name = "Potato",
                    Description = "Ukrainian potato",
                    Cost = 12
                });
                db.Products.Add(new Product()
                {
                    Name = "Banana",
                    Description = "Brazilian banana",
                    Cost = 34
                });
                db.SaveChanges();
                OrderLine orderLine = new OrderLine()
                {
                    ProductId = 1,
                    Quantity = 5
                };
                OrderLine orderLine1 = new OrderLine()
                {
                    ProductId = 1,
                    Quantity = 14
                };
                OrderLine orderLine2 = new OrderLine()
                {
                    ProductId = 2,
                    Quantity = 50
                };
                db.OrderLines.AddRange(orderLine, orderLine1, orderLine2);
                db.SaveChanges();
            }
        }
    }
}
