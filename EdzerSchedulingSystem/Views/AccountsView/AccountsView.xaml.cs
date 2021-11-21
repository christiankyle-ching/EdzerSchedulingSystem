using EdzerSchedulingSystem.Models;
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

namespace EdzerSchedulingSystem.Views.AccountsView
{
    /// <summary>
    /// Interaction logic for AccountsView.xaml
    /// </summary>
    public partial class AccountsView : UserControl
    {
        private MainWindow _parent;

        private User selectedUser = null;

        public AccountsView(object sender)
        {
            InitializeComponent();

            _parent = (MainWindow)sender;
            //refresh data in table
            refresh();

            //trigger selectionchange event to prime enabled/disabled status
            tblAccounts_SelectionChanged(this, null);
        }

        private void btnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow addAccountWindow = new AddAccountWindow();
            addAccountWindow.ShowDialog();

            refresh();
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(selectedUser);
            changePasswordWindow.Show();

            refresh();
        }

        private void btnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null) return;

            if (MessageBox.Show($"Are you sure you want to delete account '{selectedUser.username}'?",
                                "Delete Account",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Exclamation) ==
                                MessageBoxResult.Yes)
            {
                Database.deleteAccount(selectedUser);
            }

            refresh();
        }
        
        private void refresh()
        {
            //clear items first
            tblAccounts.Items.Clear();

            //unselect all selected cells
            tblAccounts.UnselectAll();

            //fetch database
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "";
            query += "SELECT * FROM tbl_user WHERE IsDeleted = 0";
            
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User newUser = new User(reader.GetInt32(0),     //userID
                                                reader.GetString(1),    //username
                                                reader.GetString(2),    //passphrase
                                                reader.GetBoolean(3));  //isadmin

                        tblAccounts.Items.Add(newUser);
                    }
                }
                else
                {
                    Console.WriteLine("No accounts found");
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void tblAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tblAccounts.SelectedIndex > -1 && tblAccounts.Items.Count > 1)
            {
                selectedUser = (User)tblAccounts.SelectedItem;
                btnDeleteAccount.IsEnabled = true;
                btnChangePassword.IsEnabled = true;

                if (selectedUser.isAdmin)
                {
                    int adminCount = 0;
                    foreach (User u in tblAccounts.Items)
                    {
                        if (u.isAdmin) adminCount++;
                    }

                    if (adminCount <= 1)
                    {
                        btnDeleteAccount.IsEnabled = false;
                    }
                }
                
                
            } else
            {
                selectedUser = null;
                btnDeleteAccount.IsEnabled = false;
                btnChangePassword.IsEnabled = false;
            }
        }

        private void tblAccounts_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                tblAccounts.UnselectAll();
            }
        }
    }
}
