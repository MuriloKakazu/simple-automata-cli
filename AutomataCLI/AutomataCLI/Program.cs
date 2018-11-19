using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataCLI.Struct;
using System.Diagnostics;
using AutomataCLI.Serialization;
using AutomataCLI.AutomataOperators;
using AutomataCLI.Commands;

namespace AutomataCLI {
    class Program {
        public static Automata CurrentAutomata;

        static void Main(String[] args) {
            LoadCommands();
            ShowAvailableCommands();
            while (true) {
                try {
                    Command.Execute(Console.ReadLine());
                } catch (Exception e) {
                    LogError(e.Message);
                }
            }
        }

        static void LoadCommands() {
            LoadAutomataCommand.Load();
            LoadInputsCommand.Load();
            ConvertAutomataCommand.Load();
            ShowAutomataCommand.Load();
        }

        static void ShowAvailableCommands() {
            Console.ForegroundColor = ConsoleColor.Cyan;
            WriteSeparator();
            Console.WriteLine("Available commands:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var command in Command.GetAll()) {
                Console.WriteLine($"{command.HelpText}");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            WriteSeparator();
            Console.ResetColor();
        }

        public static void WriteSeparator() {
            Console.WriteLine("===================================");
        }

        public static void LogError(String message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}