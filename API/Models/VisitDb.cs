using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class VisitDb : DbContext
    {
        public VisitDb(DbContextOptions<VisitDb> options) : base(options) { }

        public DbSet<Visit> Visits => Set<Visit>();
    }
}
