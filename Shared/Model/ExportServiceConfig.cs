using System;
using System.Security;
using static Delos.Helpers.Helper;

namespace Shared.Model
{
    public class ExportServiceConfig
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public RecurringModeEnum RecurringMode { get; set; }
        public int RecurringInterval { get; set; }
        public TimeSpan StartsOn { get; set; }
        public bool StartImmediately { get; set; }
        public string ServiceId { get; set; }
        public string Implementation { get; set; }
        public string Path { get; set; }
        public string FtpAddress { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public int KeepFileDays { get; set; }
    }
}