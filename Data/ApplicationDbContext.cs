using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VMSApplication.Controllers;
using VMSApplication.Models;
using Microsoft.Identity.Client;
using VMSApplication.UserViewModel;

namespace VMSApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var Relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                Relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.Entity<RRequest>()
                .HasOne(f => f.Status).WithMany().HasForeignKey(f => f.StatusId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Department> departments { get; set; }
        public DbSet<VisitorType> visitertypes {  get; set; }
        public DbSet<Designation> designations { get; set; }
        public DbSet<Company> companys { get; set; }
        public DbSet<VisitorPurpose>visitorPurposes { get; set; }
        public DbSet<SystemCode>systemCodes { get; set; }
        public DbSet<ApplicationUser>applicationUsers { get; set; }
        public DbSet<HelpandSupport> helpandSupports { get; set; }
        public DbSet<VisitorRegistration> visitorsregistration { get; set; }
        public DbSet<SecurityForm>securityForms { get; set; }
     
        public DbSet<VisitorBlacklist>visitorBlacklists { get; set; }
        public DbSet<RRequest>requests { get; set; }

        public DbSet<Notification>notifications { get; set; }
        public DbSet<VMSApplication.Models.SafetyVideo> SafetyVideo { get; set; } = default!;
        public DbSet<VisitorSafetyAnswer>visitorSafetyAnswers { get; set; }
        public DbSet<Mailconfig>mailconfigs { get; set; }
        
    }
}
