using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Preceda.HealthCheck.STP.Entities;
using Preceda.HealthCheck.STP.Export;


namespace Preceda.HealthCheck.STP
{
    class SingleTouchPayrollImport
    {
        private readonly IDbConnection _DbConnection;
        private readonly IExporter _Exporter;

        public SingleTouchPayrollImport(IDbConnection dBConnection, IExporter exporter)
        {
            _DbConnection = dBConnection;
            _Exporter = exporter;
        }

        public void ImportSTPHealthCheck(Guid id)
        {
            var validation = new ValidationRun()
            {
                Id = id,
                Description = "Single Touch Payroll",
                RunDate = DateTime.Today,
                ValidationProgram = "STPHC001C"
            };

            Import(validation);
        }

        public void ImportYearEndHealthCheck(Guid id)
        {
            var validation = new ValidationRun()
            {
                Id = id,
                Description = "Year End",
                RunDate = DateTime.Today,
                ValidationProgram = "YEHC010C"
            };

            Import(validation);
        }

        private void Import(ValidationRun validation)
        {
            // Add Run Header
            using (var unitOfWork = new UnitOfWork(_DbConnection))
            {
                unitOfWork.RunRepository.Delete(validation.Id);
                unitOfWork.DatabaseRepository.Delete(validation.Id);
                unitOfWork.DetailRepository.Delete(validation.Id);

                unitOfWork.RunRepository.Add(validation);

                unitOfWork.Save();
            }

            // Add each database
            var databases = _Exporter.GetDatabases(validation);
            foreach (var database in databases)
            {
                using (var unitOfWork = new UnitOfWork(_DbConnection))
                {
                    database.Id = validation.Id;
                    unitOfWork.DatabaseRepository.Add(database);
             
                    var details = _Exporter.GetDetails(database);
                    foreach (var detail in details)
                    {
                        detail.Id = validation.Id;
                        unitOfWork.DetailRepository.Add(detail);
                    }

                    unitOfWork.Save();
                }
            }
        }
    }
}
