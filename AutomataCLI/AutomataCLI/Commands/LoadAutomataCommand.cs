using AutomataCLI.Serialization;
using AutomataCLI.Struct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Commands {
    public sealed class LoadAutomataCommand : Command {
        public static void Load() {
            var command = new LoadAutomataCommand() {
                Body = "load_automata",
                HelpText = "load_automata <PATH>"
            };
            Command.Subscribe(command);
        }

        public override void Execute() {
            if (String.IsNullOrWhiteSpace(Supplement)) {
                throw new CommandException(
                    $"Please enter a valid supplement for the command \"{Body}\""
                );
            }

            if (File.Exists(Supplement)) {
                try {
                    Program.CurrentAutomataFilePath = Supplement;
                    Program.CurrentAutomata = AutomataSerializer.Deserialize(File.ReadAllText(Supplement), true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Automata succesfully loaded! Type 'show_automata' to see its inner members.");
                    Console.ResetColor();
                } catch (Exception e) {
                    Program.LogError(e.Message);
                }
            } else {
                Program.LogError($"File \"{Supplement}\" does not exist.");
            }
        }
    }
}
