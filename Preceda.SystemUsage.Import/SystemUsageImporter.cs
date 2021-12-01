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
        public IDataImporter CreateImporter(IDbConnection dBConnection, string server, string userName, string password)
        {
            var dataExtractor = new SystemUsageDataExtractor(server, userName, password);
            var importer = new SystemUsageImporter(dBConnection, dataExtractor);

            return importer;
        }
    }

    public class SystemUsageImporter : IDataImporter
    {
        private readonly IDbConnection _DbConnection;
        private readonly ISystemUsageDataExtractor _DataExtractor;
    
        public event EventHandler<ImportProgressEventArgs> ImportProgressEvent;
        public event EventHandler ImportCompleteEvent;
        
        private int _DatabaseCount;
        private int _DatabasesRead;
        private int _DatabasesWritten;

        private bool _ExportComplete;

        private ConcurrentQueue<UsageDatabase> _Queue = new ConcurrentQueue<UsageDatabase>();

        public SystemUsageImporter(IDbConnection dBConnection, ISystemUsageDataExtractor extractor)
        {
            _DbConnection = dBConnection;
            _DataExtractor = extractor;
        } 

        public async Task Import(Guid id)
        {
            _Queue.Clear();

            // Get Count of Databases
            _DatabaseCount = await _DataExtractor.GetDatabaseCount();
            _DatabasesRead = 0;
            _DatabasesWritten = 0;
            _ExportComplete = false;

            // Add Run Header
            using (var unitOfWork = new SystemUsageUnitOfWork(_DbConnection))
            {
                await unitOfWork.UsageDatabaseRepository.DeleteAll();
                await unitOfWork.UsageDetailRepository.DeleteAll();
                await unitOfWork.UsageItemRepository.DeleteAll();
  
                var items = _DataExtractor.GetItems();
                await foreach (var item in items)
                {
                    await unitOfWork.UsageItemRepository.Add(item);
                }

                unitOfWork.Save(); 
                OnProgressChanged();
            }

            var exportTask = Task.Run(() => ExportDatabases());
            var importTask = Task.Run(() => ImportDatabases());

            await Task.WhenAll(new[] { exportTask, importTask });

            OnImportComplete();
        } 

        private async Task ExportDatabases()
        {
            // Add each database to the queue
            var databases = _DataExtractor.GetDatabases();
            await foreach (var database in databases)
            {
                _Queue.Enqueue(database);
                _DatabasesRead++;
                OnProgressChanged(); 
            }

            _ExportComplete = true;
        } 

        private async Task ImportDatabases()
        {
            while (true)
            {
                if (_Queue.TryDequeue(out var exportedDatabase))
                {
                    using (var unitOfWork = new SystemUsageUnitOfWork(_DbConnection))
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
