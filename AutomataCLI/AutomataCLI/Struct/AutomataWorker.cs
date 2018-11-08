using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        async private Task<Boolean> Work(){
            
            var possibleTransitions = new List<Transition>();
            var remainingSymbols    = new List<Char>(InputSymbols);

            for(int i = 0; i < InputSymbols.Count; i++){   
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

                switch (transitionsQuantity) {
                    case 0:
                        return false;
                    case 1:
                        this.CurrentState = possibleTransitions[0].To;
                        this.LastState = possibleTransitions[0].From;

                        if (possibleTransitions[0].Input != null)
                        {
                            remainingSymbols.RemoveAt(i);
                        }
                        break;
                    default:
                        Boolean[] results = await summonWorkers(possibleTransitions, remainingSymbols);
                        return results.Any(x => x);
                }
                
                if (transitionsQuantity >= 1) {
                    
                }

            }
            return true;
        }
        public Task<Boolean[]> summonWorkers(List<Transition> possibleTransitions, List<Char> remainingSymbols) {
        
            foreach(Transition transition in possibleTransitions) {
                var newWorker = new AutomataWorker(this.Automata, transition.To, remainingSymbols);
            }
            
            return Task.WhenAll(possibleTransitions.Select(x => new AutomataWorker(this.Automata, x.To, remainingSymbols).Work()));
        }
    }
}