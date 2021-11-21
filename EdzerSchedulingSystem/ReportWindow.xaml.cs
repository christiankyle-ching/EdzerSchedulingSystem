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

namespace EdzerSchedulingSystem
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        //for specific schedule
        private Schedule selectedSchedule;

        //for specific month
        private int totalSchedules;

        //constructor for single schedule report
        public ReportWindow(int scheduleID, List<InstrumentType> instrumentsRented)
        {
            InitializeComponent();
            
            //get all details needed 
            selectedSchedule = Database.getScheduleDetails(scheduleID);
            float initialInstrumentsTotalCost = 0;
            foreach (InstrumentType it in instrumentsRented)
            {
                initialInstrumentsTotalCost += it.totalPrice;
            }
            float totalInstrumentCost = initialInstrumentsTotalCost * selectedSchedule.duration;
            float total = selectedSchedule.duration * selectedSchedule.studioRate;
            total += totalInstrumentCost;

            //generate text report
            string finalReport = "Edzer Music Studio System\n\n";
            finalReport += "Report:\n\n";
            
            finalReport += $"Transaction ID: \t\t{selectedSchedule.scheduleID}\n\n";

            finalReport += $"Band ID:\t\t{selectedSchedule.bandID}\n";
            finalReport += $"Band:\t\t\t{selectedSchedule.bandName}\n";
            finalReport += $"Representative:\t\t{selectedSchedule.representativeName}\n";
            finalReport += $"Contact Number:\t{Database.getContactNumber(selectedSchedule.representativeName)}\n\n";

            finalReport += $"Schedule Type: \t\t{selectedSchedule.scheduleType}\n";
            finalReport += $"Schedule Rate: \t\t{selectedSchedule.studioRate} PHP\n";
            finalReport += $"Duration: \t\t{selectedSchedule.duration} Hours\n\n";
            
            finalReport += $"Instruments Rented:\n\n";
            foreach(InstrumentType it in instrumentsRented)
            {
                if (it.rentedQuantity > 0)
                {
                    finalReport += $"{it.instrumentType}\t\t{it.rentedQuantity} * {it.pricePerHour.ToString("0.00")} PHP\n";
                    finalReport += $"{it.totalPrice.ToString("0.00")} PHP\n";
                }
            }
            finalReport += $"\n";
            finalReport += $"Instruments Total Cost: \t{initialInstrumentsTotalCost.ToString("0.00")} PHP * {selectedSchedule.duration.ToString("0.00")} Hour = \t{totalInstrumentCost.ToString("0.00")} PHP\n\n";
            finalReport += $"Final Total Cost: \t\t{total.ToString("0.00")} PHP\n\n";

            //line template
            //finalReport += $"\n";
            
            //List<String> final = new List<String>();
            //final.Add(finalReport);

            txtReport.Text = finalReport;
        }

        //constructor for monthly report
        public ReportWindow(DateTime date)
        {
            InitializeComponent();

            totalSchedules = Database.countScheduleOfMonth(date);
            //float totalRevenue = Database.getScheduleTotalMonthlyRevenue(date); REMOVED QUERY
            //totalRevenue += Database.getInstrumentTotalMonthlyRevenue(date); REMOVED QUERY


            string finalReport = "Edzer Music Studio System \n\n";

            finalReport += $"Monthly Report:\n\n";

            finalReport += $"Total number of Transactions this month:\t\t{totalSchedules}\n\n";

            finalReport += $"List of Bands that Rented this month:\n\n";
            foreach (string bandName in Database.getBandListRented(DateTime.Now))
            {
                finalReport += $"\t{bandName}\n";
            }
            finalReport += $"\n\n";

            //finalReport += $"Total Revenue Earned:\t\t\t{totalRevenue.ToString("0.00")} PHP\n\n";

            //line template
            //finalReport += $"\n";

            //List<string> final = new List<String>();
            //final.Add(finalReport);

            txtReport.Text = finalReport;
        }
       
    }
}
