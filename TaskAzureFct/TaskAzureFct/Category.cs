using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WallyLookaLayout.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string ColorHex { get; set; }

        internal static List<Category> ReadCategories()
        {
            List<Category> results = new List<Category>();
            var assembly = typeof(Category).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("WallyLookaLayout.Assets.categories.csv");

            using (var reader = new System.IO.StreamReader(stream))
            {
                reader.ReadLine(); //ignore title row
                string line = reader.ReadLine();

                while (line != null)
                {
                    //process line
                    results.Add(CreateCategory(line));

                    line = reader.ReadLine();
                }
            }

            return results;
        }

        private static Category CreateCategory(string line)
        {
            string[] parts = line.Split(';');
            return new Category()
            {
                Name = parts[0],
                IconUrl = parts[1],
                ColorHex = parts[2]
            };
        }
    }
}
