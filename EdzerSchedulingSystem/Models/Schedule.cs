using EdzerSchedulingSystem.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdzerSchedulingSystem.Models
{
    
    public class Schedule
    {
        public int scheduleID { get; set; }

        public int scheduleTypeID { get; set; }

        public int bandID { get; set; }

        public string bandName { get; set; }
        
        public string scheduleType { get; set; }

        public string representativeName { get; set; }
        
        public string scheduleDate { get; set; }
        
        public string startTime { get; set; }
        
        public int duration { get; set; }
        
        public bool isPaid { get; set; }

        public float studioRate { get; set; }

        public bool hasPenalty { get; set; }
    }
}
