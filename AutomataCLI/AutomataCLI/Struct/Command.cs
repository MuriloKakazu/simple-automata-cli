using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Struct {
    public abstract class Command {
        public class CommandException : Exception {
            public CommandException(String message) : base(message) { }
        }

        private static List<Command> AvailableCommands;

        static Command() {
            AvailableCommands = new List<Command>();
        }

        protected static void Subscribe(Command command) {
            if (Exists(command)) {
                throw new CommandException($"Command {command.Body} already exists.");
            } else {
                AvailableCommands.Add(command);
            }
        }

        public static Boolean Exists(Command command) {
            return Exists(command?.Body);
        }

        public static Boolean Exists(String commandBody) {
            return AvailableCommands.Any(x => x.Body == commandBody);
        }

        public static Command Get(String commandBody) {
            return AvailableCommands.Where(
                x => x.Body == commandBody
            ).FirstOrDefault();
        }

        public static Command[] GetAll() {
            return AvailableCommands.ToArray();
        }

        public static void Execute(String commandInput) {
            String[] commandArgs = commandInput.Split(' ');
            if (commandArgs.Length >= 1) {
                String commandBody = commandArgs[0];
                String commandSupplement = "";

                if (commandArgs.Length >= 2) {
                    commandSupplement = commandArgs[1];
                }

                if (Exists(commandBody)) {
                    var command = Get(commandBody);
                    command.SetSupplement(commandSupplement);
                    command.Execute();
                    return;
                }
            }

            throw new CommandException($"Command {commandInput} is invalid.");
        }

        public void SetSupplement(String supplement) {
            this.Supplement = supplement;
        }

        public String Body { get; protected set; }
        public String Supplement { get; protected set; }
        public String HelpText { get; protected set; }

        public virtual void Execute() {

        }
    }
}
