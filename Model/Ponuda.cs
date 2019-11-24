using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delos.Model
{
    public class ponuda
    {
        [Key]
        public string broj { get; set; }
        public string status { get; set; }
        public string napomena { get; set; }
        public string predmet { get; set; }
        public string radnik { get; set; }
        public DateTime datum { get; set; }
        public int? partner_sifra { get; set; }
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
        public decimal? iznos_bez_rabata { get; set; }
        public decimal? rabat { get; set; }
        public decimal? iznos_sa_rabatom { get; set; }
        public decimal? pdv { get; set; }
        public decimal? iznos_sa_pdv { get; set; }
        public decimal? nabavna_vrijednost { get; set; }
        public decimal? ruc { get; set; }

        [ForeignKey("partner_sifra")]
        public partner partner { get; set; }

        [ForeignKey("radnik")]
        public korisnik Korisnik { get; set; }

        public IEnumerable<ponuda_stavka> stavke { get; set; }
    }

}
