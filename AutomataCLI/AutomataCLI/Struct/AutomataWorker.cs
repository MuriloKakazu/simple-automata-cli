using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutomataCLI.Struct {
    public class AutomataWorker {
        private Automata Automata {get; set;}
        private State CurrentState {get; set;}
        private List<String> InputSymbols {get; set;}
        private State LastState {get; set;}
        private AutomataWorker LastWorker {get; set;}

        public AutomataWorker(Automata automata, State currentState, List<String> inputSymbols){
            this.Automata     = automata;
            this.CurrentState = currentState;
            this.InputSymbols = inputSymbols;
        }
        
        async public Task<Boolean> Work(){
            
            var possibleTransitions = new List<Transition>();
            var remainingSymbols    = new List<String>(InputSymbols);

            for(int i = 0; i < InputSymbols.Count; i++){   
                var currentSymbol = InputSymbols[i]; 
                possibleTransitions = this.Automata.GetTransitions().ToList().Where(
                    x => (
                        x.From  == this.CurrentState && (
                            x.Input == currentSymbol.ToString() ||
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

                        if (possibleTransitions[0].Input != null) { 
                            remainingSymbols.RemoveAt(0);
                        }
                        break;
                    default:
                        if (possibleTransitions[0].Input != null) {
                            remainingSymbols.RemoveAt(0);
                        }

                        var cts = new CancellationTokenSource();
                        Boolean[] results = await SummonWorkers(possibleTransitions, remainingSymbols);
                        if(results.Any(x => x)){
                            cts.Cancel();
                            return true;
                        }
                        return false;
                }
            }
            return this.Automata.GetFinalStates().Contains(this.CurrentState);
        }
        public Task<Boolean[]> SummonWorkers(List<Transition> possibleTransitions, List<String> remainingSymbols) {
            
            return Task.WhenAll(possibleTransitions.Select(x => new AutomataWorker(this.Automata, x.To, remainingSymbols).Work()));
        }
    }
}