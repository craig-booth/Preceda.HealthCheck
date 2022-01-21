using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Concurrent;

using Preceda.HealthCheck.DataLayer;

using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{
    public class HealthCheckImporterFactory : IDataImporterFactory
    {
        private readonly string _Type;

        public HealthCheckImporterFactory(string type)
        {
            _Type = type;
        }
        public IDataImporter CreateImporter(string sqlServerConnectionString, string iSeriesConnectionString)
        {
            if (_Type == "STP")
                return new StpHealthCheckImporter(sqlServerConnectionString, iSeriesConnectionString);
            else if (_Type == "YE")
                return new YearEndHealthCheckImporter(sqlServerConnectionString, iSeriesConnectionString);
            else
                throw new NotSupportedException();
        }
    }

    public abstract class HealthCheckImporter : IDataImporter
    {
        protected readonly string _SqlServerConnectionString;
        protected readonly string _ISeriesConnectionString;
    
        public event EventHandler<ImportProgressEventArgs> ImportProgressEvent;
        public event EventHandler ImportCompleteEvent;
        
        protected int _DatabaseCount;
        protected int _DatabasesRead;
        protected int _DatabasesWritten;

        protected bool _ExportComplete;

        protected ConcurrentQueue<ExportedDatabase> _Queue = new ConcurrentQueue<ExportedDatabase>();

        public HealthCheckImporter(string sqlServerConnectionString, string iSeriesConnectionString)
        {
            _SqlServerConnectionString = sqlServerConnectionString;
            _ISeriesConnectionString = iSeriesConnectionString;   
        }

        public abstract Task Import(Guid id);

        protected class ExportedDatabase
        {
            public ValidationDatabase Database;
            public List<ValidationDetail> Details = new List<ValidationDetail>();

            public ExportedDatabase(ValidationDatabase database)
            {
                Database = database;
            }
        } 
    
        protected async Task ImportValidation(ValidationRun validation)
        {
            _Queue.Clear();
         
            // Get Count of Databases
            using (var dataExtractor = new HealthCheckDataDataExtractor(_ISeriesConnectionString))
            {
                _DatabaseCount = dataExtractor.GetDatabaseCount(validation);
                _DatabasesRead = 0;
                _DatabasesWritten = 0;
                _ExportComplete = false;
            }

            
            // Add Run Header
            using (var unitOfWork = new HealthCheckUnitOfWork(_SqlServerConnectionString))
            {
                await unitOfWork.RunRepository.Delete(validation.Id);
                await unitOfWork.DatabaseRepository.Delete(validation.Id);
                await unitOfWork.DetailRepository.Delete(validation.Id);

                await unitOfWork.RunRepository.Add(validation);

                unitOfWork.Save(); 
                OnProgressChanged();
            }

            var exportTask = Task.Run(() => ExportDatabases(validation));
            var importTask = Task.Run(() => ImportDatabases(validation));

            await Task.WhenAll(new[] { exportTask, importTask });

            OnImportComplete();
        }
        private async Task ExportDatabases(ValidationRun validation)
        {
            // Add each database to the queue
            using (var dataExtractor = new HealthCheckDataDataExtractor(_ISeriesConnectionString))
            {
                var databases = dataExtractor.GetDatabases(validation);
                foreach (var database in databases)
                {
                    try
                    {
                        database.Id = validation.Id;
                        var exportedDatabase = new ExportedDatabase(database);

                        var details = dataExtractor.GetDetails(database);
                        foreach (var detail in details)
                        {
                            detail.Id = validation.Id;
                            exportedDatabase.Details.Add(detail);
                        }

                        _Queue.Enqueue(exportedDatabase);
                        _DatabasesRead++;
                        OnProgressChanged();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            _ExportComplete = true;
        } 
    
        private async Task ImportDatabases(ValidationRun validation)
        {
            while (true)
            {
                if (_Queue.TryDequeue(out var exportedDatabase))
                {
                    using (var unitOfWork = new HealthCheckUnitOfWork(_SqlServerConnectionString))
                    {
                        try
                        {
                            await unitOfWork.DatabaseRepository.Add(exportedDatabase.Database);

                            await unitOfWork.DetailRepository.Add(exportedDatabase.Details);

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
