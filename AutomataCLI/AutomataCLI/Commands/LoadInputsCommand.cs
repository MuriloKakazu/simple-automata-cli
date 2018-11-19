using AutomataCLI.AutomataOperators;
using AutomataCLI.Struct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Commands {
    public sealed class LoadInputsCommand : Command {
        public static void Load() {
            var command = new LoadInputsCommand() {
                Body = "load_inputs",
                HelpText = "load_inputs <PATH>"
            };
            Command.Subscribe(command);
        }

        public override void Execute() {
            if (String.IsNullOrWhiteSpace(Supplement)) {
                throw new CommandException(
                    $"Please enter a valid supplement for the command \"{Body}\""
                );
            }

            if (Program.CurrentAutomata != null) {
                if (File.Exists(Supplement)) {
                    var reader = new AutomataReader(Program.CurrentAutomata);
                    reader.MatchAll(File.ReadAllLines(Supplement));
                } else {
                    Program.LogError($"File \"{Supplement}\" does not exist.");
                }
            } else {
                Program.LogError("Can't load input. Automata not set.");
            }
        }
    }
}
