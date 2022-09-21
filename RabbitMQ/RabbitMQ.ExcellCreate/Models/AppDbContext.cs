using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RabbitMQ.ExcellCreate.Models
{
    public class AppDbContext:IdentityDbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<UserFile> UserFile { get; set; }
    }
}
