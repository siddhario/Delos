namespace Delos.Model
{
    public class ponuda_dokument
    {
        public string ponuda_broj { get; set; }
        public short dokument_broj { get; set; }
        public byte[]? dokument { get; set; }
        public string naziv { get; set; }
        public string opis { get; set; }
    }
}
