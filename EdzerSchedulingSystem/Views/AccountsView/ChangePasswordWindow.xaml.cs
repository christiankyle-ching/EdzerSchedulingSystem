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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EdzerSchedulingSystem.Views.AccountsView
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private User _oldUser;
        
        public ChangePasswordWindow(User oldUser)
        {
            InitializeComponent();

            //set param to field variable
            _oldUser = oldUser;

            txtUserID.Text = _oldUser.userID.ToString();
            txtUsername.Text = _oldUser.username;
            chkIsAdmin.IsChecked = _oldUser.isAdmin;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (validateData())
            {
                User updatedUser = new User(_oldUser.userID, txtUsername.Text, pwdPassword.Password, chkIsAdmin.IsChecked.Value);
                Database.updateAccount(updatedUser);

                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void validatePassword(object sender, RoutedEventArgs e)
        {
            if (!(pwdPassword.Password.Equals(pwdConfirmPassword.Password)))
            {
                pwdConfirmPassword.Foreground = (SolidColorBrush)FindResource("ErrorColor");
            }
            else
            {
                pwdConfirmPassword.Foreground = (SolidColorBrush)FindResource("SecondaryTextColor");
            }
        }

        private bool validateData()
        {
            string username = txtUsername.Text;
            string password = pwdPassword.Password;
            string confirmPassword = pwdConfirmPassword.Password;

            string errorMessage = "";
            bool errorFound = false;
            
            //check username if empty
            if (username.Equals("") || username.Contains(" "))
            {
                errorMessage += "\n\nUsername cannot be empty or contain any spaces.";
                errorFound = true;
            }

            //check username length
            if (txtUsername.Text.Length < 8)
            {
                errorMessage += "\n\nUsername minimum characters is 8.";
                errorFound = true;
            }

            //check password if equal to confirm password
            if (!password.Equals(confirmPassword))
            {
                errorMessage += "\n\nPassword didn't match.";
                errorFound = true;
            }

            //check password length
            if (pwdPassword.Password.Length < 8)
            {
                errorMessage += "\n\nPassword minimum characters is 8.";
                errorFound = true;
            }

            errorMessage = errorMessage.Trim();
            if (errorFound) MessageBox.Show(errorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Information);

            return !errorFound;

        }
    }
}
