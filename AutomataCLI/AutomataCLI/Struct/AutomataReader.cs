using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataCLI;

namespace AutomataCLI.Struct {
    public class AutomataReader {
	    private Automata Automata {get; set; }

	    // private AutomataWorker[] Workers;

        public AutomataReader(Automata automata){
            this.Automata = automata;
        }
        
        public Boolean Matches(String input){
            
            Transition initialTransition = Automata.GetTransitions().ToList().Find(
                x => (
                    x.From  == this.Automata.GetInitialState() && (
                        x.Input == input[0].ToString() ||
                        x.Input == null
                    )
                )
            );
            if(initialTransition == null){
                return false;
            }

            State firstWorkerState = initialTransition.From;
             
            try{
                return new AutomataWorker(this.Automata, firstWorkerState, input.ToCharArray().ToList()).Work().Result;
            } catch(AggregateException e){
                throw e.Flatten();
            }
        }
    }
}