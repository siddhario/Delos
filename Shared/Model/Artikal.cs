using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Delos.Model
{
    public class artikal
    {
        [Key]
        public string sifra { get; set; }
        public string dobavljac_sifra { get; set; }
        public string dobavljac { get; set; }
        public string naziv { get; set; }
        public decimal kolicina { get; set; }
        public decimal cijena_sa_rabatom { get; set; }
        public decimal? cijena_prodajna { get; set; }
        public decimal? cijena_mp { get; set; }
        public string dostupnost { get; set; }
        public DateTime? zadnje_ucitavanje { get; set; }
        public List<string> slike { get; set; }
        public List<string> vrste { get; set; }
        public string kategorija { get; set; }
        public bool kalkulacija { get; set; }
        public string barkod { get; set; }
        public string brend { get; set; }
        public string garancija { get; set; }
        public string opis { get; set; }
        public bool aktivan { get; set; }
        public string vrsteString
        {
            get
            {
                var str = "";
                if (this.vrste != null)
                {
                    foreach (var s in this.vrste)
                        str += s + ";";
                }
                return str;
            }
        }
    }
}
