using EdzerSchedulingSystem.Models;
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

namespace EdzerSchedulingSystem.Views.SchedulesView
{
    /// <summary>
    /// Interaction logic for AddScheduleWindow.xaml
    /// </summary>
    public partial class AddScheduleWindow : Window
    {
        public List<InstrumentType> instrumentsRented = new List<InstrumentType>();

        public bool doneAdding = false; //only to know whether to trigger closing messagebox or not after successful adding of schedule

        SchedulesView _schedulesView;

        private List<Schedule> listOfSelectedDaySchedules;

        public AddScheduleWindow(object schedulesView, DateTime date)
        {
            InitializeComponent();

            _schedulesView = (SchedulesView)schedulesView;

            //Set current selected day/date to datepicker
            datePicker.SelectedDate = date.Date;
            datePicker.DisplayDate = date.Date;

            //initialize combo boxes
            initializeScheduleTypes();
            initializeStatus();
            initializeAMPM();
            initializeBands();
            initializeRepresentatives();
            initializeInstrumentTypes();

            tblInstrumentsRented.ItemsSource = instrumentsRented;

            getSelectedDaySchedule();
        }

        private void getSelectedDaySchedule()
        {
            listOfSelectedDaySchedules = Database.getScheduleOfDay(datePicker.SelectedDate.Value);
        }

        private void initializeScheduleTypes()
        {
            cbScheduleType.ItemsSource = null;
            cbScheduleType.ItemsSource = Database.getScheduleTypes();
            cbScheduleType.SelectedIndex = 0;
        }

        private void initializeStatus()
        {
            cbStatus.Items.Add("Paid");
            cbStatus.Items.Add("Not Paid");
            cbStatus.SelectedIndex = 1;
        }

        private void initializeAMPM()
        {
            cbAMPM.Items.Add("AM");
            cbAMPM.Items.Add("PM");
        }

        private void initializeBands()
        {
            cbBandName.ItemsSource = Database.getBands();
        }

        private void initializeRepresentatives()
        {
            cbRepresentativeName.ItemsSource = Database.getRepresentatives();
        }

        private void initializeInstrumentTypes()
        {
            instrumentsRented = Database.removeZeroQuantityInstruments(Database.getAllInstrumentTypes());
        }
        
        private void btnAddInstrument_Click(object sender, RoutedEventArgs e)
        {
            //get instrument type
            InstrumentType type = (InstrumentType)tblInstrumentsRented.SelectedItem;

            //check first if you can add more
            if ((type.rentedQuantity + 1) <= type.totalQuantity)
            {
                //add then adjust price
                type.rentedQuantity++;
                type.totalPrice = (type.rentedQuantity * type.pricePerHour);
            }

            tblInstrumentsRented.Items.Refresh();
            
            displayPrices();
        }

        private void btnRemoveInstrument_Click(object sender, RoutedEventArgs e)
        {
            //get instrument type
            InstrumentType type = (InstrumentType)tblInstrumentsRented.SelectedItem;

            //check first if you can add more
            if ((type.rentedQuantity - 1) >= 0)
            {
                //remove then compute price
                type.rentedQuantity--;
                type.totalPrice = (type.rentedQuantity * type.pricePerHour);
            }

            tblInstrumentsRented.Items.Refresh();
            displayPrices();
        }


