using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UBSIS3.Web.Models;

namespace UBSIS3.Web.Data.Context
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<ContactUs> ContactUsMessages { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<EmailNewsletter> EmailNewsletters { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Cov> Covs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContactUs>()
                 .Property(b => b.Message).HasMaxLength(4000);
        }
    }
}
