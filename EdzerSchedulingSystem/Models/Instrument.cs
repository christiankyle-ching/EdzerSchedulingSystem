using EdzerSchedulingSystem.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdzerSchedulingSystem.Models
{
    public class Instrument
    {
        private int _instrumentID;
        public int instrumentID
        {
            get { return this._instrumentID; }
        }

        private string _instrumentModel;
        public string instrumentModel
        {
            get { return this._instrumentModel; }
        }

        private string _instrumentType;
        public string instrumentType
        {
            get { return this._instrumentType; }
        }

        private string _instrumentDescription;
        public string instrumentDescription
        {
            get { return this._instrumentDescription; }
        }


        //Constructor
        public Instrument(int instrumentID, string instrumentModel, string instrumentType, string instrumentDescription)
        {
            _instrumentID = instrumentID;
            _instrumentModel = instrumentModel;
            _instrumentType = instrumentType;
            _instrumentDescription = instrumentDescription;
        }

        
    }
}
