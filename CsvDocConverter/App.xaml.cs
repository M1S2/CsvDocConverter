using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CsvDocConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Eine unbehandelter Fehler ist aufgetreten: " + e.Exception.Message, "Unbehandelter Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
