using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Delos.Model
{
    public class prijava
    {
        [Key]
        public string broj { get; set; }
        public string broj_naloga { get; set; }
        public DateTime? datum { get; set; }
        public int? kupac_sifra { get; set; }
        public string kupac_ime { get; set; }
        public string kupac_adresa { get; set; }
        public string kupac_telefon { get; set; }
        public string kupac_email { get; set; }
        public string model { get; set; }
        public string serijski_broj { get; set; }
        public string dodatna_oprema { get; set; }
        public string predmet { get; set; }
        public string napomena_servisera { get; set; }
        public string serviser { get; set; }
        public string serviser_primio { get; set; }
        public DateTime? datum_vracanja { get; set; }
        public DateTime? poslat_mejl_dobavljacu { get; set; }
        public DateTime? zavrseno { get; set; }
        public string dobavljac { get; set; }
        public int? dobavljac_sifra { get; set; }
        public int? garantni_rok { get; set; }
        public string broj_garantnog_lista { get; set; }
        public string broj_racuna { get; set; }
        public bool? instalacija_os { get; set; }
        public bool? instalacija_office { get; set; }
        public bool? instalacija_ostalo { get; set; }
        public string instalacija { get; set; }
        [ForeignKey("kupac_sifra")]
        public partner partner { get; set; }

        [ForeignKey("serviser_primio")]
        public korisnik Korisnik { get; set; }

        [ForeignKey("dobavljac_sifra")]
        public partner dobavljac_partner { get; set; }
    }
}
