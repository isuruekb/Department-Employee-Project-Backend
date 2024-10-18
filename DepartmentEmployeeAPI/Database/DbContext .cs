using DepartmentEmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentEmployeeAPI.Database
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasColumnType("decimal(18,2") 
                .IsRequired();
        }
    }
}
