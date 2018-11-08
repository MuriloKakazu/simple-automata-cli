using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataCLI;

namespace AutomataCLI.Struct {
    public class AutomataReader {
        
        public String Input { get; set; }
	    private Automata Automata {get; set; }

	    // private AutomataWorker[] Workers;
        
        public Boolean Matches(){
            
            Transition initialTransition = Automata.Transitions.Find(
                x => (
                    x.From  == this.Automata.InitialState &&
                    x.Input == this.Input[0] 
                )
            );

            State firstWorkerState = initialTransition.From;
            
            /* Workers.add(new AutomataWorker(
                this.Automata,
                firstWorkerState
            )); */
            return false;
        }
    }
}