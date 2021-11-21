using EdzerSchedulingSystem.Models;
using EdzerSchedulingSystem.Views.SchedulesView;
using MySql.Data.MySqlClient;
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

namespace EdzerSchedulingSystem.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public MainWindow _parent;

        public Dashboard(object sender)
        {
            InitializeComponent();

            //store reference to main window to call methods
            _parent = (MainWindow) sender;
        }

        private void btnInstrumentCount_Click(object sender, RoutedEventArgs e)
        {
            _parent.DataContext = _parent.instrumentsView;
        }

        private void btnScheduleNextMonth_Click(object sender, RoutedEventArgs e)
        {
            _parent.schedulesView.jumpToDate(DateTime.Now.AddMonths(1).Subtract(TimeSpan.FromDays(DateTime.Now.Day)).AddDays(1));
            _parent.DataContext = _parent.schedulesView;
        }

        private void btnScheduleThisMonth_Click(object sender, RoutedEventArgs e)
        {
            _parent.schedulesView.jumpToDate(DateTime.Now.Subtract(TimeSpan.FromDays(DateTime.Now.Day)).AddDays(1));
            _parent.DataContext = _parent.schedulesView;
        }

        private void tbl_NextSchedules_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                tbl_NextSchedules.UnselectAll();
            }
        }

        public void refresh()
        {
            //refresh number of schedule this month
            txtScheduleThisMonth.Text = Database.countScheduleOfMonth(DateTime.Now).ToString();
            
            //refresh number of schedule next month
            txtScheduleNextMonth.Text = Database.countScheduleOfMonth(DateTime.Now.AddMonths(1)).ToString();
            
            //refresh number of available instruments
            txtInstrumentCount.Text = Database.countInstruments().ToString();

            refreshNextSchedules();
            
            //to still show schedule even 20mins after it has started
            //Schedule nextSchedule = Database.getNextScheduleTime();
            //if (nextSchedule.startTime != "")
            //{
            //    DateTime startTime = DateTime.Parse(nextSchedule.startTime);
            //    DateTime endTime = startTime.Add(TimeSpan.FromHours(nextSchedule.duration));
            //    tmpMinutes = endTime.Subtract(DateTime.Now).Minutes;
            //}

            txtSummary.Text = $"Summary ({DateTime.Now.ToString("MMMM")})";
            //txtRevenue.Text = ((Database.getScheduleTotalMonthlyRevenue(DateTime.Now)).ToString()); //+ Database.getInstrumentTotalMonthlyRevenue(DateTime.Now)).ToString("0.00" + " PHP");  
            txtBands.Text = Database.countBandsRented(DateTime.Now).ToString();
            txtInstruments.Text = Database.countInstrumentsRented(DateTime.Now).ToString();
        }

        public void refreshNextSchedules()
        {
            //get 5 next schedules
            tbl_NextSchedules.ItemsSource = null;

            int tmpMinutes = 20;
            tbl_NextSchedules.ItemsSource = Database.getNextSchedules(DateTime.Now.Subtract(TimeSpan.FromMinutes(tmpMinutes)), 5);
        }

        private void tbl_NextSchedules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tbl_NextSchedules.SelectedItem == null) return;

            _parent.schedulesView.jumpToDate(DateTime.Parse(((Schedule)tbl_NextSchedules.SelectedItem).scheduleDate));
            int selectedID = ((Schedule)tbl_NextSchedules.SelectedItem).scheduleID;
            foreach(Schedule s in _parent.schedulesView.listSchedules.Items)
            {
                if (s.scheduleID == selectedID)
                {
                    _parent.schedulesView.listSchedules.SelectedItem = s;
                    break;
                }
            }

            _parent.btnSchedules_Click(this, null);
        }
        
        private void btnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            AddScheduleWindow window = new AddScheduleWindow(_parent.schedulesView, DateTime.Now);
            window.ShowDialog();

            //to refresh list of schedules
            _parent.schedulesView.jumpToDate(DateTime.Now);

            refresh();
        }

        private void tbl_NextSchedules_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //change ui of topmost schedule if it is currently running
            if (tbl_NextSchedules.Items[0] != null)
            {   
                Schedule nextSched = (Schedule)tbl_NextSchedules.Items[0];
                DateTime dateTime = DateTime.Parse(nextSched.scheduleDate + " " + nextSched.startTime);

                if (DateTime.Compare(DateTime.Now, dateTime) > 0)
                {
                    DataGridRow row = (DataGridRow)tbl_NextSchedules.ItemContainerGenerator.ContainerFromIndex(0);
                    row.Background = Brushes.Green;
                    row.Foreground = Brushes.White;
                }
            }
        }

        private void btnAddInstrument_Click(object sender, RoutedEventArgs e)
        {
            AddInstrumentWindow window = new AddInstrumentWindow();
            window.ShowDialog();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow window = new ReportWindow(DateTime.Now);
            window.ShowDialog();
        }
    }

    

}
