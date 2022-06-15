using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MendzerWydatkow_v2.Models
{
    public class ExpenseReport
    {
        
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string AmountSum { get; set; }
    }
}
