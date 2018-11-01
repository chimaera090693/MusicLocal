using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Configuration = music.local.Repository.Migrations.Configuration;

namespace music.local.Repository
{
    // [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class AppDbContext : DbContext
    {
        private readonly static string NameDbContext = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["DbContext"]) ? ConfigurationManager.AppSettings["DbContext"] : "SSD_Context";
        public AppDbContext()
            : base("Name=" + NameDbContext)
        {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Configuration>(NameDbContext));
        }


        //public DbSet<TrackingPayment> TrackingPayments { get; set; }
        //public DbSet<Assignment> Assignments { get; set; }
        //public DbSet<PaymentMilestone> PaymentMilestones { get; set; }
        //public DbSet<OldDebtConfig> OldDebtConfigs { get; set; }
        //public DbSet<BillingSchedule> BillingSchedules { get; set; }
        //public DbSet<PaymentSchedule> PaymentSchedules { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(22, 2));
        }

        public override int SaveChanges()
        {
            //var modifiedEntries = ChangeTracker.Entries()
            //    .Where(x => x.Entity is IEntity
            //        && (x.State == System.Data.Entity.EntityState.Added || x.State == System.Data.Entity.EntityState.Modified));

            //foreach (var entry in modifiedEntries)
            //{
            //    IEntity entity = entry.Entity as IEntity;
            //    if (entity != null)
            //    {
            //        string identityName = Thread.CurrentPrincipal.Identity.Name;
            //        DateTime now = DateTime.UtcNow;

            //        if (entry.State == System.Data.Entity.EntityState.Added)
            //        {
            //            entity.CreatedBy = identityName;
            //            entity.CreatedDate = now;
            //        }
            //        else
            //        {
            //            base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
            //            base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
            //        }

            //        entity.UpdatedBy = identityName;
            //        entity.UpdatedDate = now;
            //    }
            //}

            return base.SaveChanges();
        }
    }
}
