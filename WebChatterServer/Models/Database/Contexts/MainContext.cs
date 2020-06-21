using Microsoft.EntityFrameworkCore;
using WebChatterServer.Models.Database.BindingModels;

namespace WebChatterServer.Models.Database.Contexts
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> contextOptions)
            : base(contextOptions)
        {}

        public DbSet<UserStatus> UserStatuses { get; set; }

    }
}