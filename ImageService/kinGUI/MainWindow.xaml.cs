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
using Microsoft.Practices.Prism.Commands;

namespace kinGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private VClient client;
        //private VModelSetting viewModel;

        public MainWindow()
        {
            InitializeComponent();
            //this.client = new VClient("127.0.0.1", 5555);
          //  this.viewModel = new VModelSetting();
            //this.DataContext = this.viewModel;
            //((Vie this.DataContex
        }

        private void TextBlock_StylusOutOfRange(object sender, StylusEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
