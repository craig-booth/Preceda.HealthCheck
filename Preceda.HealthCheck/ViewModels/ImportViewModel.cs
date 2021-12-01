using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

using Booth.WpfControls;

using Preceda.HealthCheck.DataLayer;
using Preceda.HealthCheck.Import;
using Preceda.SystemUsage.Import;


namespace Preceda.HealthCheck.ViewModels
{
    class ImportViewModel : INotifyPropertyChanged
    {
        private const string _HealthCheckConnectionString = "Server=srv-tpg-qa-db1;Database=PrecedaHealthChecks;User Id=StatsCollection;Password=C8Fev4!5YI;";
        private const string _SystemUsageConnectionString = "Server=srv-tpg-qa-db1;Database=PrecedaSystemUsage;User Id=StatsCollection;Password=C8Fev4!5YI;";

        private string _Server;
        public string Server
        {
            get { return _Server; }
            set
            {
                if (value != _Server)
                {
                    _Server = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                if (value != _UserName)
                {
                    _UserName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                if (value != _Password)
                {
                    _Password = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _ImportStartTime;
        private bool _ImportInProgress;

        private int _DatabaseCount;
        public int DatabaseCount
        {
            get { return _DatabaseCount; }
            set
            {
                if (value != _DatabaseCount)
                {
                    _DatabaseCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _DatabasesRead;
        public int DatabasesRead
        {
            get { return _DatabasesRead; }
            set { 
                if (value != _DatabasesRead)
                {
                    _DatabasesRead = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _DatabasesWritten;
        public int DatabasesWritten
        {
            get { return _DatabasesWritten; }
            set
            {
                if (value != _DatabasesWritten)
                {
                    _DatabasesWritten = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _DatabasesPerSecond;
        public double DatabasesPerSecond
        {
            get { return _DatabasesPerSecond; }
            set
            {
                if (value != _DatabasesPerSecond)
                {
                    _DatabasesPerSecond = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _TimeRemaining;
        public TimeSpan TimeRemaining
        {
            get { return _TimeRemaining; }
            set
            {
                if (value != _TimeRemaining)
                {
                    _TimeRemaining = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool NoProgress
        {
            get { return _ImportInProgress && (_DatabaseCount == 0); }
        }

        public ImportViewModel()
        {
            _ImportInProgress = false;
            _DatabaseCount = 999;
            SaveConfigCommand = new RelayCommand(SaveConfig, CanSave);
            ImportCommand = new RelayCommand(Import, CanImport);
        }

        public RelayCommand SaveConfigCommand { get; private set; }
        public async void SaveConfig()
        {
            var connection = new MySqlConnection(_HealthCheckConnectionString);
            await connection.OpenAsync();

            var importConfig = new ImportConfiguration(connection)
            {
                ISeriesServer = Server,
                ISeriesUser = UserName,
                ISeriesPassword = Password
            };

            await importConfig.SaveConfiguration();

            await connection.CloseAsync();
        }

        public async void LoadConfig()
        {
            var connection = new MySqlConnection(_HealthCheckConnectionString);
            await connection.OpenAsync();

            var importConfig = new ImportConfiguration(connection);
            await importConfig.LoadConfiguration();

            Server = importConfig.ISeriesServer;
            UserName = importConfig.ISeriesUser;
            Password = importConfig.ISeriesPassword;

            await connection.CloseAsync();
        }

        public RelayCommand ImportCommand { get; private set; }
        public async void Import()
        {
            await ImportData(_HealthCheckConnectionString, new HealthCheckImporterFactory("STP"));

            await ImportData(_HealthCheckConnectionString, new HealthCheckImporterFactory("YE"));

            await ImportData(_SystemUsageConnectionString, new SystemUsageImporterFactory());
        }

        private async Task ImportData(string connectionString, IDataImporterFactory importerFactory)
        {
            _ImportInProgress = true;
            DatabaseCount = 0;
            DatabasesRead = 0;
            DatabasesWritten = 0;
            DatabasesPerSecond = 0.0;
            TimeRemaining = new TimeSpan(0, 0, 0);

            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var dataImporter = importerFactory.CreateImporter(connection, Server, UserName, Password);

            dataImporter.ImportCompleteEvent += Importer_ImportCompleteEvent;
            dataImporter.ImportProgressEvent += Importer_ImportProgressEvent;

            _ImportStartTime = DateTime.Now;

            var id = Guid.NewGuid();
            await dataImporter.Import(id);

            await connection.CloseAsync();
        }

        private void Importer_ImportProgressEvent(object sender, ImportProgressEventArgs e)
        {
            DatabaseCount = e.DatabaseCount;
            DatabasesRead = e.DatabasesRead;
            DatabasesWritten = e.DatabasesWritten;

            var elapsedTime = (TimeSpan)(DateTime.Now - _ImportStartTime);
            var secondsRemaining = 0;
            if (e.DatabasesImported > 0)
            {
                DatabasesPerSecond = e.DatabasesImported / elapsedTime.TotalSeconds;
                secondsRemaining = Convert.ToInt32((e.DatabaseCount - e.DatabasesImported) * (1 / DatabasesPerSecond));
            }
            TimeRemaining = new TimeSpan(0, 0 , secondsRemaining); 
        }

        private void Importer_ImportCompleteEvent(object sender, EventArgs e)
        {
            _ImportInProgress = false;
        }

        public bool CanImport()
        {
            return !_ImportInProgress;
        }
        public bool CanSave()
        {
            return !_ImportInProgress;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    
}
