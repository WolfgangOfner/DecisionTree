using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DecisionTree
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth - 10;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome to the decison tree calculator");
            Console.WriteLine("---------------------------------------");
            Console.ResetColor();

            do
            {
                var data = new DataTable();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 - Import data from csv file");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("2 - Enter data manually");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("3 - End program");
                Console.ResetColor();
                var input = ReadLineTrimmed();

                switch (input)
                {
                    // data will be imported from csv file
                    case "1":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nEnter the path to the .csv file which you want to import");
                        Console.ResetColor();
                        input = ReadLineTrimmed();

                        data = CsvFileHandler.ImportFromCsvFile(input);

                        if (data == null)
                        {
                            DisplayErrorMessage(
                                "An error occured while importing the data from the .csv file. Press any key to close the program.");
                            Console.ReadKey();
                            EndProgram();
                        }
                        else
                        {
                            CreateTreeAndHandleUserOperation(data);
                        }

                        break;

                    // user enters data by hand
                    case "2":
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\n1 - Enter data");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("2 - Create decision tree");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("3 - Export to csv file");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("4 - End program");
                            Console.ResetColor();
                            input = ReadLineTrimmed();

                            switch (input)
                            {
                                // user enters data by hand
                                case "1":
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("\nHow many columns do you want to enter?");
                                    Console.ResetColor();
                                    input = ReadLineTrimmed();

                                    int amountOfColumns;
                                    var rightInput = int.TryParse(input, out amountOfColumns);

                                    if (rightInput && amountOfColumns > 1)
                                    {
                                        data = EnterColumnTitle(amountOfColumns);
                                        data = EnterRowValues(data);

                                        CreateTreeAndHandleUserOperation(data);
                                    }
                                    else
                                    {
                                        DisplayErrorMessage(
                                            "Wrong input. Amount of columns must be an integer greater than 1");
                                    }

                                    break;

                                // after data input decision tree can be created
                                case "2":
                                    if (data?.Rows.Count > 0)
                                    {
                                        CreateTreeAndHandleUserOperation(data);
                                    }
                                    else
                                    {
                                        DisplayErrorMessage("You don't have data entered yet");
                                    }
                                    break;

                                // after data input the data can be exported into a csv file
                                case "3":
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("\nEnter path for your csv file");
                                    Console.ResetColor();
                                    input = ReadLineTrimmed();

                                    var endOfDirectoryPath = input.LastIndexOf("\\");

                                    if (endOfDirectoryPath > 0 &&
                                        Directory.Exists(input.Substring(0, endOfDirectoryPath)))
                                    {
                                        try
                                        {
                                            CsvFileHandler.ExportToCsvFile(data, input);
                                        }
                                        catch (Exception ex)
                                        {
                                            DisplayErrorMessage(
                                                $"An error during the export occured. Error message: {ex.Message}");
                                        }
                                    }
                                    else
                                    {
                                        DisplayErrorMessage("Directory not found");
                                    }
                                    break;

                                case "4":
                                    EndProgram();
                                    break;

                                default:
                                    DisplayErrorMessage("Wrong input");
                                    break;
                            }
                        } while (true);

                    case "3":
                        EndProgram();
                        break;

                    default:
                        DisplayErrorMessage("Wrong input");
                        break;
                }
            } while (true);
        }

        private static string ReadLineTrimmed()
        {
            return Console.ReadLine().TrimStart().TrimEnd();
        }

        private static void CreateTreeAndHandleUserOperation(DataTable data)
        {
            var decisionTree = new Tree();
            decisionTree.Root = Tree.Learn(data, "");
            var returnToMainMenu = false;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDecision tree created");
            Console.ResetColor();

            do
            {
                var valuesForQuery = new Dictionary<string, string>();

                // loop for data input for the query and some special commands
                for (var i = 0; i < data.Columns.Count - 1; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(
                        $"\nEnter your value for {data.Columns[i]} or help for a list of the additional instructions");
                    Console.ResetColor();
                    var input = ReadLineTrimmed();

                    if (input.ToUpper().Equals("ENDPROGRAM"))
                    {
                        EndProgram();
                    }
                    else if (input.ToUpper().Equals("PRINT"))
                    {
                        Console.WriteLine();
                        Tree.Print(decisionTree.Root, decisionTree.Root.Name.ToUpper());
                        Tree.PrintLegend(
                            "Due to the limitation of the console the tree is displayed as a list of every possible route. The colors indicate the following values:");

                        i--;
                    }
                    else if (input.ToUpper().Equals("MAINMENU"))
                    {
                        returnToMainMenu = true;
                        Console.WriteLine();

                        break;
                    }
                    else if (input.ToUpper().Equals("HELP"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nExportData to export your tree");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Print to print the tree");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("EndProgram to end the program");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("MainMenu to return to the main menu");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        i--;
                    }
                    else if (input.ToUpper().Equals("EXPORTDATA"))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nEnter path for your csv file");
                        Console.ResetColor();
                        input = ReadLineTrimmed();

                        var endOfDirectoryPath = input.LastIndexOf("\\");

                        if (endOfDirectoryPath > 0 && Directory.Exists(input.Substring(0, endOfDirectoryPath)))
                        {
                            try
                            {
                                CsvFileHandler.ExportToCsvFile(data, input);
                            }
                            catch (Exception ex)
                            {
                                DisplayErrorMessage(
                                    $"An error during the export occured. \nError message: {ex.Message}");
                            }
                        }
                        else
                        {
                            DisplayErrorMessage("Directory not found");
                        }

                        i--;
                    }
                    else if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                    {
                        DisplayErrorMessage("The attribute can't be empty or a white space");
                        i--;
                    }
                    else
                    {
                        valuesForQuery.Add(data.Columns[i].ToString(), input);
                    }
                }

                // if input was not to return to the main menu, the query will be processed
                if (!returnToMainMenu)
                {
                    var result = Tree.CalculateResult(decisionTree.Root, valuesForQuery, "");

                    Console.WriteLine();

                    if (result.Contains("Attribute not found"))
                    {
                        DisplayErrorMessage("Can't caluclate outcome. Na valid route through the tree was found");
                    }
                    else
                    {
                        Tree.Print(null, result);
                        Tree.PrintLegend("The colors indicate the following values:");
                    }
                }
            } while (!returnToMainMenu);
        }


        private static DataTable EnterRowValues(DataTable data)
        {
            var userEnteringMoreData = true;

            do
            {
                string input;
                var enteredValues = new string[data.Columns.Count];
                var userEnteringMoreDataOrStopInput = true;

                for (var i = 0; i < data.Columns.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\nEnter value for column {data.Columns[i]}");
                    Console.ResetColor();
                    input = ReadLineTrimmed();

                    // user can't enter an empty value
                    if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                    {
                        DisplayErrorMessage("You must enter a value");
                        i--;
                    }
                    else
                    {
                        enteredValues[i] = input;
                    }
                }

                data.Rows.Add(enteredValues);

                do
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n1 - Enter more data");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("2 - Stop data input");
                    Console.ResetColor();
                    input = ReadLineTrimmed();

                    switch (input)
                    {
                        case "1":
                            userEnteringMoreDataOrStopInput = false;
                            break;

                        case "2":
                            userEnteringMoreData = false;
                            userEnteringMoreDataOrStopInput = false;
                            break;

                        default:
                            DisplayErrorMessage("Wrong input");
                            break;
                    }
                } while (userEnteringMoreDataOrStopInput);
            } while (userEnteringMoreData);

            return data;
        }

        private static DataTable EnterColumnTitle(int amountOfColumns)
        {
            var data = new DataTable();

            for (var i = 0; i < amountOfColumns; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nEnter title for column {i}");
                Console.ResetColor();
                var input = ReadLineTrimmed();

                // user can't enter an empty title
                if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                {
                    DisplayErrorMessage("You must enter a title");
                    i--;
                }
                else
                {
                    try
                    {
                        data.Columns.Add(input);
                    }
                    catch (Exception ex)
                    {
                        DisplayErrorMessage($"An error occured. Error message: {ex.Message}");
                        i--;
                    }
                }
            }

            return data;
        }

        private static void DisplayErrorMessage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{errorMessage}\n");
            Console.ResetColor();
        }

        private static void EndProgram()
        {
            Environment.Exit(0);
        }
    }
}