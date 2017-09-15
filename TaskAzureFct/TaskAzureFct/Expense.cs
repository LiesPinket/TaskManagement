using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WallyLookaLayout.Data
{
    public class Expense
    {
        public Category Category { get; set; }
        public string Location { get; set; }
        public double Amount { get; set; }
        public DateTime DateSpent { get; set; }

        public static List<GroupedExpenses> GetExpenses()
        {
            //TODO: check if the project name and categories.csv path match your project structure!!
            //gets al list of all categories
            List<Category> categories = Category.ReadCategories();
            
            //gets a week list of 7 days (starting today and going back in time)
            List<GroupedExpenses> weeklist = GroupedExpenses.GetWeekList();

            //read the expenses.csv file and add the created Expense objects to the weeklist
            var assembly = typeof(Expense).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("WallyLookaLayout.Assets.expenses.csv");

            using (var reader = new System.IO.StreamReader(stream))
            {
                reader.ReadLine(); //ignore title row
                string line = reader.ReadLine();

                while (line != null)
                {
                    //process line
                    Expense exp = CreateExpense(line, categories);

                    //search for group (date) - add if present
                    var group = weeklist.Where(x => x.ActualDate.Date.Equals(exp.DateSpent.Date)).Select(x => x);
                    if (group.Count() > 0)
                        group.First().Add(exp);

                    line = reader.ReadLine();
                }
            }

            //return the weeklist that was filled with data
            return weeklist;
        }

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
