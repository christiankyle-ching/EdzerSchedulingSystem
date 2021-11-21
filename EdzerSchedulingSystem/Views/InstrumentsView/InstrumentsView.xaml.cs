using EdzerSchedulingSystem.Models;
using EdzerSchedulingSystem.ViewModels;
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
    /// Interaction logic for InstrumentsView.xaml
    /// </summary>
    public partial class InstrumentsView : UserControl
    {
        private MainWindow _parent;

        private Instrument selectedInstrument;
        private int selectedInstrumentID;
        
        public InstrumentsView(object sender)
        {
            InitializeComponent();

            //store parent caller
            _parent = (MainWindow)sender;

            //only to set disabled buttons when none is selected
            tblInstruments_SelectionChanged(this, null);

            //refresh/get types
            refreshTypes();

            //refresh/get instruments/get instrument types
            refresh();
            
        }

        private void btnAddInstrument_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddInstrumentWindow();
            window.ShowDialog();

            refreshTypes();
            refresh();
            _parent.dashboard.refresh();
        }

        private void btnEditInstrument_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditInstrumentWindow(selectedInstrument.instrumentID);
            window.ShowDialog();

            refreshTypes();
            refresh();
            _parent.dashboard.refresh();
        }
       
        private void btnDeleteInstrument_Click(object sender, RoutedEventArgs e)
        {
            if ((Instrument)tblInstruments.SelectedItem == null) return;

            selectedInstrumentID = ((Instrument)tblInstruments.SelectedItem).instrumentID;
            if (MessageBox.Show($"Are you sure you want to delete Instrument #{selectedInstrumentID}?",
                                "Delete Schedule",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning)
                                ==
                                MessageBoxResult.Yes
                )
            {
                Database.deleteInstrument(selectedInstrumentID);
            }
            else
            {
                MessageBox.Show("Operation cancelled!", "Delete Schedule", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            refreshTypes();
            refresh();
            _parent.dashboard.refresh();
        }

        private void refresh()
        {
            //refresh the table
            tblInstruments.ItemsSource = null;
            tblInstruments.ItemsSource = Database.getInstrumentsList(txtSearch.Text, cmbType.SelectedItem.ToString());
        }

        private void refreshTypes()
        {
            //refresh intrument types combobox
            //get lastselecteditem first if not first run
            string lastSelectedItem = "";
            if (cmbType.SelectedItem != null)
            {
                lastSelectedItem = cmbType.SelectedItem.ToString();
            } else
            {
                lastSelectedItem = "All";
            }
            
            List<string> tmpList = new List<string>();
            tmpList.Add("All");
            foreach (InstrumentType it in Database.getAllInstrumentTypes())
            {
                tmpList.Add(it.instrumentType);
            }

            //replace itemssource
            cmbType.ItemsSource = null;
            cmbType.ItemsSource = tmpList;

            //set last selected item 
            cmbType.SelectedItem = lastSelectedItem;
        }

        private void tblInstruments_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                tblInstruments.UnselectAll();
            }
        }

        private void tblInstruments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tblInstruments.SelectedIndex > -1 && tblInstruments.Items.Count > 0)
            {
                selectedInstrument = (Instrument) tblInstruments.SelectedItem;
                btnEditInstrument.IsEnabled = true;
                btnDeleteInstrument.IsEnabled = true;
            }
            else
            {
                selectedInstrument = null;
                btnEditInstrument.IsEnabled = false;
                btnDeleteInstrument.IsEnabled = false;
            }

            if (_parent == null) return;

            //if user is not admin, disable anyway
            if (_parent.currentUser.isAdmin == false)
            {
                btnAddInstrument.IsEnabled = false;
                btnEditInstrument.IsEnabled = false;
                btnDeleteInstrument.IsEnabled = false;
            }
        }
        
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            //if enter is pressed, go search
            if (e.Key == Key.Enter)
            {
                refresh();
            }
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbType.ItemsSource == null) return;
            refresh();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //refresh if empty
            if (txtSearch.Text == "")
            {
                refresh();
            }
        }
    }
}
