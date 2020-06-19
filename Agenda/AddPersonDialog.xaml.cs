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
    /// Interaction logic for AddPersonDialog.xaml
    /// </summary>
    public partial class AddPersonDialog : Window
    {
        public Person NewPerson { get; set; }

        public AddPersonDialog()
        {
            InitializeComponent();
            Loaded += AddPersonModal_Loaded;
        }

        private void AddPersonModal_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += AddPersonDialog_KeyDown;
        }

        private void AddPersonDialog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (acceptButton.IsEnabled)
                        HandleAccept();
                    break;
                case Key.Escape:
                    HandleCancel();
                    break;
            }
        }

        private void Add_Accept(object sender, RoutedEventArgs e)
        {
            HandleAccept();
        }

        private void Add_Cancel(object sender, RoutedEventArgs e)
        {
            HandleCancel();
        }

        private void HandleAccept()
        {
            NewPerson = new Person(Add_CajaNombre.Text,
                Add_CajaApellidos.Text,
                Add_CB_IsFriend.IsChecked == true,
                Add_CB_Man.IsSelected == true ?
                    SexEnum.Man :
                    Add_CB_Woman.IsSelected == true ?
                        SexEnum.Woman :
                        SexEnum.Unknown);
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
