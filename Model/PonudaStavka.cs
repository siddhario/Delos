using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Delos.Model
{
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
        public ponuda ponuda { get; set; }
    }
}
