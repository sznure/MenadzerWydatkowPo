using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MendzerWydatkow_v2.Models
{   
    public class Expense
    {
       
        public int ExpenseId { get; set; }
        //właściwość Name typu string
        [DisplayName("Nazwa:")]
        public string Name { get; set; }
        [DisplayName("Krótki opis:")]
        public string Description { get; set; }
        [DisplayName("Kwota:")]
        public decimal Amount { get; set; }
        [DisplayName("Data:")]
        public DateTime? Date { get; set; }
        [DisplayName("Kategoria:")]
        public string Category { get; set; }

        public string UserId { get; set; }

        public Expense() //konstruktor - nie zawiera zawartości
        {

        }

    }
}
