using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WallyLookaLayout.Data
{
    public class Expense
    {
        public Category Category { get; set; }
        public string Location { get; set; }
        public double Amount { get; set; }
        public DateTime DateSpent { get; set; }
        

        private static Expense CreateExpense(string line, List<Category> categories)
        {
            string[] parts = line.Split(';');
            return new Expense()
            {
                Location = parts[0],
                Category = categories.Where(cat => cat.Name.Equals(parts[1])).Select(cat => cat).First(),
                Amount = Convert.ToDouble(parts[2]),
                DateSpent = DateTime.ParseExact(parts[3],"dd/MM/yy", new CultureInfo("nl-BE"))
            };
        }


        public override string ToString()
        {
            return Category.Name + " (at " + Location + ") - € " + Amount;
        }
    }

   
}
