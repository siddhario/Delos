using Delos.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.Model
{
    public class istorija_cijena
    {
        [JsonIgnore]
        [ForeignKey("artikal_sifra")]
        public artikal artikal { get; set; }
        public string artikal_sifra { get; set; }
        public DateTime vrijeme { get; set; }
        public decimal cijena { get; set; }
    }
}
