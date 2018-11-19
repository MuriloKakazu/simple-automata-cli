using AutomataCLI.AutomataOperators;
using AutomataCLI.Extensions;
using AutomataCLI.Struct;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Commands {
    public sealed class ShowTransitionsTableCommand : Command {
        public static void Load() {
            var command = new ShowTransitionsTableCommand() {
                Body = "show_transitions_table",
                HelpText = "show_transitions_table"
            };
            Command.Subscribe(command);
        }

        public override void Execute() {
            if (Program.CurrentAutomata != null) {
                try {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    var automataStates = Program.CurrentAutomata.GetStates();
                    var automataSymbols = Program.CurrentAutomata.GetSymbols().ToList();
                    var columnsQuantity = automataSymbols.Count + 1;
                    if(Program.CurrentAutomata.GetAutomataType() == AutomataType.AFNe){
                        columnsQuantity += 1;
                        automataSymbols.Add("@");
                    }
                    var arrValues = new String[automataStates.Length + 1, columnsQuantity];
                    arrValues[0, 0] = " ";

                    for(int i = 0; i < automataSymbols.Count(); i++) {
                        arrValues[0, i + 1] = automataSymbols[i];
                    }

                    for(int i = 0; i < automataStates.Length; i++) {
                        arrValues[i + 1,0] = automataStates[i].Name;
                        for(int j = 1; j < columnsQuantity; j++) {
                            var possibleTransitions = Program.CurrentAutomata.GetTransitionsFromState(automataStates[i],automataSymbols[j - 1]);
                            if(possibleTransitions.Count() == 0){
                                arrValues[i + 1, j] = "";
                            } else if(possibleTransitions.Count() == 1){
                                arrValues[i + 1, j]  = possibleTransitions.First().To.Name;
                            } else {
                                arrValues[i + 1, j] = "{" + String.Join(",", possibleTransitions.Select(x => x.To.Name).ToList()) + "}";
                            }
                        }
                    }
                    var table = arrValues.ToStringTable();

                    var pathsArray = Program.CurrentAutomataFilePath.Split('\\');
                    pathsArray.SetValue(Path.GetFileNameWithoutExtension(Program.CurrentAutomataFilePath) + ".TABLE.txt", pathsArray.Length - 1);
                    using(var writer = new StreamWriter(String.Join("\\", pathsArray))){
                        writer.Write(table);
                    }

                    Console.Write(Environment.NewLine + table);
                    Console.ResetColor();
                } catch (Exception e) {
                    Program.LogError(e.Message);
                }
            } else {
                Program.LogError("Can't show. Automata not set.");
            }
        }
    }

    public static class TableParser
    {
        public static string ToStringTable<T>(
          this IEnumerable<T> values,
          string[] columnHeaders,
          params Func<T, object>[] valueSelectors)
        {
            return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
        }

        public static string ToStringTable<T>(
          this T[] values,
          string[] columnHeaders,
          params Func<T, object>[] valueSelectors)
        {
            Debug.Assert(columnHeaders.Length == valueSelectors.Length);

            var arrValues = new string[values.Length + 1, valueSelectors.Length];

            // Fill headers
            for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                arrValues[0, colIndex] = columnHeaders[colIndex];
            }

            // Fill table rows
            for (int rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    arrValues[rowIndex, colIndex] = valueSelectors[colIndex]
                      .Invoke(values[rowIndex - 1]).ToString();
                }
            }

            return ToStringTable(arrValues);
        }

        public static string ToStringTable(this string[,] arrValues)
        {
            int[] maxColumnsWidth = GetMaxColumnsWidth(arrValues);
            var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

            var sb = new StringBuilder();
            sb.AppendFormat(" |{0}| ", headerSpliter);
            sb.AppendLine();
            for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    // Print cell
                    string cell = arrValues[rowIndex, colIndex];
                    cell = cell.PadRight(maxColumnsWidth[colIndex]);
                    sb.Append(" | ");
                    sb.Append(cell);
                }

                // Print end of line
                sb.Append(" | ");
                sb.AppendLine();
                sb.AppendFormat(" |{0}| ", headerSpliter);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static int[] GetMaxColumnsWidth(string[,] arrValues)
        {
            var maxColumnsWidth = new int[arrValues.GetLength(1)];
            for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    int newLength = arrValues[rowIndex, colIndex].Length;
                    int oldLength = maxColumnsWidth[colIndex];

                    if (newLength > oldLength)
                    {
                        maxColumnsWidth[colIndex] = newLength;
                    }
                }
            }

            return maxColumnsWidth;
        }
    }
}
