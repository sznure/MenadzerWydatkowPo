using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MendzerWydatkow_v2.Models
{
    public class ExpenseCategory
    {
        public List<Expense> Expenses { get; set; }
        public SelectList Categories { get; set; }
        public string ExpCategory { get; set; }
        public string SearchString { get; set; }
    }
}
