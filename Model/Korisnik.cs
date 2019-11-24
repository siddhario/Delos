using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delos.Model
{
    public class korisnik
    {
        [Key]
        public string korisnicko_ime { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public string email { get; set; }
        public string lozinka { get; set; }
        public bool admin { get; set; }
        [NotMapped]
        public string token { get; set; }
    }
}
