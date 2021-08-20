using DAL.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.EF
{
    public class AppDBContext : IdentityDbContext
    {
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderLine> OrderLines {  get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(i => i.Id);
            modelBuilder.Entity<OrderLine>(m =>
            {
                m.HasKey(i => i.Id);
            });
            modelBuilder.Entity<ProductPhoto>(m =>
            {
                m.HasKey(i => i.Id);
                m.HasOne(i => i.Product)
                .WithMany(j => j.ProductPhotos)
                .HasForeignKey(i => i.ProductId);
            });
        }
    }
}