        //add to database button
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //check first if fields are valid
            if (validateData())
            {
                bool continueAllowed = false;
                //if band has penalty, then ask first
                if (IsBandHasPenalty())
                {
                    if (MessageBox.Show("This band has pending penalty. Do you want to proceed?", "Band With Penalty",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        continueAllowed = true;
                    }
                } else
                {
                    continueAllowed = true;
                }

                if (continueAllowed)
                {
                    //add/check first if band already exists
                    Database.addBand(cbBandName.Text, chkPenalty.IsChecked.Value);

                    //add/check first if representative already exists
                    Database.addRepresentative(cbRepresentativeName.Text, txtContactNumber.Text);

                    //then add all details to database - addSchedule method calls rent instruments method automatically,
                    //since this method returns the ID used for newly inserted row
                    Schedule newSchedule = new Schedule();
                    newSchedule.bandID = Database.getBandID(cbBandName.Text);
                    newSchedule.scheduleTypeID = Database.getScheduleTypeID(cbScheduleType.Text);
                    newSchedule.representativeName = cbRepresentativeName.Text;
                    newSchedule.scheduleDate = datePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                    newSchedule.startTime = $"{txtHour.Text}:{txtMinute.Text} {cbAMPM.Text}";
                    newSchedule.duration = Convert.ToInt32(txtDuration.Text);
                    newSchedule.isPaid = (cbStatus.SelectedItem.ToString() == "Paid") ? true : false;

                    Database.addSchedule(newSchedule, instrumentsRented);

                    doneAdding = true;
                    _schedulesView.jumpToDate(datePicker.SelectedDate.Value);
                    _schedulesView._parent.dashboard.refresh();
                    this.Close();
                }

            }
        }

        private bool validateData()
        {
            //store fields to be checked in variables
            string bandName = cbBandName.Text.Trim();
            string representativeName = cbRepresentativeName.Text.Trim();
            string tmpHour = txtHour.Text;
            string tmpMinute = txtMinute.Text;
            string tmpDuration = txtDuration.Text;

            string errorMessage = "";
            bool errorFound = false;

            //check each condition for error, if error found, add error message then change errorfound bool to true
            if (bandName == "")
            {
                errorMessage += "Band Name cannot be empty.\n";
                errorFound = true;
            }

            if (representativeName == "")
            {
                errorMessage += "Representative Name cannot be empty.\n";
                errorFound = true;
            }

            if (tmpHour != "")
            {
                int hour = Convert.ToInt32(tmpHour);
                if (hour > 12 || hour < 1)
                {
                    errorMessage += "Invalid start time 'hour'.\n";
                    errorFound = true;
                }
            } else
            {
                errorMessage += "Empty start time 'hour'.\n";
                errorFound = true;
            }

            if (tmpMinute != "")
            {
                int minute = Convert.ToInt32(tmpMinute);
                if (minute > 59 || minute < 0)
                {
                    errorMessage += "Invalid start time 'minute'.\n";
                    errorFound = true;
                }
            }
            else
            {
                errorMessage += "Empty start time 'minute'.\n";
                errorFound = true;
            }

            if (tmpDuration != "")
            {
                int duration = Convert.ToInt32(tmpDuration);
                if (duration > 24 || duration < 1)
                {
                    errorMessage += "Invalid hours of duration.\n";
                    errorFound = true;
                }
            }
            else
            {
                errorMessage += "'Duration' cannot be empty.\n";
                errorFound = true;
            }


            if (cbStatus.SelectedIndex == -1)
            {
                errorMessage += "Select 'Status' option.\n";
                errorFound = true;
            }

            if (cbAMPM.SelectedIndex == -1)
            {
                errorMessage += "Select AM/PM option.\n";
                errorFound = true;
            }

            if (cbScheduleType.SelectedIndex == -1)
            {
                errorMessage += "Select 'Recording Type' option.\n";
                errorFound = true;
            }

            
            DateTime selectedDate = DateTime.Parse($"{datePicker.SelectedDate.Value.ToString("MMM-dd-yyyy")} {txtHour.Text}:{txtMinute.Text} {cbAMPM.Text}");
            if (DateTime.Compare(DateTime.Now, selectedDate) > 0 && !errorFound)
            {
                errorMessage += "Can't plot schedule for date/time already passed.\n";
                errorFound = true;
            }

            getSelectedDaySchedule();
            if (!errorFound)
            {
                foreach (Schedule s in listOfSelectedDaySchedules)
                {
                    DateTime existingStartTime = DateTime.Parse(s.startTime);
                    DateTime existingEndTime = existingStartTime.AddHours(s.duration);

                    DateTime scheduleStartTime = DateTime.Parse($"{txtHour.Text}:{txtMinute.Text} {cbAMPM.Text}");
                    DateTime scheduleEndTime = scheduleStartTime.AddHours(int.Parse(txtDuration.Text));

                    if (((scheduleStartTime >= existingStartTime) && (scheduleStartTime < existingEndTime)) ||
                        ((scheduleEndTime > existingStartTime) && (scheduleEndTime < existingEndTime)))
                    {
                        errorMessage += "Time selected has a conflict with an existing schedule. Please check time availability again.\n";
                        errorFound = true;
                        break;
                    }
                }
            }
            

            errorMessage = errorMessage.Trim();
            if (errorFound) MessageBox.Show(errorMessage, "Error/s Found", MessageBoxButton.OK, MessageBoxImage.Error);

            return !errorFound;
        }

