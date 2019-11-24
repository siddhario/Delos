using Delos.Model;
using Microsoft.EntityFrameworkCore;

namespace Delos.Contexts
{
    public class DelosDbContext : DbContext
    {
        public DbSet<partner> partner { get; set; }
        public DbSet<korisnik> korisnik { get; set; }
        public DbSet<ponuda> ponuda { get; set; }
        public DbSet<ponuda_stavka> ponuda_stavka { get; set; }
        public DbSet<ponuda_dokument> ponuda_dokument { get; set; }
        public DelosDbContext(DbContextOptions<DelosDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ponuda_stavka>()
           .HasKey(o => new { o.ponuda_broj, o.stavka_broj });
            modelBuilder.Entity<ponuda_dokument>()
       .HasKey(o => new { o.ponuda_broj, o.dokument_broj });
        }
    }
}
