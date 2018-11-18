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

            String firstInput = String.IsNullOrWhiteSpace(input) ? input : input[0].ToString();

            Transition initialTransition = Automata.GetTransitions().ToList().Find(
                x => (
                    x.From  == this.Automata.GetInitialState() && (
                        x.Input == firstInput ||
                        x.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION
                    )
                )
            );

            if(initialTransition == null){
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