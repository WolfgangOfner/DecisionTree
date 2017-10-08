using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DecisionTree
{
    public class MyAttribute
    {
        public MyAttribute(string name, List<string> differentAttributenames)
        {
            Name = name;
            DifferentAttributeNames = differentAttributenames;
        }

        public string Name { get; }

        public List<string> DifferentAttributeNames { get; }

        public double InformationGain { get; set; }

        public static List<string> GetDifferentAttributeNamesOfColumn(DataTable data, int columnIndex)
        {
            var differentAttributes = new List<string>();

            for (var i = 0; i < data.Rows.Count; i++)
            {
                var found = differentAttributes.Any(t => t.ToUpper().Equals(data.Rows[i][columnIndex].ToString().ToUpper()));

                if (!found)
                {
                    differentAttributes.Add(data.Rows[i][columnIndex].ToString());
                }
            }

            return differentAttributes;
        }
    }
}