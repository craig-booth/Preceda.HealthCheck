using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preceda.HealthCheck.DataLayer
{

    public class ImportProgressEventArgs : EventArgs
    {
        public int DatabasesRead { get; set; }
        public int DatabasesWritten { get; set; }

        public int DatabasesImported { get; set; }
        public int DatabaseCount { get; set; }
    }
}
