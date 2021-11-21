using EdzerSchedulingSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for ChangelogWindow.xaml
    /// </summary>
    public partial class ChangelogWindow : Window
    {
        public ChangelogWindow()
        {
            InitializeComponent();

            //initialize
            initializeTypes();
            initializeDatePickers();

            chkStart.IsChecked = true;
            chkEnd.IsChecked = true;
        }

        private void initializeTypes()
        {
            cbType.Items.Add("Schedule");
            cbType.Items.Add("Instrument");
        }

        private void initializeDatePickers()
        {
            dpFromDate.SelectedDate = DateTime.Now.AddMonths(-1).Date;
            dpFromDate.DisplayDate =dpFromDate.SelectedDate.Value;

            dpToDate.SelectedDate = DateTime.Now.Date;
            dpToDate.DisplayDate = DateTime.Now.Date;
        }

        private void chkStart_CheckedChanged(object sender, RoutedEventArgs e)
        {
            dpFromDate.IsEnabled = !chkStart.IsChecked.Value;
        }

        private void chkEnd_CheckedChanged(object sender, RoutedEventArgs e)
        {
            dpToDate.IsEnabled = !chkEnd.IsChecked.Value;
        }

        private void validateControls(object sender, RoutedEventArgs e)
        {
            btnGenerate.IsEnabled = loadAllowed();
        }

        private bool loadAllowed()
        {
            bool errorFound = false;

            //check for controls if allowed to load history
            if (cbType.SelectedIndex == -1 || dpFromDate.SelectedDate == null || dpToDate.SelectedDate == null)
            {
                errorFound = true;
            }

            return !errorFound;
        }

        private void loadHistory(string type)
        {
            if (type == "Schedule")
            {
                loadScheduleHistory(chkStart.IsChecked.Value, chkEnd.IsChecked.Value);
            } else if (type == "Instrument")
            {
                loadInstrumentHistory(chkStart.IsChecked.Value, chkEnd.IsChecked.Value);
            }
        }

        private void loadInstrumentHistory(bool fromBeginning, bool toEnd)
        {
            string query = "SELECT tbl_instrumenttype.InstrumentType AS Type, tbl_instrumenttypeprice.PricePerHour, DATE_FORMAT(tbl_instrumenttypeprice.DateEffective, '%M %d, %Y') AS Date FROM tbl_instrumenttype INNER JOIN tbl_instrumenttypeprice ON tbl_instrumenttype.InstrumentType = tbl_instrumenttypeprice.InstrumentType ";
            
            if (fromBeginning == true && toEnd == false)
            {
                query += "WHERE `DateEffective`<=@TO ";
            } else if (fromBeginning == false && toEnd == true)
            {
                query += "WHERE `DateEffective`>=@FROM ";
            } else if (fromBeginning == false && toEnd == false)
            {
                query += "WHERE `DateEffective`>=@FROM AND `DateEffective`<=@TO ";
            }

            query += "ORDER BY `DateEffective` DESC";

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@FROM", dpFromDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
            dbCommand.Parameters.AddWithValue("@TO", dpToDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
            dbCommand.CommandTimeout = 60;

            Console.WriteLine(query);

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(dbCommand);
            DataTable dataTable = new DataTable();
            try
            {
                //open and close connection
                dbConnection.Open();
                
                //fill datatable with dataadapter
                dataAdapter.Fill(dataTable);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            //set items to datatable
            dgReport.DataContext = null;
            dgReport.DataContext = dataTable;
        }

        private void loadScheduleHistory(bool fromBeginning, bool toEnd)
        {
            string query = "SELECT tbl_scheduletype.ScheduleTypeName AS Type, tbl_scheduletypeprice.PricePerHour, DATE_FORMAT(tbl_scheduletypeprice.DateEffective, '%M %d, %Y') AS Date FROM `tbl_scheduletypeprice` INNER JOIN tbl_scheduletype ON tbl_scheduletype.ScheduleTypeID = tbl_scheduletypeprice.ScheduleTypeID ";
            
            if (fromBeginning == true && toEnd == false)
            {
                query += "WHERE `DateEffective`<=@TO ";
            }
            else if (fromBeginning == false && toEnd == true)
            {
                query += "WHERE `DateEffective`>=@FROM ";
            }
            else if (fromBeginning == false && toEnd == false)
            {
                query += "WHERE `DateEffective`>=@FROM AND `DateEffective`<=@TO ";
            }

            query += "ORDER BY `DateEffective` DESC";

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@FROM", dpFromDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
            dbCommand.Parameters.AddWithValue("@TO", dpToDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
            dbCommand.CommandTimeout = 60;
            
            Console.WriteLine(query);

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(dbCommand);
            DataTable dataTable = new DataTable();
            try
            {
                //open and close connection
                dbConnection.Open();

                //fill datatable with dataadapter
                dataAdapter.Fill(dataTable);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            //set items to datatable
            dgReport.DataContext = null;
            dgReport.DataContext = dataTable;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            loadHistory(cbType.SelectedItem.ToString());
        }
    }
}
