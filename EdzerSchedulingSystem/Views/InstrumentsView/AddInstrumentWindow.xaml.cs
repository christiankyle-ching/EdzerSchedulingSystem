using EdzerSchedulingSystem.Models;
using EdzerSchedulingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for AddInstrumentWindow.xaml
    /// </summary>
    public partial class AddInstrumentWindow : Window
    {   
        public AddInstrumentWindow()
        {
            InitializeComponent();

            initializeAvailableTypes();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (validateData())
            {
                //add instrument type first using sql
                //Database.addInstrumentType(new InstrumentType
                //{
                //    instrumentType = cbType.Text.ToUpper(),
                //    pricePerHour = float.Parse(txtPricePerHour.Text)
                //});

                //then add instrument itself
                Database.addInstrument(new Instrument(-1, txtInstrumentModel.Text, cbType.Text.ToUpper(), txtDescription.Text)); //-1 only for placeholder as it should be null for db
                MessageBox.Show($"InstrumentModel for {txtInstrumentModel.Text} has been added!", "Instrument Added");
            }
            
            this.Close();
        }

        private bool validateData()
        {
            //store fields to be checked in variables
            string instrumentModel = txtInstrumentModel.Text;
            string instrumentType = cbType.Text;
            string tmpPrice = txtPricePerHour.Text;

            //initialize error bool and error message
            string errorMessage = "";
            bool errorFound = false;

            //check each condition for error, if error found, add error message then change errorfound bool to true
            //check if instrument model is empty
            if (instrumentModel == "")
            {
                errorMessage += "Instrument Model cannot be empty.\n";
                errorFound = true;
            }

            if (tmpPrice == "")
            {
                errorMessage += "Price cannot be empty.\n";
                errorFound = true;
            }

            if (instrumentType == "")
            {
                errorMessage += "Instrument Type cannot be empty.\n";
                errorFound = true;
            }

            errorMessage = errorMessage.Trim();
            if (errorFound) MessageBox.Show(errorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Information);

            return !errorFound;
        }

        private void initializeAvailableTypes()
        {
            foreach (InstrumentType it in Database.getAllInstrumentTypes())
            {
                cbType.Items.Add(it.instrumentType);
            }
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtPricePerHour.Text = Database.getInstrumentTypePricePerHour(cbType.SelectedItem.ToString(), DateTime.Now.ToString("yyyy-MM-dd")).ToString();
        }

        //TEXTBOX VALIDATION
        private readonly Regex _regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
        private bool IsText(string text)
        {
            return _regex.IsMatch(text); //check if matches text
        }

        private void validateNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsText(e.Text);
        }
        
    }
}
