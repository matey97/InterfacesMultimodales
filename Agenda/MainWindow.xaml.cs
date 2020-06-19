using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Person> persons = new List<Person>();

        private int currentIndex;
        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                currentIndex = value;
                if (currentIndex < 0 || currentIndex >= persons.Count)
                {
                    prevButton.IsEnabled = false;
                    nextButton.IsEnabled = false;
                    modifyButton.IsEnabled = false;
                    deleteButton.IsEnabled = false;
                    ElSlider.IsEnabled = false;

                    CajaNombre.Text = "";
                    CajaApellidos.Text = "";
                    CB_IsFriend.IsChecked = false;
                    CB_Man.IsSelected = false;
                    CB_Woman.IsSelected = false;
                    CB_Unknown.IsSelected = false;

                    return;
                }

                updateDisplayedPerson();
                updateControls();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentIndex = -1;
        }

        private void updateDisplayedPerson()
        {
            var person = persons[CurrentIndex];
            CajaNombre.Text = person.FirstName;
            CajaApellidos.Text = person.LastName;
            CB_IsFriend.IsChecked = person.IsFriend;
            CB_Man.IsSelected = person.Sex == SexEnum.Man;
            CB_Woman.IsSelected = person.Sex == SexEnum.Woman;
            CB_Unknown.IsSelected = person.Sex == SexEnum.Unknown;
        }

        private void updateControls()
        {
            prevButton.IsEnabled = (CurrentIndex == 0) ? false : true;
            nextButton.IsEnabled = (CurrentIndex == persons.Count - 1) ? false : true;
            modifyButton.IsEnabled = true;
            deleteButton.IsEnabled = true;
            ElSlider.IsEnabled = true;
            ElSlider.Value = persons.Count > 1 ? CurrentIndex * (100.0 / (persons.Count - 1)) : 0;
        }

        private void ButtonPrev(object sender, RoutedEventArgs e)
        {
            CurrentIndex--;
        }

        private void ButtonNext(object sender, RoutedEventArgs e)
        {
            CurrentIndex++;
        }

        private void ButtonModify(object sender, RoutedEventArgs e)
        {
            var person = persons[CurrentIndex];
            person.FirstName = CajaNombre.Text;
            person.LastName = CajaApellidos.Text;
            person.IsFriend = CB_IsFriend.IsChecked.Value;
            person.Sex = CB_Man.IsSelected == true ?
                    SexEnum.Man :
                    CB_Woman.IsSelected == true ?
                        SexEnum.Woman :
                        SexEnum.Unknown;
        }

        private void ButtonAdd(object sender, RoutedEventArgs e)
        {
            AddPersonDialog addModal = new AddPersonDialog();
            bool result = addModal.ShowDialog() == true;

            if (result)
            {
                persons.Add(addModal.NewPerson);
                CurrentIndex = persons.Count - 1;
            }
        }

        private void ButtonDelete(object sender, RoutedEventArgs e)
        {
            persons.RemoveAt(CurrentIndex);

            if (persons.Count == 0 || CurrentIndex != 0)
                CurrentIndex--;
            else
                CurrentIndex = CurrentIndex; //Force update displayed person and controls
        }

        private void ElSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentIndex = (int) Math.Floor(e.NewValue / (100.0 / persons.Count));
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            persons.Clear();
            CurrentIndex = -1;
        }

        private void MenuAbrir_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            bool result = ShowFileDialog(openFileDialog);

            if (result)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Person>));
                using (Stream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    persons = (List<Person>)serializer.Deserialize(reader);
                        
                    CurrentIndex = persons.Count > 0 ? 0 : -1;
                }  
            }
        }

        private void MenuGuardar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            bool result = ShowFileDialog(saveFileDialog);

            if (result)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Person>));
                using (Stream writer = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(writer, persons);

                    MessageBox.Show("Contactos guardados correctamente",
                        "Guardar",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        private bool ShowFileDialog(FileDialog dialog)
        {
            dialog.FileName = "persons";
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML documents (.xml)|*.xml";

            return dialog.ShowDialog() == true;
        }

        private void MenuSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            SearchDialog searchDialog = new SearchDialog();
            bool result = searchDialog.ShowDialog() == true;

            if (!result)
                return;

            var searchString = searchDialog.SearchString;
            var i = 0;
            foreach (var person in persons)
            {
                if (person.FirstName.ToLower().Contains(searchString) || 
                    person.LastName.ToLower().Contains(searchString))
                {
                    CurrentIndex = i;
                    return;
                }
                i++;
            }

            MessageBox.Show("No se ha encontrado ninguna coincidencia",
                "Búsqueda",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
