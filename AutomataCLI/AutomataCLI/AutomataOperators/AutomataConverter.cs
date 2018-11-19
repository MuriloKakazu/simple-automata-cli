using System;
using AutomataCLI;
using AutomataCLI.Struct;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Extensions;

namespace AutomataCLI.AutomataOperators {
    public class AutomataConverter{
        public class AutomataConverterException : Exception {
            const String MESSAGE_BASE = "Invalid conversion. ";
            public AutomataConverterException(String supplement) : base($"{MESSAGE_BASE}{supplement}") { }
        }

        public static Automata ToDFA(Automata automata) {
            var originalType = automata.GetAutomataType();

            if (originalType == AutomataType.AFD) {
                return automata;
            }

            var epsilonTuple = GetEpsilonTuple(automata);

            return CreateNewAutomataFromTuple(
                automata,
                AutomataType.AFD,
                epsilonTuple
            );
        }

        private static Automata CreateNewAutomataFromTuple(
            Automata automata, 
            AutomataType newType,
            Tuple<List<GroupedState>, List<Transition>> aggregationTuple
        ) {

            var epsilonStates      = aggregationTuple.Item1;
            var epsilonTransitions = aggregationTuple.Item2;
            Automata newAutomata = DeepCloneAutomata(automata);

            newAutomata.SetAutomataType(newType);
            newAutomata.SetStates(epsilonStates.ToArray());
            newAutomata.SetTransitions(epsilonTransitions.ToArray());
            newAutomata.SetInitialState(
                epsilonStates.Where(
                    x => x.GetSubStates().ToList().Any(
                        y => y.Name == automata.GetInitialState().Name
                    )
                ).FirstOrDefault()
            );
            newAutomata.RefreshFinalStates();

            return newAutomata;
        }

        public static Tuple<List<GroupedState>, List<Transition>> GetEpsilonTuple(
            Automata automata, 
            State currentState = null, 
            GroupedState currentEpsilon = null,
            Tuple<List<GroupedState>, List<Transition>> aggregatedValues = null
        ) {

            // init our tuple
            if (aggregatedValues == null) {
                aggregatedValues = new Tuple<List<GroupedState>, List<Transition>>(
                    new List<GroupedState>(),
                    new List<Transition>()
                );
            }

            var aggregatedStates      = aggregatedValues.Item1;
            var aggregatedTransitions = aggregatedValues.Item2;

            // defining first state
            if (currentState == null) {
                currentState = automata.GetInitialState();
            }

            // defining the current epsilon
            if (currentEpsilon == null) {
                currentEpsilon = GetEpsilonClosureFromState(
                    automata, currentState
                );
            }

            // aggregate initial epsilon
            if (aggregatedStates == null) {
                aggregatedStates = new List<GroupedState>();
            }

            if (!AggregatedStateListContainsStateLike(aggregatedStates, currentEpsilon)) {
                aggregatedStates.Add(currentEpsilon);

                // find more epsilons and aggregate them recursively
                foreach (var symbol in automata.GetSymbols()) {
                    var epsilonCandidates = new List<State>();

                    // get all states found consuming the symbol
                    // from every subState in our epsilon
                    currentEpsilon.GetSubStates().ToList().ForEach(
                        x => epsilonCandidates.AddRange(
                                automata.GetTransitionsFromState(
                                    x, symbol
                                ).Select(t => t.To)
                            )
                    );

                    if (epsilonCandidates.Count > 0) {
                        var aggregatedCandidates = new GroupedState(epsilonCandidates);
                        var aggregationEpsilon = GetEpsilonClosureFromState(
                            automata, aggregatedCandidates
                        );

                        aggregatedTransitions.Add(new Transition(
                            currentEpsilon, symbol, aggregationEpsilon
                        ));

                        // aggregate new epsilons recursively
                        GetEpsilonTuple(
                            automata, 
                            aggregatedCandidates, 
                            aggregationEpsilon, 
                            aggregatedValues
                        );
                    }
                }
            }

            return aggregatedValues;
        }

        protected static Boolean AggregatedStateListContainsStateLike(List<GroupedState> stateList, State stateLike) {
            return stateList.Any(x => x.Name == stateLike.Name);
        }

        public static GroupedState GetAggregatedEpsilonClosureFromStates(Automata automata, List<State> targetStates) {
            GroupedState aggregatedState = null;
            targetStates.ForEach(
                x => {
                    var epsilon = GetEpsilonClosureFromState(automata, x);

                    if (aggregatedState == null) {
                        aggregatedState = new GroupedState(epsilon.GetSubStates());
                    } else {
                        aggregatedState.AddSubStates(epsilon.GetSubStates());
                    }
                }
            );
            return aggregatedState;
        }

        public static GroupedState GetEpsilonClosureFromState(Automata automata, State targetState, GroupedState aggregatedState = null) {
            if (targetState is GroupedState) {
                return GetAggregatedEpsilonClosureFromStates(
                    automata, ((GroupedState)targetState).GetSubStates().ToList()
                );
            } else {
                // spontaneous transitions from target state
                var transitionsFromState = automata.GetTransitionsFromState(
                    automata.GetStateLike(targetState), Automata.SYMBOL_SPONTANEOUS_TRANSITION
                );

                // spontaneously (directly) reachable states from target state
                var reachableStates = transitionsFromState.Select(x => x.To).ToList();

                // epsilon closure includes current state
                reachableStates.Add(targetState);

                // aggregate spontaneously reachable states to a single state
                if (aggregatedState == null) {
                    aggregatedState = new GroupedState(reachableStates);
                } else {
                    aggregatedState.AddSubStates(reachableStates);
                }

                // aggregate spontaneously reachable 
                // child states to our state recursively
                if (reachableStates.Count > 1) {
                    reachableStates.ForEach(
                        x => {
                            if (x != targetState) {
                                aggregatedState.AddSubState(
                                    GetEpsilonClosureFromState(
                                        automata, x, aggregatedState
                                    )
                                );
                            }
                        }
                    );
                }
                return aggregatedState;
            }
        }

        public static Automata DeepCloneAutomata(Automata automata) {
            Automata clonedAutomata = new Automata();
            clonedAutomata.SetAutomataType(automata.GetAutomataType());
            clonedAutomata.SetSymbols(automata.GetSymbols());
            automata.GetStates().ToList().ForEach(
                x => clonedAutomata.AddState(DeepCloneState(x))
            );
            automata.GetTransitions().ToList().ForEach(
                x => clonedAutomata.AddTransition(DeepCloneTransition(clonedAutomata, x))
            );
            clonedAutomata.SetInitialState(clonedAutomata.GetStateLike(automata.GetInitialState()));
            return clonedAutomata;
        }

        public static State DeepCloneState(State state) {
            State clonedState = null;

            if (state is GroupedState) {
                clonedState = new GroupedState(
                    ((GroupedState) state).GetSubStates()
                );
            } else {
                clonedState = new State(state.Name, state.IsFinal);
            }
            return clonedState;
        }

        public static Transition DeepCloneTransition(Automata automata, Transition transition) {
            return new Transition(
                automata.GetStateLike(transition.From),
                transition.Input,
                automata.GetStateLike(transition.To)
            );
        }
    }
}
