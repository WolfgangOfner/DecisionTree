using System;
using System.Data;
using System.IO;
using System.Text;

namespace DecisionTree
{
    public static class CsvFileHandler
    {
        public static DataTable ImportFromCsvFile(string filePath)
        {
            var rows = 0;
            var data = new DataTable();

            try
            {
                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Substring(0, line.Length - 1).Split(';');

                        foreach (var item in values)
                        {
                            if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
                            {
                                throw new Exception("Value can't be empty");
                            }

                            if (rows == 0)
                            {
                                data.Columns.Add(item);
                            }
                        }

                        if (rows > 0)
                        {
                            data.Rows.Add(values);
                        }

                        rows++;

                        if (values.Length != data.Columns.Count)
                        {
                            throw new Exception("Row is shorter or longer than title row");
                        }
                    }
                }

                var differentValuesOfLastColumn =
                    MyAttribute.GetDifferentAttributeNamesOfColumn(data, data.Columns.Count - 1);

                if (differentValuesOfLastColumn.Count > 2)
                {
                    throw new Exception("The last column is the result column and can contain only 2 different values");
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex.Message);
                data = null;
            }

            // if no rows are entered or data == null, return null
            return data?.Rows.Count > 0 ? data : null;
        }

        public static void ExportToCsvFile(DataTable data, string filePath)
        {
            if (data.Columns.Count == 0)
            {
                throw new Exception("Nothing to export");
            }

            var sb = new StringBuilder();

            // add titles to the string builder
            foreach (var item in data.Columns)
            {
                // seperate values with a ;
                sb.AppendFormat($"{item};");
            }

            sb.AppendLine();

            // add every row to the string builder
            for (var i = 0; i < data.Rows.Count; i++)
            {
                for (var j = 0; j < data.Columns.Count; j++)
                {
                    // seperate values with a ;
                    sb.AppendFormat($"{data.Rows[i][j]};");
                }

                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Data sucessfully exported");
            Console.ResetColor();
        }

        private static void DisplayErrorMessage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{errorMessage}\n");
            Console.ResetColor();
        }
    }
}