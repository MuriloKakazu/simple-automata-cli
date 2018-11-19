using AutomataCLI.AutomataOperators;
using AutomataCLI.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Commands {
    public sealed class ShowAutomataCommand : Command {
        public static void Load() {
            var command = new ShowAutomataCommand() {
                Body = "show_automata",
                HelpText = "show_automata"
            };
            Command.Subscribe(command);
        }

        public override void Execute() {
            if (Program.CurrentAutomata != null) {
                try {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Program.CurrentAutomata);
                    Console.ResetColor();
                } catch (Exception e) {
                    Program.LogError(e.Message);
                }
            } else {
                Program.LogError("Can't show. Automata not set.");
            }
        }
    }
}
