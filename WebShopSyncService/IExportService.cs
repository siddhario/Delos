using Delos.Contexts;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delos.Model
{

    public abstract class IExportService
    {
        public string ConnectionString { get; set; }
        public ExportServiceConfig Config { get; set; }
        public abstract Task ExportAsync();

    }
}
