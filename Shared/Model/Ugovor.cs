using Delos.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class ugovor
    {
        [Key]
        public string broj { get; set; }
        public DateTime datum { get; set; }
        public int kupac_sifra { get; set; }
        public string kupac_maticni_broj { get; set; }
        public string kupac_broj_lk { get; set; }
        public string kupac_naziv { get; set; }
        public string kupac_adresa { get; set; }
        public string kupac_telefon { get; set; }
        public string broj_racuna { get; set; }
        public string radnik { get; set; }
        public decimal inicijalno_placeno { get; set; }
        public decimal iznos_bez_pdv { get; set; }
        public decimal pdv { get; set; }
        public decimal iznos_sa_pdv { get; set; }
        public decimal broj_rata { get; set; }
        public decimal suma_uplata { get; set; }
        public decimal preostalo_za_uplatu { get; set; }
        public string status { get; set; }
        public string napomena { get; set; }
        public bool mk { get; set; }
        public IEnumerable<ugovor_rata> rate { get; set; }

        public decimal? uplaceno_po_ratama { get; set; }
        [ForeignKey("kupac_sifra")]
        public partner partner { get; set; }
        [ForeignKey("radnik")]
        public korisnik Korisnik { get; set; }
    }
}