        private bool IsBandHasPenalty()
        {
            return Database.getBandPenalty(Database.getBandID(cbBandName.Text));
        }


        private void displayPrices()
        {
            float additionalFees = 0;
            float totalAmount = 0;
            float studioRate = Database.getScheduleTypePrice(Database.getScheduleTypeID(cbScheduleType.SelectedItem.ToString()), datePicker.SelectedDate.Value.ToString("yyyy-MM-dd"));

            //get hours first
            int hours = 0;
            if (txtDuration.Text.Trim() != "")
            {
                hours = int.Parse(txtDuration.Text);
            }

            //then set studio rate
            txtStudioRate.Text = studioRate.ToString("0.00" + " PHP");

            //then add all addons using priceperhour and duration
            foreach (InstrumentType it in tblInstrumentsRented.Items)
            {
                additionalFees += it.totalPrice * hours; //total price is quantity times price per hour only
            }

            //then set price additional
            txtAdditionalFees.Text = additionalFees.ToString("0.00" + " PHP");

            //then display total amount
            totalAmount = (studioRate * hours) + additionalFees;
            lblTotalAmount.Content = totalAmount.ToString("0.00" + " PHP");
        }

        private void cbRepresentativeName_TextChanged(object sender, RoutedEventArgs e)
        {
            txtContactNumber.Text = Database.getContactNumber(cbRepresentativeName.Text);

        }

        private void cbScheduleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            displayPrices();
        }

        private void cbBandName_TextChanged(object sender, RoutedEventArgs e)
        {
            chkPenalty.IsChecked = Database.getBandPenalty(Database.getBandID(cbBandName.Text));
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            displayPrices();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!doneAdding)
            {
                if (MessageBox.Show("Cancel booking schedule?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }

        }


        //TEXTBOX VALIDATION
        private readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private bool IsText(string text)
        {
            return _regex.IsMatch(text); //check if matches text
        }

        private void validateNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsText(e.Text);
        }

        private void txtHour_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtHour.Text == "" || txtHour.Text.Contains("."))
            {
                txtHour.Text = "01";
                return;
            }

            if (Convert.ToInt32(txtHour.Text) > 12)
            {
                txtHour.Text = "12";
            } else if (Convert.ToInt32(txtHour.Text) < 1)
            {
                txtHour.Text = "1";
            }
        }

        private void txtMinute_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMinute.Text == "" || txtMinute.Text.Contains("."))
            {
                txtMinute.Text = "00";
                return;
            }

            if (Convert.ToInt32(txtMinute.Text) > 59)
            {
                txtMinute.Text = "59";
            }
            else if (Convert.ToInt32(txtMinute.Text) < 0)
            {
                txtMinute.Text = "0";
            }
        }

        private void txtDuration_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDuration.Text == "" || txtDuration.Text.Contains("."))
            {
                txtDuration.Text = "1";
                return;
            }

            if (Convert.ToInt32(txtDuration.Text) > 24)
            {
                txtDuration.Text = "24";
            }
            else if (Convert.ToInt32(txtDuration.Text) < 1)
            {
                txtDuration.Text = "1";
            }

            displayPrices();
        }

        
    }
}
