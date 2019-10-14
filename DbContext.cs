using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class BloggingContext : DbContext
    {
     
        public DbSet<korisnik> korisnik { get; set; }
        public DbSet<ponuda> ponuda { get; set; }
        public DbSet<ponuda_stavka> ponuda_stavka { get; set; }
        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ponuda_stavka>()
           .HasKey(o => new { o.ponuda_broj, o.stavka_broj });
        }
    }


    public class korisnik
    {
        [Key]
        public string korisnicko_ime { get; set; }
        public string ime { get; set; }

        public string prezime { get; set; }

        public string email { get; set; }

        public string lozinka { get; set; }

        public bool admin { get; set; }
    }


    public class ponuda
    {
        [Key]
        public string broj { get; set; }
        public string status { get; set; }
        public string napomena { get; set; }
        public string predmet { get; set; }
        public string radnik { get; set; }
        public DateTime datum { get; set; }
        public int partner_sifra { get; set; }
        public string partner_jib { get; set; }
        public string partner_adresa { get; set; }
        public string partner_naziv { get; set; }
        public string partner_telefon { get; set; }
        public string partner_email { get; set; }
        public string valuta_placanja { get; set; }
        public string rok_vazenja { get; set; }
        public string paritet_kod { get; set; }
        public string paritet { get; set; }
        public string rok_isporuke { get; set; }
        public decimal iznos_bez_rabata { get; set; }
        public decimal rabat { get; set; }
        public decimal iznos_sa_rabatom { get; set; }
        public decimal pdv { get; set; }
        public decimal iznos_sa_pdv { get; set; }
        public decimal? nabavna_vrijednost { get; set; }
        public decimal? ruc { get; set; }
        
        public IEnumerable<ponuda_stavka> stavke { get; set; }
    }

    public class ponuda_stavka
    {
        public string ponuda_broj { get; set; }
        public int stavka_broj { get; set; }
        public string artikal_naziv { get; set; }
        public string opis { get; set; }
        public string jedinica_mjere { get; set; }
        public decimal kolicina { get; set; }
        public decimal cijena_bez_pdv { get; set; }
        public decimal cijena_bez_pdv_sa_rabatom { get; set; }
        public decimal rabat_procenat { get; set; }
        public decimal rabat_iznos { get; set; }
        public decimal iznos_bez_pdv { get; set; }
        public decimal iznos_bez_pdv_sa_rabatom { get; set; }
        public decimal cijena_nabavna { get; set; }
        public decimal vrijednost_nabavna { get; set; }
        public decimal marza_procenat { get; set; }
        public decimal ruc { get; set; }
        public decimal pdv_stopa { get; set; }
        public decimal pdv { get; set; }        
        public decimal cijena_sa_pdv { get; set; }
        public decimal iznos_sa_pdv { get; set; }

        [JsonIgnore]
        [ForeignKey("ponuda_broj")]
        public ponuda ponuda {get;set;}
    }
}
