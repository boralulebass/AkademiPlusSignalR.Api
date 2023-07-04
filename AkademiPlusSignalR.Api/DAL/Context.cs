using Microsoft.EntityFrameworkCore;

namespace AkademiPlusSignalR.Api.DAL
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-D0HPTG1\\SQLEXPRESS;initial Catalog=DbAkademiPlusSignalR;integrated security=true;");
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
