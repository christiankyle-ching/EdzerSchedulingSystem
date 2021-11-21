using EdzerSchedulingSystem.Models;
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
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Animation objects
        private Storyboard introAnimation;
        private DoubleAnimationUsingKeyFrames logoWidthAnimation, logoHeightAnimation, mainBorderOpacityAnimation;
        private ThicknessAnimationUsingKeyFrames logoMarginAnimation, mainBorderPaddingAnimation;
        private readonly QuadraticEase ease;
        #endregion

        public LoginWindow()
        {
            InitializeComponent();

            #region Animations
            //Initialize introAnimation
            introAnimation = new Storyboard();
            introAnimation.BeginTime = new TimeSpan(0, 0, 1); //loading time
            ease = new QuadraticEase();
            ease.EasingMode = EasingMode.EaseOut;

            logoWidthAnimation = new DoubleAnimationUsingKeyFrames();
            logoWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(300));
            logoWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(150, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2)), ease));
            Storyboard.SetTargetName(logoWidthAnimation, imgLogo.Name);
            Storyboard.SetTargetProperty(logoWidthAnimation, new PropertyPath(Image.WidthProperty));
            introAnimation.Children.Add(logoWidthAnimation);

            logoHeightAnimation = new DoubleAnimationUsingKeyFrames();
            logoHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(300));
            logoHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(150, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2)), ease));
            Storyboard.SetTargetName(logoHeightAnimation, imgLogo.Name);
            Storyboard.SetTargetProperty(logoHeightAnimation, new PropertyPath(Image.HeightProperty));
            introAnimation.Children.Add(logoHeightAnimation);

            logoMarginAnimation = new ThicknessAnimationUsingKeyFrames();
            logoMarginAnimation.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 0)));
            logoMarginAnimation.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -350, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2)), ease));
            Storyboard.SetTargetName(logoMarginAnimation, imgLogo.Name);
            Storyboard.SetTargetProperty(logoMarginAnimation, new PropertyPath(Image.MarginProperty));
            introAnimation.Children.Add(logoMarginAnimation);

            //elements animation
            mainBorderOpacityAnimation = new DoubleAnimationUsingKeyFrames();
            mainBorderOpacityAnimation.BeginTime = TimeSpan.FromSeconds(1);
            mainBorderOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0.0));
            mainBorderOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), ease));
            Storyboard.SetTargetName(mainBorderOpacityAnimation, mainBorder.Name);
            Storyboard.SetTargetProperty(mainBorderOpacityAnimation, new PropertyPath(Border.OpacityProperty));
            introAnimation.Children.Add(mainBorderOpacityAnimation);

            mainBorderPaddingAnimation = new ThicknessAnimationUsingKeyFrames();
            mainBorderPaddingAnimation.BeginTime = TimeSpan.FromSeconds(1);
            mainBorderPaddingAnimation.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 0)));
            mainBorderPaddingAnimation.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 100, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), ease));
            Storyboard.SetTargetName(mainBorderPaddingAnimation, mainBorder.Name);
            Storyboard.SetTargetProperty(mainBorderPaddingAnimation, new PropertyPath(Border.PaddingProperty));
            introAnimation.Children.Add(mainBorderPaddingAnimation);
            #endregion

            Window_KeyDown(this, null);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "";
            query += "SELECT * FROM tbl_user WHERE Username = @USERNAME AND PasswordHash = SHA1(@PASSWORD) AND `IsDeleted` = 0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@USERNAME", txtUsername.Text);
            dbCommand.Parameters.AddWithValue("@PASSWORD", pwdPassword.Password);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();
                
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        //user found
                        User user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetBoolean(3));

                        MainWindow mainWindow = new MainWindow(user);
                        mainWindow.Show();
                        this.Close();
                    }
                } else
                {
                    MessageBox.Show("Username or password is incorrect.", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtUsername.Text = "";
                    pwdPassword.Password = "";
                    txtUsername.Focus();
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\nSource: " + ex.StackTrace);
            }

            //override purposes only
            //User tmpUser = new User(1, "admin", "admin", true);
            //MainWindow main = new MainWindow(tmpUser);
            //main.Show();
            //this.Close();

        }
        
        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pwdPassword.Focus();
            }
        }

        private void pwdPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(this, null);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (System.Console.CapsLock)
            {
                txtWarning_CapsLock.Visibility = Visibility.Visible;
            } else
            {
                txtWarning_CapsLock.Visibility = Visibility.Hidden;
            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Window_KeyDown(this, null);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void imgBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            } catch (Exception ex)
            {
                return;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            introAnimation.Begin(this);
        }
    }
}
