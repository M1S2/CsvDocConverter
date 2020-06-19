using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Words.NET;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CsvDocConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the DataContext for your View
            MainViewModel mainViewModel = new MainViewModel(DialogCoordinator.Instance);
            this.Closing += mainViewModel.OnWindowClosing;
            this.Drop += mainViewModel.OnFileDrop;
            this.DataContext = mainViewModel;
        }
    }
}
