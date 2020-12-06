using Delos.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Model;

namespace Delos.Contexts
{
    public class DelosDbContext : DbContext
    {
        public string ConnectionString { get; set; }
        public DbSet<partner> partner { get; set; }
        public DbSet<korisnik> korisnik { get; set; }
        public DbSet<ponuda> ponuda { get; set; }
        public DbSet<ponuda_stavka> ponuda_stavka { get; set; }
        public DbSet<ponuda_dokument> ponuda_dokument { get; set; }
        public DbSet<prijava> prijava { get; set; }
        public DbSet<artikal> artikal { get; set; }
        public DbSet<istorija_cijena> istorija_cijena { get; set; }
        public DbSet<kategorija> kategorija { get; set; }
        public DbSet<ugovor> ugovor { get; set; }
        public DbSet<ugovor_rata> ugovor_rata { get; set; }
        public DelosDbContext(DbContextOptions<DelosDbContext> options) : base(options) { }
        public DelosDbContext(string connectionString) : base()
        {
            this.ConnectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ugovor_rata>()
        .HasKey(o => new { o.broj_rate, o.ugovorbroj });
            modelBuilder.Entity<ponuda_stavka>()
           .HasKey(o => new { o.ponuda_broj, o.stavka_broj });
            modelBuilder.Entity<ponuda_dokument>()
       .HasKey(o => new { o.ponuda_broj, o.dokument_broj });

            modelBuilder.Entity<istorija_cijena>()
    .HasKey(o => new { o.artikal_sifra, o.vrijeme });
        }
    }
}
