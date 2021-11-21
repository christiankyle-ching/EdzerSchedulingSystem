using EdzerSchedulingSystem.Models;
using EdzerSchedulingSystem.Views;
using EdzerSchedulingSystem.Views.AccountsView;
using EdzerSchedulingSystem.Views.SchedulesView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //For Live Date and Clock
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public DateTime dateAndTime;

        //For next schedule checker timer
        System.Windows.Threading.DispatcherTimer checkSchedulesTimer = new System.Windows.Threading.DispatcherTimer();

        //Initialize Windows
        public SchedulesView schedulesView;
        public InstrumentsView instrumentsView;
        public AccountsView accountsView;
        public Dashboard dashboard;

        public User currentUser;

        #region Animation Objects
        //Animations StoryBoards
        private Storyboard hideNavBar, showNavBar;
        private DoubleAnimationUsingKeyFrames hideBar, showBar;
        private ThicknessAnimation hideLogo, showLogo;
        private DoubleAnimation hideWelcomeMessage, showWelcomeMessage, shrinkLogo, enlargeLogo;
        #endregion

        public MainWindow(User user)
        {
            InitializeComponent();

            //set currentUser to user arg
            currentUser = user;

            //Initialize Windows
            schedulesView = new SchedulesView(this);
            instrumentsView = new InstrumentsView(this);
            accountsView = new AccountsView(this);
            dashboard = new Dashboard(this);

            //refresh views
            dashboard.refresh();

            //Open Schedules on Startup
            btnDashboard_Click(this, null);
            
            //Initialize timer
            timer.Tick += new EventHandler(Timer_Click);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            Timer_Click(null, new EventArgs());

            checkSchedulesTimer.Tick += new EventHandler(checkSchedulesTimer_Click);
            checkSchedulesTimer.Interval = new TimeSpan(0, 1, 0);
            checkSchedulesTimer.Start();
            checkSchedulesTimer_Click(null, new EventArgs());

            #region Animations
            //Initialize showNavBar
            showNavBar = new Storyboard();
            
            //Initialize hideNavBar
            hideNavBar = new Storyboard();
            hideNavBar.BeginTime = new TimeSpan(0, 0, 0, 0, 500); //delay before hiding navbar
            #endregion

            //set txtWelcomeMessage text to user.username 
            txtWelcomeMessage.Text = $"Welcome, {currentUser.username}";

            //set accounts manager button disabled or enabled based on user admin type
            btnAccounts.IsEnabled = currentUser.isAdmin;
            btnSettings.IsEnabled = currentUser.isAdmin;

            //grey out accounts button if disabled
            if (!btnAccounts.IsEnabled)
            {
                btnAccounts.Opacity = 0.3;
            }

            notifyAtStartup();
        }

        public void notifyAtStartup()
        {
            DateTime currentDateTime = DateTime.Now;

            List<Schedule> nextSchedules = Database.getNextSchedules(DateTime.Now.Subtract(TimeSpan.FromMinutes(20)), 1);

            //check first if nextSchedule is not null
            if (nextSchedules.Count != 0)
            {
                Schedule nextSchedule = nextSchedules[0];

                DateTime dateTime = DateTime.Parse(nextSchedule.scheduleDate + " " + nextSchedule.startTime);

                if (DateTime.Compare(DateTime.Now, dateTime) > 0)
                {
                    string notificationTitle = $"Schedule already started: {nextSchedule.bandName}";
                    string notificationMessage = $"Schedule #{nextSchedule.scheduleID}\n";
                    notificationMessage += $"{nextSchedule.scheduleDate}\t\t{nextSchedule.startTime}\n";
                    notificationMessage += $"Band:\t\t\t{nextSchedule.bandName}\n";
                    notificationMessage += $"Representative:\t\t{nextSchedule.representativeName}\n";

                    SystemCore.showNotification(notificationTitle, notificationMessage);
                }
            }
        }

        #region NavBar Events
        private void NavBar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (showNavBar == null)
                return;

            showBar = new DoubleAnimationUsingKeyFrames();
            showBar.KeyFrames.Add(new EasingDoubleKeyFrame(NavBar.ActualWidth, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            //showBar.KeyFrames.Add(new EasingDoubleKeyFrame(250, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2))));
            showBar.KeyFrames.Add(new EasingDoubleKeyFrame(300, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3))));
            Storyboard.SetTargetName(showBar, NavBar.Name);
            Storyboard.SetTargetProperty(showBar, new PropertyPath(DockPanel.WidthProperty));

            showLogo = new ThicknessAnimation();
            //showLogo.From = new Thickness(0, 25, 0, 45);
            showLogo.From = imgLogo.Margin;
            showLogo.To = new Thickness(0, 0, 0, 20);
            showLogo.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetName(showLogo, imgLogo.Name);
            Storyboard.SetTargetProperty(showLogo, new PropertyPath(Image.MarginProperty));

            showWelcomeMessage = new DoubleAnimation();
            //showWelcomeMessage.From = 0;
            showWelcomeMessage.From = txtWelcomeMessage.Opacity;
            showWelcomeMessage.To = 1;
            showWelcomeMessage.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetName(showWelcomeMessage, txtWelcomeMessage.Name);
            Storyboard.SetTargetProperty(showWelcomeMessage, new PropertyPath(TextBox.OpacityProperty));

            enlargeLogo = new DoubleAnimation();
            //enlargeLogo.From = 50;
            enlargeLogo.From = imgLogo.ActualHeight;
            enlargeLogo.To = 100;
            enlargeLogo.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetName(enlargeLogo, imgLogo.Name);
            Storyboard.SetTargetProperty(enlargeLogo, new PropertyPath(Image.HeightProperty));

            showNavBar.Children.Add(showBar);
            showNavBar.Children.Add(showLogo);
            showNavBar.Children.Add(showWelcomeMessage);
            showNavBar.Children.Add(enlargeLogo);

            showNavBar.Begin(this);
        }

        private void NavBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (hideNavBar == null)
                return;
            
            hideBar = new DoubleAnimationUsingKeyFrames();
            hideBar.KeyFrames.Add(new EasingDoubleKeyFrame(NavBar.ActualWidth, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            hideBar.KeyFrames.Add(new EasingDoubleKeyFrame(60, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3))));
            Storyboard.SetTargetName(hideBar, NavBar.Name);
            Storyboard.SetTargetProperty(hideBar, new PropertyPath(DockPanel.WidthProperty));

            hideLogo = new ThicknessAnimation();
            hideLogo.From = imgLogo.Margin;
            hideLogo.To = new Thickness(0, 25, 0, 45);
            hideLogo.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetName(hideLogo, imgLogo.Name);
            Storyboard.SetTargetProperty(hideLogo, new PropertyPath(Image.MarginProperty));

            hideWelcomeMessage = new DoubleAnimation();
            hideWelcomeMessage.From = txtWelcomeMessage.Opacity;
            hideWelcomeMessage.To = 0;
            hideWelcomeMessage.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetName(hideWelcomeMessage, txtWelcomeMessage.Name);
            Storyboard.SetTargetProperty(hideWelcomeMessage, new PropertyPath(TextBox.OpacityProperty));

            shrinkLogo = new DoubleAnimation();
            shrinkLogo.From = imgLogo.ActualHeight;
            shrinkLogo.To = 50;
            shrinkLogo.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetName(shrinkLogo, imgLogo.Name);
            Storyboard.SetTargetProperty(shrinkLogo, new PropertyPath(Image.HeightProperty));

            hideNavBar.Children.Add(hideBar);
            hideNavBar.Children.Add(hideLogo);
            hideNavBar.Children.Add(hideWelcomeMessage);
            hideNavBar.Children.Add(shrinkLogo);

            hideNavBar.Begin(this);
        }
        #endregion
        
        #region Button Clicks
        public void btnSchedules_Click(object sender, RoutedEventArgs e)
        {
            if (isContinueAllowed())
            {
                DataContext = schedulesView;
                txtTabName.Text = "Schedules";
            } 
        }

        public void btnInstruments_Click(object sender, RoutedEventArgs e)
        {
            if (isContinueAllowed())
            {
                DataContext = instrumentsView;
                txtTabName.Text = "Instruments";
            }
        }

        public void btnAccounts_Click(object sender, RoutedEventArgs e)
        {
            if (isContinueAllowed())
            {
                DataContext = accountsView;
                txtTabName.Text = "Accounts";
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();

            schedulesView.initializeScheduleTypes();
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (isContinueAllowed()) {
                DataContext = dashboard;
                txtTabName.Text = "Dashboard";
            }
        }

        private bool isContinueAllowed()
        {
            //if still editing, ask to cancel first
            if (schedulesView.isEditing == true)
            {
                schedulesView.btnCancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

            //if cancelled, then continue
            if (schedulesView.isEditing == false)
            {
                return true;
            }

            return false;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Timer events
        private void checkSchedulesTimer_Click(object sender, EventArgs e)
        {
            Console.WriteLine("timecheck");
            
            DateTime currentDateTime = DateTime.Now;

            List<Schedule> nextSchedules = Database.getNextSchedules(DateTime.Now, 1);

            //check first if nextSchedule is not null
            if (nextSchedules.Count != 0)
            {
                Schedule nextSchedule = nextSchedules[0];

                DateTime dateTime = DateTime.Parse(nextSchedule.scheduleDate + " " + nextSchedule.startTime);

                string scheduledTime = dateTime.ToString("hh:mm tt"); Console.WriteLine(scheduledTime);
                string fiveMinutesToSchedule = dateTime.Subtract(TimeSpan.FromMinutes(5)).ToString("hh:mm tt"); Console.WriteLine(fiveMinutesToSchedule);

                if (fiveMinutesToSchedule.Equals(currentDateTime.ToString("hh:mm tt")) && dateTime.Date == currentDateTime.Date)
                {
                    //if 5 minutes from now
                    string notificationTitle = $"Starting in 5 minutes: {nextSchedule.bandName}";
                    string notificationMessage = $"Schedule #{nextSchedule.scheduleID}\n";
                    notificationMessage += $"{nextSchedule.scheduleDate}\t\t{nextSchedule.startTime}\n";
                    notificationMessage += $"Band:\t\t\t{nextSchedule.bandName}\n";
                    notificationMessage += $"Representative:\t\t{nextSchedule.representativeName}\n";

                    SystemCore.showNotification(notificationTitle, notificationMessage);
                } else if (scheduledTime.Equals(currentDateTime.ToString("hh:mm tt")) && dateTime.Date == currentDateTime.Date)
                {
                    string notificationTitle = $"Starting now: {nextSchedule.bandName}";
                    string notificationMessage = $"Schedule #{nextSchedule.scheduleID}\n";
                    notificationMessage += $"{nextSchedule.scheduleDate}\t\t{nextSchedule.startTime}\n";
                    notificationMessage += $"Band:\t\t\t{nextSchedule.bandName}\n";
                    notificationMessage += $"Representative:\t\t{nextSchedule.representativeName}\n";

                    SystemCore.showNotification(notificationTitle, notificationMessage);
                }

                dashboard.refresh();
            }
            
        }

        //clock only
        private void Timer_Click(object sender, EventArgs e)
        {
            dateAndTime = DateTime.Now;
            txtTime.Text = dateAndTime.ToString("hh:mm:ss tt");
            txtDate.Text = dateAndTime.ToString("MM/dd/yyyy");

            txtDay.Text = dateAndTime.ToString("ddd").ToUpper();
        }
        #endregion

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
            {
                SystemCore.closeAllWindows(this);
                timer.Stop();
                checkSchedulesTimer.Stop();

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }

}
