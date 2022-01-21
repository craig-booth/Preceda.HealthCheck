using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Preceda.HealthCheck.DataLayer;

using Preceda.SystemUsage.Import.Entities;
using System.Collections.Concurrent;

namespace Preceda.SystemUsage.Import
{

    public class SystemUsageImporterFactory : IDataImporterFactory
    {
        public IDataImporter CreateImporter(string sqlServerConnectionString, string iSeriesConnectionString)
        {          
            var importer = new SystemUsageImporter(sqlServerConnectionString, iSeriesConnectionString);

            return importer;
        }
    }

    public class SystemUsageImporter : IDataImporter
    {
        protected readonly string _SqlServerConnectionString;
        protected readonly string _ISeriesConnectionString;

        public event EventHandler<ImportProgressEventArgs> ImportProgressEvent;
        public event EventHandler ImportCompleteEvent;
        
        private int _DatabaseCount;
        private int _DatabasesRead;
        private int _DatabasesWritten;

        private bool _ExportComplete;

        private ConcurrentQueue<UsageDatabase> _Queue = new ConcurrentQueue<UsageDatabase>();

        public SystemUsageImporter(string sqlServerConnectionString, string iSeriesConnectionString)
        {
            _SqlServerConnectionString = sqlServerConnectionString;
            _ISeriesConnectionString = iSeriesConnectionString;
        } 

        public async Task Import(Guid id)
        {
            _Queue.Clear();

            // Get Count of Databases
            using (var dataExtractor = new SystemUsageDataExtractor(_ISeriesConnectionString))
            {
                _DatabaseCount = await dataExtractor.GetDatabaseCount();
                _DatabasesRead = 0;
                _DatabasesWritten = 0;
                _ExportComplete = false;


                // Add Run Header
                using (var unitOfWork = new SystemUsageUnitOfWork(_SqlServerConnectionString))
                {
                    await unitOfWork.UsageDatabaseRepository.DeleteAll();
                    await unitOfWork.UsageDetailRepository.DeleteAll();
                    await unitOfWork.UsageItemRepository.DeleteAll();

                    // Import Item definitions
                    var items = dataExtractor.GetItems();
                    await foreach (var item in items)
                    {
                        await unitOfWork.UsageItemRepository.Add(item);
                    }

                    unitOfWork.Save();
                    OnProgressChanged();
                }
            }

            var exportTask = Task.Run(() => ExportDatabases());
            var importTask = Task.Run(() => ImportDatabases());

            await Task.WhenAll(new[] { exportTask, importTask });

            OnImportComplete();
        } 

        private async Task ExportDatabases()
        {
            // Add each database to the queue
            using (var dataExtractor = new SystemUsageDataExtractor(_ISeriesConnectionString))
            {
                var databases = dataExtractor.GetDatabases();
                await foreach (var database in databases)
                {
                    _Queue.Enqueue(database);
                    _DatabasesRead++;
                    OnProgressChanged();
                }
            }

            _ExportComplete = true;
        } 

        private async Task ImportDatabases()
        {
            while (true)
            {
                if (_Queue.TryDequeue(out var exportedDatabase))
                {
                    using (var unitOfWork = new SystemUsageUnitOfWork(_SqlServerConnectionString))
                    {
                        try
                        {
                            await unitOfWork.UsageDatabaseRepository.Add(exportedDatabase);

                            await unitOfWork.UsageDetailRepository.Add(exportedDatabase.Details);

                            unitOfWork.Save();

                            _DatabasesWritten++;
                            OnProgressChanged(); 
                        }
                        catch { }
                    }
                }
                else
                {
                    if (_ExportComplete)
                        break;

                    Thread.Sleep(1000);
                }                
            }
        } 


        protected virtual void OnProgressChanged()
        {
            var @event = ImportProgressEvent;
            if (@event != null)
            {
                @event(this, new ImportProgressEventArgs()
                {
                    DatabasesRead = _DatabasesRead,
                    DatabasesWritten = _DatabasesWritten,
                    DatabasesImported = _DatabasesWritten,
                    DatabaseCount = _DatabaseCount
                });
            }
        }

        protected virtual void OnImportComplete()
        {
            var @event = ImportCompleteEvent;
            if (@event != null)
            {
                @event(this, EventArgs.Empty);
            }
        }
    }
}
