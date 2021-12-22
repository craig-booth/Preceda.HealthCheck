using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Preceda.HealthCheck.ViewModels;

namespace Preceda.HealthCheck
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var arguments = new Dictionary<string, string>();
            for (var i = 0; i < e.Args.Length; i += 2)
            {
                if (e.Args.Length == i + 1 || e.Args[i + 1].StartsWith("-"))
                {
                    arguments.Add(e.Args[i], string.Empty);
                    i--;
                }

                if (e.Args.Length >= i + 1 && !e.Args[i + 1].StartsWith("-"))
                    arguments.Add(e.Args[i], e.Args[i + 1]);
            }

            var viewModel = new ImportViewModel();
            viewModel.LoadConfig();

            if (arguments.ContainsKey("--silent"))
            {
                viewModel.Import();
                Shutdown();
            }
            else
            {
                var mainWindow = new MainWindow();
                mainWindow.DataContext = viewModel;
                mainWindow.Show();
            }
        }
    }
}
