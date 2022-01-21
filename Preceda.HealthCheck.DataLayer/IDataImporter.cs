using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preceda.HealthCheck.DataLayer
{
    public interface IDataImporter
    {
        Task Import(Guid id);
        event EventHandler<ImportProgressEventArgs> ImportProgressEvent;
        event EventHandler ImportCompleteEvent;
    }

    public interface IDataImporterFactory
    {
        IDataImporter CreateImporter(string sqlServerConnectionString, string iSeriesConnectionString);
    }
}
