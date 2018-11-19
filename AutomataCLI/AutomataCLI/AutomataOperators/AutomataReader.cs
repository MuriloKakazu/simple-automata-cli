using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataCLI;
using AutomataCLI.Struct;

namespace AutomataCLI.AutomataOperators {
    public class AutomataReader {
	    private Automata Automata {get; set; }

	    // private AutomataWorker[] Workers;

        public AutomataReader(Automata automata){
            this.Automata = automata;
        }

        public Boolean Matches(String input){
            var initialState = this.Automata.GetInitialState();
            var firstInput = String.IsNullOrWhiteSpace(input) ? input : input[0].ToString();

            var initialTransition = Automata.GetTransitions().ToList().Find(
                x => (
                    x.From  == initialState && (
                        x.Input == firstInput ||
                        x.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION
                    )
                )
            );

            if (initialTransition == null) {
                if (firstInput == "" && initialState.IsFinal) {
                    return true;
                }
                return false;
            }

            State firstWorkerState = initialTransition.From;

            try{
                return new AutomataWorker(this.Automata, firstWorkerState,
                    input.Select(x => x.ToString()).ToList()).WorkAsync().Result;
            } catch(StackOverflowException e){
                return false;
            }
        }

        public String MatchAll(String[] input) {
            String results = "";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Program.WriteSeparator();
            input.ToList().ForEach( 
                x => {
                    try {
                        Console.WriteLine($"Input: '{x}'");
                        results += x;
                        if (Matches(x)) {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("ACEITO");
                            results += " ACEITO" + Environment.NewLine;
                        } else {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("REJEITADO");
                            results += " REJEITADO" + Environment.NewLine;
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Program.WriteSeparator();
                    } catch (Exception e) {
                        Program.LogError(e.Message);
                    }
                }
            );
            Console.ResetColor();
            return results;
        }
    }
}