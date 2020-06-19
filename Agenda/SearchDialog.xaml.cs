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
using System.Windows.Shapes;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for SearchDialog.xaml
    /// </summary>
    public partial class SearchDialog : Window
    {
        public string SearchString { get; set; }
        public SearchDialog()
        {
            InitializeComponent();
            Loaded += SearchDialog_Loaded;
        }

        private void SearchDialog_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += SearchDialog_KeyDown;
        }

        private void SearchDialog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (searchButton.IsEnabled)
                        HandleOk();
                    break;
                case Key.Escape:
                    HandleCancel();
                    break;
            }
        }

        private void Search_Ok(object sender, RoutedEventArgs e)
        {
            HandleOk();
        }

        private void Search_Cancel(object sender, RoutedEventArgs e)
        {
            HandleCancel();
        }

        private void HandleOk()
        {
            SearchString = CajaBusqueda.Text.ToLower();
            DialogResult = true;
            Close();
        }

        private void HandleCancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
