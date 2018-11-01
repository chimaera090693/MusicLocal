using System.Data.Entity.Migrations;


namespace music.local.Repository.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "SSD_eContract.API.Repositorys.Common.AppDbContext";
        }

        protected override void Seed(AppDbContext context)
        {
           
        }
    }
}
