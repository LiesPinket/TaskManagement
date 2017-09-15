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
                
        public override string ToString()
        {
            return Category.Name + " (at " + Location + ") - € " + Amount;
        }
    }

   
}
