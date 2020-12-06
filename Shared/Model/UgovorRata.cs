using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class ugovor_rata
    {
        [Column("broj_ugovora")]
        public string ugovorbroj { get; set; }
        public int broj_rate { get; set; }
        public DateTime rok_placanja { get; set; }
        public DateTime? datum_placanja { get; set; }
        public decimal iznos { get; set; }
        public decimal? uplaceno { get; set; }
        public string napomena { get; set; }
    }
}
