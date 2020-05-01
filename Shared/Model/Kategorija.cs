using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Delos.Model
{
    public class kategorija
    {
        [Key]
        public string sifra { get; set; }
        public string naziv { get; set; }
        public List<string> kategorije_dobavljaca { get; set; }

        public decimal? marza { get; set; }

        public bool? aktivna { get; set; }
        public string kategorije_dobavljaca_string
        {
            get
            {
                var str = "";
                if (this.kategorije_dobavljaca != null)
                {
                    foreach (var s in this.kategorije_dobavljaca)
                        str += s + ";";
                }
                return str;
            }
        }
    }
}
