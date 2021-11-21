using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdzerSchedulingSystem.Models
{
    public static class SystemCore
    {
        public const string connectionString = "datasource=localhost;port=3306;username=root;password=;database=db_Edzer;";

        public static void closeAllWindows(Object sender)
        {
            foreach (Window w in App.Current.Windows)
            {
                if (w != sender) w.Close();
            }
        }

        public static void closeAllNotifications()
        {
            //close all existing notification window
            foreach (Window w in App.Current.Windows)
            {
                if (w is NotificationWindow) w.Close();
            }
        }

        public static void showNotification(string title, string message)
        {
            closeAllNotifications();

            NotificationWindow notification = new NotificationWindow(title, message);
            notification.Show();
            notification.Topmost = true;
        }
    }
}
