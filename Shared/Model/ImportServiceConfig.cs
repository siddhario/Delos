using System;
using System.Collections.Generic;
using System.Text;
using static Delos.Helpers.Helper;

namespace Shared.Model
{
    public class ImportServiceConfig
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public RecurringModeEnum RecurringMode { get; set; }
        public int RecurringInterval { get; set; }
        public TimeSpan StartsOn { get; set; }
        public bool StartImmediately { get; set; }
        public string Implementation { get; set; }
        public string Path { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePass { get; set; }
        public string[] Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool CalculatePrice { get; set; }
        public int? Priority { get; set; }
    }
}
