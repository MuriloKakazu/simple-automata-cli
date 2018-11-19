using AutomataCLI.AutomataOperators;
using AutomataCLI.Struct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Commands {
    public sealed class ConvertAutomataCommand : Command {
        public static void Load() {
            var command = new ConvertAutomataCommand() {
                Body = "convert_automata",
                HelpText = "convert_automata <AFN, AFD>"
            };
            Command.Subscribe(command);
        }

        public override void Execute() {
            if (String.IsNullOrWhiteSpace(Supplement)) {
                throw new CommandException(
                    $"Please enter a valid supplement for the command \"{Body}\""
                );
            }

            String[] validSupplements = new String[] {
                "AFN",
                "AFD"
            };

            if (Program.CurrentAutomata != null) {
                try {
                    var conversionType = Supplement.ToUpper();
                    if (validSupplements.Contains(conversionType)) {
                        if (conversionType == "AFN") {
                            Program.CurrentAutomata = AutomataConverter.ToNDFA(Program.CurrentAutomata);
                        } else if (conversionType == "AFD") {
                            Program.CurrentAutomata = AutomataConverter.ToDFA(Program.CurrentAutomata);
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Automata succuesfully converted! Type 'show_automata' to see its inner members.");
                        Console.ResetColor();
                    } else {
                        Program.LogError($"{Supplement} is not a valid conversion type.");
                    }
                } catch (Exception e) {
                    Program.LogError(e.Message);
                }
            } else {
                Program.LogError("Can't convert. Automata not set.");
            }
        }
    }
}
