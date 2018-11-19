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
                    var fileNameWithoutSulfix = Path.GetFileNameWithoutExtension(Supplement).Split('.').First();
                    var reader = new AutomataReader(Program.CurrentAutomata);
                    var results = reader.MatchAll(File.ReadAllLines(Supplement));

                    using (var writer = new StreamWriter(Supplement.Replace($"{fileNameWithoutSulfix}.IN", $"{ fileNameWithoutSulfix }.OUT"))){
                        writer.Write(results);
                    }

                } else {
                    Program.LogError($"File \"{Supplement}\" does not exist.");
                }
            } else {
                Program.LogError("Can't load input. Automata not set.");
            }
        }
    }
}
