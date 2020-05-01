using System.ComponentModel.DataAnnotations;

namespace Delos.Model
{
    public class partner
    {
        [Key]
        public int? sifra { get; set; }
        public string naziv { get; set; }
        public string tip { get; set; }
        public string maticni_broj { get; set; }
        public string adresa { get; set; }
        public string telefon { get; set; }
        public string email { get; set; }
        public bool kupac { get; set; }
        public bool dobavljac { get; set; }
        public string broj_lk { get; set; }
    }
}
