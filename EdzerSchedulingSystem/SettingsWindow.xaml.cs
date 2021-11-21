
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

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        bool scheduleIsTrue;

        bool addIsTrue;
        float instrumentRate = 0;
        float studioRate = 0;
        public SettingsWindow()
        {
            InitializeComponent();
        }


        private void ScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            initializeScheduleBtn();
        }

        private void initializeScheduleTypes()
        {
            cbBox.ItemsSource = null;
            cbBox.Items.Clear();
            cbBox.ItemsSource = Database.getScheduleTypes();
            cbBox.SelectedIndex = 0;
            studioRate = Database.getScheduleTypePrice(Database.getScheduleTypeID(cbBox.SelectedItem.ToString()), DateTime.Now.ToString("yyyy-MM-dd"));
            pricetb.Text = studioRate.ToString();

        }

        private void initalizeInstrumentType()
        {
            cbBox.ItemsSource = null;
            cbBox.Items.Clear();
            foreach (InstrumentType it in Database.getAllInstrumentTypes())
            {
                cbBox.Items.Add(it.instrumentType);
            }
            cbBox.SelectedIndex = 0;
            instrumentRate = Database.getInstrumentTypePricePerHour(cbBox.SelectedItem.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
            pricetb.Text = instrumentRate.ToString();
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //if (addIsTrue)
            //{
            //    if (pricetb.Text == null)
            //    {
            //        MessageBox.Show("Please input price");
            //    }
            //    else 
            //    {
            //        Database.addScheduleType(newTypeTB.Text.ToString(), Int32.Parse(pricetb.Text.ToString()));
            //        pricetb.Text = "";
            //        MessageBox.Show($"Schedule Type {newTypeTB.Text.ToString()} has been added!", "ScheduleType Added");
            //    }
            //}
            //else if (!addIsTrue)
            //{
            // if (scheduleIsTrue)
            //{
            //    Database.updateScheduleType(cbBox.SelectedItem.ToString(), Int32.Parse(pricetb.Text.ToString()));

            //    MessageBox.Show($"Schedule Type for {cbBox.SelectedItem.ToString()} has been updated!", "ScheduleType Updated");
            //}
            // else if (!scheduleIsTrue)
            //{
            //    Database.updateInstrumentType(cbBox.SelectedItem.ToString(), Int32.Parse(pricetb.Text.ToString()));

            //    MessageBox.Show($"Instrument Type for {cbBox.SelectedItem.ToString()} has been updated!", "InstrumentType Updated");
            //}
            //}
            if (addIsTrue)
            {
                if (scheduleIsTrue)
                {
                    if (pricetb.Text == null)
                    {
                        MessageBox.Show("Please input price");
                    }
                    else
                    {
                        Database.addScheduleType(newTypeTB.Text.ToString());
                        Database.addScheduleTypePrice(newTypeTB.Text.ToString(), float.Parse(pricetb.Text.ToString()));
                        pricetb.Text = "";
                        MessageBox.Show($"Successfully added {newTypeTB.Text} with price of {float.Parse(pricetb.Text).ToString("0.00 PHP")}", "New Schedule Type Added", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (!scheduleIsTrue)
                {
                    if (pricetb.Text == null)
                    {
                        MessageBox.Show("Please input price");
                    }
                    else
                    {
                        Database.addInstrumentType(new InstrumentType
                        {
                            instrumentType = newTypeTB.Text,
                            pricePerHour = float.Parse(pricetb.Text)
                        });

                        MessageBox.Show($"Successfully added {newTypeTB.Text} with price of {float.Parse(pricetb.Text).ToString("0.00 PHP")}", "New Instrument Type Added", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else if (!addIsTrue)
            {
                if (scheduleIsTrue)
                {
                    if (pricetb.Text == studioRate.ToString())
                    {
                        MessageBox.Show("Value did not change, no updates occured");
                    }
                    else
                    {
                        Database.updateScheduleTypePrice(cbBox.SelectedItem.ToString(), float.Parse(pricetb.Text.ToString()), datePicker.SelectedDate.Value.ToString("yyyy-MM-dd 00:00:00"));

                        MessageBox.Show($"Schedule Type for {cbBox.SelectedItem.ToString()} has been updated!\nNew Price of {float.Parse(pricetb.Text.ToString()).ToString("0.00 PHP")} to take effect on {datePicker.SelectedDate.Value.ToString("MMMM-dd-yyyy")}", "ScheduleType Updated");
                    }
                }
                else if (!scheduleIsTrue)
                {
                    if (pricetb.Text == instrumentRate.ToString())
                    {
                        MessageBox.Show("Value did not change, no updates occured");
                    }
                    else
                    {
                        Database.updateInstrumentTypePrice(cbBox.SelectedItem.ToString(), float.Parse(pricetb.Text.ToString()), datePicker.SelectedDate.Value.ToString("yyyy-MM-dd 00:00:00"));

                        MessageBox.Show($"Instrument Type for {cbBox.SelectedItem.ToString()} has been updated!\nNew Price of {float.Parse(pricetb.Text.ToString()).ToString("0.00 PHP")} to take effect on {datePicker.SelectedDate.Value.ToString("MMMM-dd-yyyy")}", "InstrumentType Updated");
                    }
                }
            }
        }

        private void cbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox.SelectedItem == null)
            {
                return;
            }

            if (scheduleIsTrue)
            {
                studioRate = Database.getScheduleTypePrice(Database.getScheduleTypeID(cbBox.SelectedItem.ToString()), DateTime.Now.ToString("yyyy-MM-dd"));
                pricetb.Text = studioRate.ToString();
            }
            else if (!scheduleIsTrue)
            {
                instrumentRate = Database.getInstrumentTypePricePerHour(cbBox.SelectedItem.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                pricetb.Text = instrumentRate.ToString();
            }
        }

        private void instrumentBtn_Click(object sender, RoutedEventArgs e)
        {
            initializeInstrumentBtn();
        }


        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            ChangelogWindow window = new ChangelogWindow();
            window.ShowDialog();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Editcb_Checked_1(object sender, RoutedEventArgs e)
        {
            addIsTrue = false;
            btnEdit.Content = "Edit";
            newTBlock.Visibility = Visibility.Collapsed;
            newTypeTB.Visibility = Visibility.Collapsed;
            cbBox.Visibility = Visibility.Visible;
            tBlock.Visibility = Visibility.Visible;
            cbBox.IsEnabled = true;
            tBlock.IsEnabled = true;
            newTypeTB.IsEnabled = false;
            datePicker.Visibility = Visibility.Visible;
            DateLbl.Visibility = Visibility.Visible;
            if (scheduleIsTrue)
            {
                tBlock.Content = "Schedule Type";
                initializeScheduleTypes();
            }
            else if (!scheduleIsTrue)
            {
                tBlock.Content = "Instrument Type";
                initalizeInstrumentType();
            }

        }

        private void Addcb_Checked_1(object sender, RoutedEventArgs e)
        {
            addIsTrue = true;
            pricetb.Text = "";
            btnEdit.Content = "Add";
            newTBlock.Visibility = Visibility.Visible;
            newTypeTB.Visibility = Visibility.Visible;
            cbBox.Visibility = Visibility.Collapsed;
            tBlock.Visibility = Visibility.Collapsed;
            datePicker.Visibility = Visibility.Collapsed;
            cbBox.IsEnabled = false;
            tBlock.IsEnabled = true;
            newTypeTB.IsEnabled = true;
            DateLbl.Visibility = Visibility.Collapsed;
            if (scheduleIsTrue)
            {
                tBlock.Content = "Schedule Type";
            }
            else if (!scheduleIsTrue)
            {
                tBlock.Content = "Instrument Type";
            }
        }

        //Data Validation
        private readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private bool IsText(string text)
        {
            return _regex.IsMatch(text); //check if matches text
        }

        private void validateNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsText(e.Text);
        }

        //initialize;
        private void initializeInstrumentBtn()
        {
            datePicker.Text = "";
            scheduleIsTrue = false;
            btnEdit.IsEnabled = true;
            Addcb.IsEnabled = true;
            Editcb.IsEnabled = true;
            Editcb.Visibility = Visibility.Visible;
            Addcb.Visibility = Visibility.Visible;
            newTBlock.Visibility = Visibility.Collapsed;
            newTypeTB.Visibility = Visibility.Collapsed;
            btnEdit.Content = "Edit";
            tBlock.Content = "Instrument Type";
            cbBox.IsEnabled = true;
            Editcb.IsChecked = true;
            newTypeTB.Text = "";
            initalizeInstrumentType();
        }

        private void initializeScheduleBtn()
        {
            datePicker.Text = "";
            scheduleIsTrue = true;
            tBlock.Content = "Schedule Types";
            btnEdit.IsEnabled = true;
            Addcb.IsEnabled = true;
            Editcb.IsEnabled = true;
            cbBox.IsEnabled = true;
            newTBlock.Visibility = Visibility.Collapsed;
            newTypeTB.Visibility = Visibility.Collapsed;
            Editcb.Visibility = Visibility.Visible;
            Addcb.Visibility = Visibility.Visible;
            Editcb.IsChecked = true;
            newTypeTB.Text = "";
            initializeScheduleTypes();
        }

        
    }
}