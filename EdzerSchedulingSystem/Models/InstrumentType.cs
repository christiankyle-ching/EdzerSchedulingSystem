using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdzerSchedulingSystem.Models
{
    public class InstrumentType
    {
        public string instrumentType { get; set; }

        public int scheduleID { get; set; }

        public int totalQuantity { get; set; }

        public int rentedQuantity { get; set; }

        public float pricePerHour { get; set; }

        public float totalPrice { get; set; }
    }
}
