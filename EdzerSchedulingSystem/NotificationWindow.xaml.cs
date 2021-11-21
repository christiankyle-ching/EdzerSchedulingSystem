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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        
        public NotificationWindow(string title, string message)
        {
            InitializeComponent();

            txtTitle.Text = title;
            txtMessage.Text = message;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void notificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Set location to bottom right
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - (this.Width + 20);
            this.Top = desktopWorkingArea.Bottom - (this.Height + 20);

            //Window Animation
            var popUpAnimation = new Storyboard();

            var easeIn = new QuadraticEase();
            easeIn.EasingMode = EasingMode.EaseIn;
            var easeOut = new QuadraticEase();
            easeIn.EasingMode = EasingMode.EaseOut;

            var windowShow = new DoubleAnimation();
            windowShow.From = 0.0;
            windowShow.To = 1.0;
            windowShow.EasingFunction = easeIn;
            windowShow.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(windowShow, notificationWindow.Name);
            Storyboard.SetTargetProperty(windowShow, new PropertyPath(Window.OpacityProperty));
            popUpAnimation.Children.Add(windowShow);


            var toLeftAnimation = new DoubleAnimation();
            toLeftAnimation.From = desktopWorkingArea.Right + (this.Width * 2);
            toLeftAnimation.To = desktopWorkingArea.Right - (this.Width + 20);
            toLeftAnimation.EasingFunction = easeIn;
            toLeftAnimation.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(toLeftAnimation, notificationWindow.Name);
            Storyboard.SetTargetProperty(toLeftAnimation, new PropertyPath(Window.LeftProperty));
            popUpAnimation.Children.Add(toLeftAnimation);
            

            //Begin Animation
            popUpAnimation.Begin(this);

            //Play Sound
            System.Media.SystemSounds.Beep.Play();

            //Get Focus
            this.Focus();
        }
    
    }
}
