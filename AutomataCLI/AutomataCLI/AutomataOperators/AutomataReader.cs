using System;
using System.Collections.Generic;
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
    }
}