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
    }
}
