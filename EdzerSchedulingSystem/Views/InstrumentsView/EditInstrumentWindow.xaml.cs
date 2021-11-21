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
    /// Interaction logic for EditInstrumentWindow.xaml
    /// </summary>
    public partial class EditInstrumentWindow : Window
    {
        private Instrument _instrument;
        
        public EditInstrumentWindow(int instrumentID)
        {
            InitializeComponent();
            initializeAvailableTypes();


            _instrument = Database.getInstrument(instrumentID);

            if (_instrument == null)
            {
                MessageBox.Show("Instrument not found in the database!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

            //if _instrument is not null, then initialize all fields based on instrument
            txtInstrumentID.Text = _instrument.instrumentID.ToString();
            txtInstrumentModel.Text = _instrument.instrumentModel;
            cbType.SelectedItem = _instrument.instrumentType;
            txtDescription.Text = _instrument.instrumentDescription;
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (validateData())
            {
                //add type first if type does not exist, then update
                Database.addInstrumentType(new InstrumentType
                {
                    instrumentType = cbType.Text.ToUpper(),
                    pricePerHour = float.Parse(txtPricePerHour.Text)
                });

                Database.editInstrument(new Instrument(_instrument.instrumentID, txtInstrumentModel.Text, cbType.Text.ToUpper(), txtDescription.Text));
                
                MessageBox.Show($"Instrument Information for {txtInstrumentModel.Text} has been updated!", "Instruments Updated");
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
                errorMessage += "Instrument Model cannot be empty.";
                errorFound = true;
            }

            if (tmpPrice == "")
            {
                errorMessage += "Price cannot be empty.";
                errorFound = true;
            }

            if (instrumentType == "")
            {
                errorMessage += "Instrument Type cannot be empty.";
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
