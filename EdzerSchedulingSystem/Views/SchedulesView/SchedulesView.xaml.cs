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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EdzerSchedulingSystem.Views.SchedulesView
{
    /// <summary>
    /// Interaction logic for SchedulesView.xaml
    /// </summary>
    public partial class SchedulesView : UserControl
    {
        #region Animation objects

        private ScaleTransform scaleToSmall;
        private DoubleAnimation scaleAnimation = new DoubleAnimation(1.1, 1.0, TimeSpan.FromMilliseconds(400));

        #endregion

        public MainWindow _parent;

        DateTime _selectedDate;
        private Schedule selectedSchedule;
        public List<InstrumentType> instrumentsRented;
        private List<Schedule> schedulesOfDay;
        public bool isEditing = false;
        

        public SchedulesView(object sender)
        {
            InitializeComponent();

            #region Animations
            //Initialize Animations
            scaleToSmall = new ScaleTransform();
            txtSelectedDate.RenderTransform = scaleToSmall;
            txtSelectedDay.RenderTransform = scaleToSmall;
            #endregion

            _parent = (MainWindow)sender;
            
            //Set current selected day/date to textblocks
            calendar.SelectedDate = DateTime.Now;
            _selectedDate = DateTime.Now;

            //initialize combo boxes
            initializeScheduleTypes();
            initializeStatus();
            initializeAMPM();
            
        }

        public void initializeScheduleTypes()
        {
            cbScheduleType.ItemsSource = null;
            cbScheduleType.ItemsSource = Database.getScheduleTypes();
        }

        private void initializeStatus()
        {
            cbStatus.Items.Add("Paid");
            cbStatus.Items.Add("Not Paid");
        }

        private void initializeAMPM()
        {
            cbAMPM.Items.Add("AM");
            cbAMPM.Items.Add("PM");
        }
        

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar.SelectedDate == null)
            {
                schedulesOfDay.Clear();
                return;
            }

            if (isContinueAllowed())
            {
                //Change TextBlocks
                txtSelectedDay.Text = calendar.SelectedDate.Value.ToString("ddd").ToUpper() + " ";
                txtSelectedDate.Text = calendar.SelectedDate.Value.ToString("MM/dd/yyyy");

                //Animation
                scaleToSmall.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scaleToSmall.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

                //update list of schedules
                schedulesOfDay = Database.getScheduleOfDay(calendar.SelectedDate.Value);
                displaySchedulesList();
                listSchedules.UnselectAll();
                txtSearch.Text = "";
            }
            
        }

        private void listSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSchedules.SelectedItem == null && isEditing == true)
            {
                btnCancel_Click(this, null);
                return;
            }
            else if (listSchedules.SelectedItem == null && isEditing == false)
            {
                selectedSchedule = null;
                clearAll();
                btnEditSchedule.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnReport.IsEnabled = false;
                return;
            }

            selectedSchedule = (Schedule)listSchedules.SelectedItem;

            displayScheduleData(selectedSchedule.scheduleID);
            btnReport.IsEnabled = true;
            btnEditSchedule.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnReport.IsEnabled = true;
            
        }

        private void cbScheduleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isEditing == true)
            {
                displayPrices();
            }
        }

        private void listSchedules_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                listSchedules.UnselectAll();
            }
        }

        
        private void displaySchedulesList()
        {
            listSchedules.ItemsSource = null;
            listSchedules.ItemsSource = Database.getScheduleOfDay(calendar.SelectedDate.Value);
        }

        public void jumpToDate(DateTime date)
        {
            //set selected date
            calendar.SelectedDate = date;
            calendar.DisplayDate = date;
            
        }


        private bool isContinueAllowed()
        {
            //if still editing, ask to cancel first
            if (isEditing == true)
            {
                btnCancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

            //if cancelled, then continue
            if (isEditing == false)
            {
                return true;
            }

            return false;
        }

        private void enableAll()
        {
            cbStatus.IsEnabled = true;
            cbScheduleType.IsEnabled = true;
            datePicker.IsEnabled = true;
            txtHour.IsEnabled = true;
            txtMinute.IsEnabled = true;
            cbAMPM.IsEnabled = true;
            txtDuration.IsEnabled = true;
            tblInstrumentsRented.IsEnabled = true;
            btnCancel.IsEnabled = true;
            chkPenalty.IsEnabled = true;

            btnAddSchedule.IsEnabled = false;
            listSchedules.IsEnabled = false;
            _parent.btnSettings.IsEnabled = false;
            _parent.btnAbout.IsEnabled = false;
            _parent.btnLogout.IsEnabled = false;
        }

        private void disableAll()
        {
            cbStatus.IsEnabled = false;
            cbScheduleType.IsEnabled = false;
            datePicker.IsEnabled = false;
            txtHour.IsEnabled = false;
            txtMinute.IsEnabled = false;
            cbAMPM.IsEnabled = false;
            txtDuration.IsEnabled = false;
            tblInstrumentsRented.IsEnabled = false;
            btnCancel.IsEnabled = false;
            chkPenalty.IsEnabled = false;

            btnAddSchedule.IsEnabled = true;
            listSchedules.IsEnabled = true;
            _parent.btnSettings.IsEnabled = true;
            _parent.btnAbout.IsEnabled = true;
            _parent.btnLogout.IsEnabled = true;
        }

        private void clearAll()
        {
            instrumentsRented.Clear();

            txtBandName.Text = "";
            txtRepresentative.Text = "";
            txtScheduleID.Text = "";
            cbStatus.SelectedIndex = -1;
            cbScheduleType.SelectedIndex = -1;
            txtDuration.Text = "";
            datePicker.SelectedDate = null;
            txtHour.Text = "";
            txtMinute.Text = "";
            cbAMPM.SelectedIndex = -1;
            tblInstrumentsRented.ItemsSource = null;
            chkPenalty.IsChecked = false;
            txtHasPenalty.Visibility = Visibility.Hidden;

            txtStudioRate.Text = "";
            txtAdditionalFees.Text = "";

            lblTotalAmount.Content = "";
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text == "")
            {
                displaySchedulesList();
                return;
            }

            List<Schedule> foundSchedules = new List<Schedule>();
            foreach (Schedule s in schedulesOfDay)
            {
                if (s.bandID.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper()) ||
                    s.bandName.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper()) ||
                    s.representativeName.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper()) ||
                    s.scheduleDate.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper()) ||
                    s.scheduleID.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper()) ||
                    s.scheduleType.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper()) ||
                    s.startTime.ToString().ToUpper().Contains(txtSearch.Text.Trim().ToUpper())
                    )
                {

                    foundSchedules.Add(s);
                }
            }

            listSchedules.ItemsSource = null;
            listSchedules.ItemsSource = foundSchedules;
        }

        private void displayScheduleData(int scheduleID)
        {
            //get schedule data from db
            selectedSchedule = Database.getScheduleDetails(scheduleID);

            //set textboxes based on scheduleID
            txtBandName.Text = selectedSchedule.bandName;
            txtRepresentative.Text = selectedSchedule.representativeName;
            txtScheduleID.Text = "Schedule #" + selectedSchedule.scheduleID.ToString();
            if (selectedSchedule.isPaid == true)
            {
                cbStatus.SelectedItem = "Paid";
            } else
            {
                cbStatus.SelectedItem = "Not Paid";
            }
            cbScheduleType.SelectedItem = selectedSchedule.scheduleType;
            txtDuration.Text = selectedSchedule.duration.ToString();
            datePicker.SelectedDate = DateTime.Parse(selectedSchedule.scheduleDate);
            txtHour.Text = DateTime.Parse(selectedSchedule.startTime).ToString("hh");
            txtMinute.Text = DateTime.Parse(selectedSchedule.startTime).ToString("mm");
            cbAMPM.SelectedItem = DateTime.Parse(selectedSchedule.startTime).ToString("tt");
            chkPenalty.IsChecked = selectedSchedule.hasPenalty;
            txtHasPenalty.Visibility = (selectedSchedule.hasPenalty) ? Visibility.Visible : Visibility.Hidden;

            displayInstrumentsRented(selectedSchedule.scheduleID);
            
            txtStudioRate.Text = selectedSchedule.studioRate.ToString();

            displayPrices();
        }

        private void displayInstrumentsRented(int scheduleID)
        {
            tblInstrumentsRented.ItemsSource = null;
            instrumentsRented = Database.getScheduleInstrumentTypes(scheduleID, selectedSchedule.scheduleDate);
            tblInstrumentsRented.ItemsSource = Database.removeZeroQuantityInstruments(instrumentsRented);
        }

        private void displayPrices()
        {
            float additionalFees = 0;
            float totalAmount = 0;
            float studioRate = Database.getScheduleTypePrice(selectedSchedule.scheduleTypeID, selectedSchedule.scheduleDate);

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

        


        
        private void btnAddInstrument_Click(object sender, RoutedEventArgs e)
        {
            //get instrument type
            InstrumentType type = (InstrumentType) tblInstrumentsRented.SelectedItem;

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

        private void btnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            AddScheduleWindow window = new AddScheduleWindow(this, calendar.SelectedDate.Value);
            window.ShowDialog();

            displaySchedulesList();
        }

        private void btnEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSchedule == null) return;

            if (isEditing == false)
            {
                isEditing = true;
                btnEditSchedule.Content = "Save";
                btnDelete.IsEnabled = false;
                btnReport.IsEnabled = false;
                enableAll();
            }
            else
            {
                //SAVING
                if (validateData()) { 
                    if (MessageBox.Show($"Do you want to save changes to Schedule #{selectedSchedule.scheduleID}?", "Save Changes?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        //store temporary schedule on a schedule object, then push to sql method on database class
                        Schedule schedule = new Schedule
                        {
                            scheduleID = selectedSchedule.scheduleID,
                            scheduleTypeID = Database.getScheduleTypeID(cbScheduleType.SelectedItem.ToString()),
                            bandID = selectedSchedule.bandID,
                            scheduleType = cbScheduleType.SelectedItem.ToString(),
                            representativeName = selectedSchedule.representativeName,
                            scheduleDate = datePicker.SelectedDate.Value.ToString("yyyy-MM-dd"),
                            startTime = DateTime.Parse($"{txtHour.Text}:{txtMinute.Text} {cbAMPM.SelectedItem.ToString()}").ToString("HH:mm"),
                            duration = int.Parse(txtDuration.Text),
                            isPaid = (cbStatus.SelectedItem.ToString() == "Paid") ? true : false,
                            hasPenalty = chkPenalty.IsChecked.Value
                        };

                        Database.updateBandPenalty(selectedSchedule.bandID, schedule.hasPenalty);
                        Database.updateSchedule(schedule);
                        

                        //Database.updateInstrumentsRented(selectedSchedule.scheduleID, listInstrumentsRented);
                        Database.deleteInstrumentsRented(selectedSchedule.scheduleID);
                        Database.rentInstruments(selectedSchedule.scheduleID, instrumentsRented);

                        //change status after save
                        isEditing = false;
                        btnEditSchedule.Content = "Edit";
                        listSchedules.UnselectAll();
                        disableAll();

                        //refresh listview
                        displaySchedulesList();

                        //refresh dashboard too
                        _parent.dashboard.refresh();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            

            if (MessageBox.Show($"Do you want to delete Schedule#{selectedSchedule.scheduleID}?", "Delete Schedule?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_parent.currentUser.isAdmin == false)
                {
                    MessageBox.Show($"Please notify the admin for cancellation of Schedule#{selectedSchedule.scheduleID}", "Delete Schedule?", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                Database.deleteSchedule(selectedSchedule.scheduleID);
            }
            
            displaySchedulesList();
        }

        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing == true)
            {
                if (MessageBox.Show("Do you want to cancel editing?", "Cancel Edit?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    isEditing = false;
                    btnEditSchedule.Content = "Edit";
                    disableAll();
                    listSchedules.UnselectAll();
                    listSchedules_SelectionChanged(this, null);
                }
            }
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow window = new ReportWindow(selectedSchedule.scheduleID, instrumentsRented);
            window.ShowDialog();
        }

        private bool validateData()
        {
            //store fields to be checked in variables
            string tmpHour = txtHour.Text;
            string tmpMinute = txtMinute.Text;
            string tmpDuration = txtDuration.Text;

            string errorMessage = "";
            bool errorFound = false;

            //check each condition for error, if error found, add error message then change errorfound bool to true
            if (tmpHour != "")
            {
                int hour = Convert.ToInt32(tmpHour);
                if (hour > 12 || hour < 1)
                {
                    errorMessage += "Invalid start time 'hour'.\n";
                    errorFound = true;
                }
            }
            else
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

            //INSERT - check if schedule time is already occupied
            if (!errorFound)
            {
                List<Schedule> tmpListOfSchedule = Database.getScheduleOfDay(datePicker.SelectedDate.Value);
                foreach (Schedule s in tmpListOfSchedule)
                {
                    if (s.scheduleID != selectedSchedule.scheduleID)
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
            }
            

            errorMessage = errorMessage.Trim();
            if (errorFound) MessageBox.Show(errorMessage, "Error/s Found", MessageBoxButton.OK, MessageBoxImage.Error);

            return !errorFound;
        }


        //TEXTBOX VALIDATION
        private readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private bool IsText(string text)
        {
            return _regex.IsMatch(text); //check if matches text
        }

        private void txtHour_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtHour.Text == "")
            {
                txtHour.Text = "01";
                return;
            }

            if (Convert.ToInt32(txtHour.Text) > 12)
            {
                txtHour.Text = "12";
            }
            else if (Convert.ToInt32(txtHour.Text) < 1)
            {
                txtHour.Text = "1";
            }
        }

        private void txtMinute_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMinute.Text == "")
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
            if (txtDuration.Text == "")
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

        private void validateNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsText(e.Text);
        }

        
    }

}
