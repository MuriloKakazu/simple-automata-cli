using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomataCLI.Struct {
    public class AutomataWorker {
        private Automata Automata {get; set;}
        private State CurrentState {get; set;}
        private List<Char> InputSymbols {get; set;}
        private State LastState {get; set;}
        private AutomataWorker LastWorker {get; set;}

        public AutomataWorker(Automata automata, State currentState, List<Char> inputSymbols){
            this.Automata     = automata;
            this.CurrentState = currentState;
            this.InputSymbols = inputSymbols;
        }

        private Boolean Work(){
            
            var possibleTransitions = new List<Transition>();
            var remainingSymbols    = new List<Char>(InputSymbols);

            for(int i = 0; i < InputSymbols.Count; i++){
                {    
                    var currentSymbol = InputSymbols[i]; 
                    possibleTransitions = this.Automata.Transitions.Where(
                        x => (
                            x.From  == this.CurrentState && (
                                x.Input == currentSymbol ||
                                x.Input == null
                            )
                        )
                    ).ToList();

                    var transitionsQuantity = possibleTransitions.Count; 

                    if(transitionsQuantity == 0) {
                        return false;
                    }
                    
                    this.CurrentState = possibleTransitions[0].To;
                    this.LastState    = possibleTransitions[0].From;
                    
                    if(possibleTransitions[0].Input != null){
                        remainingSymbols.RemoveAt(i);
                    }

                    if(transitionsQuantity >= 1){
                        summonWorkers(possibleTransitions.GetRange(1, transitionsQuantity - 1), remainingSymbols);
                    }
                }
            }
            return true;
        }
        public void summonWorkers(List<Transition> possibleTransitions, List<Char> remainingSymbols) {
        
            foreach(Transition transition in possibleTransitions) {
                var newWorker = new AutomataWorker(this.Automata, transition.To, remainingSymbols);
                Boolean result =  newWorker.Work();
            }
        }
    }
}