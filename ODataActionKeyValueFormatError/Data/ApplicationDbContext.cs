using Microsoft.EntityFrameworkCore;
using ODataActionKeyValueFormatError.Data.Entities;

namespace ODataActionKeyValueFormatError.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LocalizableString> LocalizableStrings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new LocalizableStringMap());
        }
    }
}