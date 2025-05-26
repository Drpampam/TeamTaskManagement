using Domain.Entities;
using Domain.Entities.Collaterals;
using Domain.Entities.EmploymentAnalysis;
using Domain.Entities.FamilyAndFriends;
using Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Confgurations
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //public DbSet<Demo> Demo { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Collateral> Collaterals { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<EmploymentInfo> EmploymentInfo { get; set; }
        public DbSet<PersonInfo> FamilynFriends { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            return result;
        }
    }
}
