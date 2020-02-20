using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVEditor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static CVEditor.Controllers.HomeController;
using Microsoft.AspNetCore.Http;

namespace CVEditor.EntityFramework
{
    public class DataContext:DbContext, IService
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobOffer> JobOffers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HR> HRs { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasData(
                    new Company {
                        Id = 3,
                        Name = "Company C"},
                    new Company
                    {
                        Id = 4,
                        Name = "Company D"
                    }
                );
        }

        public UserType CheckUserType(string nameId)
        {
            if (Admins.ToList().Find(u => u.NameId == nameId) != null) return UserType.Admin;
            else if (HRs.ToList().Find(u => u.NameId == nameId) != null) return UserType.HR;
            else if (Users.ToList().Find(u => u.NameId == nameId) != null) return UserType.User;
            else
            {
                Users.Add(new User() { NameId = nameId });
                this.SaveChanges();
                return UserType.User;
            }
        }
    }
}
