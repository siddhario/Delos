using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Delos.Model
{
    public class ponuda_dokument
    {
        public string ponuda_broj { get; set; }
        public short dokument_broj { get; set; }
        public byte[]? dokument { get; set; }
        public string naziv { get; set; }
        public string opis { get; set; }

        [JsonIgnore]
        [ForeignKey("ponuda_broj")]
        public ponuda ponuda { get; set; }
    }
}
