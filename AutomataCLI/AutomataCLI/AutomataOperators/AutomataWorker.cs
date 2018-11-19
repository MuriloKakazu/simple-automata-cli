using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomataCLI.Struct;

namespace AutomataCLI.AutomataOperators {
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

        async public Task<Boolean> WorkAsync(){

            var possibleTransitions = new List<Transition>();
            var remainingSymbols    = new List<String>(InputSymbols);

            for (int i = 0; i < InputSymbols.Count; i++) {
                var currentSymbol = InputSymbols[i];

                possibleTransitions = this.Automata.GetTransitions().ToList().Where(
                    x => x.From  == this.CurrentState &&
                         x.Input == currentSymbol.ToString()
                ).ToList();

                // make sure spontaneous transitions are the last members in list
                possibleTransitions.AddRange(this.Automata.GetTransitions().ToList().Where(
                    x => x.From  == this.CurrentState && 
                         x.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION &&
                         x.From != x.To
                ));

                var transitionsQuantity = possibleTransitions.Count;
                var spawnExtraWorkers = true;

                switch (transitionsQuantity) {
                    case 0:
                        return false;
                    case 1:
                        this.CurrentState = possibleTransitions[0].To;
                        this.LastState = possibleTransitions[0].From;

                        if (possibleTransitions[0].Input != Automata.SYMBOL_SPONTANEOUS_TRANSITION) {
                            remainingSymbols.RemoveAt(0);
                        }
                        break;
                    default:
                        if (possibleTransitions[0].Input != Automata.SYMBOL_SPONTANEOUS_TRANSITION) {
                            remainingSymbols.RemoveAt(0);
                        }
                        break;
                }

                if (spawnExtraWorkers) {
                    return await RetrieveWorkersResultAsync(possibleTransitions, remainingSymbols);
                }
            }

            if (InputSymbols.Count == 0) {
                possibleTransitions = this.Automata.GetTransitions().ToList().Where(
                    x => (
                        x.From  == this.CurrentState &&
                        x.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION &&
                        x.From != x.To
                    )
                ).ToList();
                if (possibleTransitions.Count > 0) {
                    return await RetrieveWorkersResultAsync(possibleTransitions, remainingSymbols);
                }
            }

            return this.Automata.GetFinalStates().Contains(this.CurrentState);
        }

        public Task<Boolean[]> SummonWorkers(List<Transition> possibleTransitions, List<String> remainingSymbols) {
            try{
                return Task.WhenAll(possibleTransitions.Select(
                    x => new AutomataWorker(this.Automata, x.To, remainingSymbols).WorkAsync())
                );
            } catch(StackOverflowException e){
                return new Task<Boolean[]>(() => getFalseResult());
            }
        }

        public Boolean[] getFalseResult() => new Boolean[]{false};

        public async Task<Boolean> RetrieveWorkersResultAsync(List<Transition> possibleTransitions, List<String> remainingSymbols) {
            var cts = new CancellationTokenSource();
            Boolean[] results;
            try {
                results = await SummonWorkers(possibleTransitions, remainingSymbols);
                if (results.Any(x => x)) {
                    cts.Cancel();
                    return true;
                }
            } catch (Exception e) {
                cts.Cancel();
            }
            return false;
        }
    }
}