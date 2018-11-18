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

            if (originalType == AutomataType.AFD){
                return automata;
            }

            var newAutomata = DeepCloneAutomata(automata);

            // if it's an NDFAe -> DFA conversion, 
            // we need to convert it to NDFA first
            if (originalType == AutomataType.AFNe) {
                newAutomata = ToNDFA(automata);
            }

            var deterministicTuple = GetDeterministicTuple(newAutomata);

            return CreateNewAutomataFromTuple(
                automata, 
                AutomataType.AFN, 
                deterministicTuple
            );
        }

        public static Automata ToNDFA(Automata automata) {
            var originalType = automata.GetAutomataType();

            if (originalType == AutomataType.AFN) {
                return automata;
            }

            if (originalType == AutomataType.AFD) {
                throw new AutomataConverterException(
                    "Can't convert from DFA to NDFA."
                );
            }

            var epsilonTuple = GetEpsilonTuple(automata);

            return CreateNewAutomataFromTuple(
                automata, 
                AutomataType.AFN, 
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

        //public static void MergeSpontaneousTransitions(Automata automata) {
        //    Transition spontaneousTransition = automata.GetTransitionsWithSymbol(
        //        Automata.SYMBOL_SPONTANEOUS_TRANSITION).FirstOrDefault();

        //    // "from" and "to" states from the spontaneous transition
        //    var oldStates = new List<State> {
        //        spontaneousTransition.From,
        //        spontaneousTransition.To
        //    };

        //    // merging "from" and "to" states
        //    var newState = new GroupedState(oldStates);
        //    State newInitialState = null;

        //    // if any of these states were initial, the merged state becomes initial
        //    if (oldStates.Contains(automata.GetInitialState())) {
        //        newInitialState = newState;
        //    }

        //    var transitionsFromOldStates = new List<Transition>();
        //    var transitionsToOldStates = new List<Transition>();

        //    // get transitions that point to our old "from" state
        //    // and transitions that that state has to other states
        //    if (automata.ContainsState(spontaneousTransition.From)) {
        //        transitionsFromOldStates.AddRange(
        //            automata.GetTransitionsFromState(spontaneousTransition.From)
        //        );
        //        transitionsToOldStates.AddRange(
        //            automata.GetTransitionsToState(spontaneousTransition.From)
        //        );
        //    }
        //    if (automata.ContainsState(spontaneousTransition.To)) {
        //        transitionsFromOldStates.AddRange(
        //            automata.GetTransitionsFromState(spontaneousTransition.To)
        //        );
        //        transitionsToOldStates.AddRange(
        //            automata.GetTransitionsToState(spontaneousTransition.To)
        //        );
        //    }

        //    var transitionsFromNewState = new List<Transition>();
        //    transitionsFromOldStates.ForEach(
        //        y => {
        //            State newTo = oldStates.Contains(y.To) ? newState : y.To;
        //            if (newTo == newState && y.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION) {
        //            } else {
        //                transitionsFromNewState.Add(new Transition(
        //                    newState, y.Input, newTo
        //                ));
        //            }
        //        }
        //    );

        //    var transitionsToNewState = new List<Transition>();
        //    transitionsToOldStates.ForEach(
        //        y => {
        //            State newFrom = oldStates.Contains(y.From) ? newState : y.From;
        //            if (newFrom == newState && y.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION) {
        //            } else {
        //                transitionsToNewState.Add(new Transition(
        //                    newFrom, y.Input, newState
        //                ));
        //            }
        //        }
        //    );

        //    var oldTransitions = new List<Transition>();
        //    oldTransitions.AddRange(transitionsFromOldStates);
        //    oldTransitions.AddRange(transitionsToOldStates);

        //    var newTransitions = new List<Transition>();
        //    newTransitions.AddRange(transitionsFromNewState);
        //    newTransitions.AddRange(transitionsToNewState);

        //    // take new and unique transitions only
        //    newTransitions = newTransitions
        //        .Where(y => !automata.ContainsTransition(y))
        //        .GroupBy(y => y.ToString()).Select(y => y.First())
        //        .ToList();

        //    automata.AddState(newState);
        //    automata.RemoveTransitions(oldTransitions.ToArray());
        //    automata.AddTransitions(newTransitions.ToArray());
        //    automata.RemoveStates(oldStates.ToArray());

        //    if (newInitialState != null) {
        //        automata.SetInitialState(newInitialState);
        //    }

        //    automata.RefreshFinalStates();

        //    // merge again if any spontaneous transition exists
        //    if (automata.GetTransitionsWithSymbol(Automata.SYMBOL_SPONTANEOUS_TRANSITION).Count() > 0) {
        //        MergeSpontaneousTransitions(automata);
        //    }
        //}

        //public static void MakeTransitionsDeterministic(Automata automata, State targetState = null) {
        //    if (targetState == null) {
        //        targetState = automata.GetInitialState();
        //    }
        //    var transitionsGrouping = automata.GetTransitionsFromState(targetState)
        //        .GroupBy(x => x.Input).ToDictionary(t => t.Key, t => t.ToList()); ;

        //    foreach (var key in transitionsGrouping.Keys) {
        //        transitionsGrouping.TryGetValue(key, out var aggregatedTransitions);

        //        if (aggregatedTransitions.Count > 0) {
        //            var oldStates = aggregatedTransitions.Select(x => x.To).ToList();
        //            var newState = new GroupedState(oldStates.ToList());
        //            var newTransitions = new List<Transition>();

        //            aggregatedTransitions.ForEach(
        //                x => {
        //                    newTransitions.Add(new Transition(
        //                        targetState, key, newState
        //                    ));
        //                }
        //            );

        //            var transitionsToOldStates   = new List<Transition>();
        //            var transitionsFromOldStates = new List<Transition>();

        //            oldStates.ForEach(
        //                x => {
        //                    transitionsToOldStates.AddRange(automata.GetTransitionsToState(x));
        //                }
        //            );

        //            oldStates.ForEach(
        //                x => {
        //                    transitionsFromOldStates.AddRange(automata.GetTransitionsFromState(x));
        //                }
        //            );

        //            transitionsToOldStates.ForEach(
        //                x => {
        //                    State fromState = oldStates.Contains(x.From) ? newState : x.From;
        //                    newTransitions.Add(new Transition(
        //                        fromState, x.Input, newState
        //                    ));
        //                }
        //            );

        //            transitionsFromOldStates.ForEach(
        //                x => {
        //                    State toState = oldStates.Contains(x.To) ? newState : x.To;
        //                    newTransitions.Add(new Transition(
        //                        newState, x.Input, toState
        //                    ));
        //                }
        //            );

        //            // take new and unique transitions only
        //            newTransitions = newTransitions.Where(x => !automata.ContainsTransition(x))
        //                .GroupBy(x => x.ToString()).Select(x => x.First()).ToList();

        //            if (!automata.ContainsState(newState)) {
        //                automata.AddState(newState);
        //            }
        //            //automata.RemoveTransitions(aggregatedTransitions.ToArray());
        //            if (newTransitions.Count > 0) {
        //                automata.AddTransitions(newTransitions.ToArray());
        //            }
        //            //automata.RemoveStates(oldStates.ToArray());

        //            MakeTransitionsDeterministic(automata, newState);
        //        }
        //    }
        //}

        public static Tuple<List<GroupedState>, List<Transition>> GetDeterministicTuple(
            Automata automata,
            List<State> statesToSearch = null,
            Tuple<List<GroupedState>, List<Transition>> aggregatedValues = null
        ) {
            // init our tuple
            if (aggregatedValues == null) {
                aggregatedValues = new Tuple<List<GroupedState>, List<Transition>>(
                    new List<GroupedState>(),
                    new List<Transition>()
                );
            }

            var newStates = new List<State>();

            if (statesToSearch == null) {
                statesToSearch = automata.GetStates().ToList();
            }

            statesToSearch.ForEach(
                x => {
                    automata.GetSymbols().ToList().ForEach(
                        s => {
                            
                        }
                    );
                }
            );
            return aggregatedValues;
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
            clonedAutomata.SetSymbols(automata.GetSymbols());
            automata.GetStates().ToList().ForEach(
                x => clonedAutomata.AddState(DeepCloneState(x))
            );
            automata.GetTransitions().ToList().ForEach(
                x => clonedAutomata.AddTransition(DeepCloneTransition(clonedAutomata, x))
            );
            clonedAutomata.SetInitialState(clonedAutomata.GetStateLike(automata.GetInitialState()));
            clonedAutomata.SetAutomataType(automata.GetAutomataType());
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

        //public static void ConvertStatesToDFA(
        //    Automata automata, List<State> states,
        //    List<State>      referencedStates      = null,
        //    List<Transition> referencedTransitions = null) {

        //    if (referencedStates == null) {
        //        referencedStates = new List<State>();
        //    }
        //    if (referencedTransitions == null) {
        //        referencedTransitions = new List<Transition>();
        //    }

        //    List<State> newStates = new List<State>();
        //    foreach (var state in states) {
        //        foreach (var symbol in automata.GetSymbols()) {
        //            if (state is GroupedState) {
        //                var groupedState = (GroupedState) state;

        //                var transitions = new List<Transition>();
        //                foreach (var subState in groupedState.GetSubStates()) {
        //                    transitions.AddRange(automata.GetTransitionsFromState(subState, symbol).ToList());
        //                }

        //                // remove repeated transitions
        //                transitions.RemoveAll(x => x.To is GroupedState);

        //                // take unique states
        //                var subStateList = transitions
        //                    .GroupBy(x => x.To.Name)
        //                    .Select(g => g.First())
        //                    .Select(t => t.To).ToList();

        //                State newState;
        //                if (subStateList.Count > 0) {
        //                    newState = new GroupedState(subStateList);
        //                    if (!automata.ContainsState(newState)) {
        //                        automata.AddState(newState);
        //                        newStates.Add(newState);
        //                    }

        //                    var newTransition = new Transition(
        //                        automata.GetStateLike(state),
        //                        symbol,
        //                        automata.GetStateLike(newState)
        //                    );

        //                    if (!automata.ContainsTransition(newTransition)) {
        //                        automata.AddTransition(newTransition);
        //                        referencedStates.Add(automata.GetStateLike(newTransition.To));
        //                        referencedStates.Add(automata.GetStateLike(newTransition.From));
        //                    }
        //                    referencedTransitions.Add(automata.GetTransitionLike(newTransition));
        //                }
        //            } else {
        //                var transitions = automata.GetTransitionsFromState(state, symbol).ToList();
        //                if (transitions.Count == 0) {
        //                    continue;
        //                }

        //                var toStateList = transitions.Select(t => t.To).ToList();

        //                State newState;
        //                if (transitions.Count > 1) {
        //                    newState = new GroupedState(toStateList);

        //                    if (!automata.ContainsState(newState)) {
        //                        newStates.Add(newState);
        //                        automata.AddState(newState);
        //                    }
        //                } else {
        //                    newState = transitions.First().To;
        //                }

        //                var newTransition = new Transition(
        //                    automata.GetStateLike(state),
        //                    symbol,
        //                    automata.GetStateLike(newState)
        //                );
        //                if (!automata.ContainsTransition(newTransition)) {
        //                    automata.AddTransition(newTransition);
        //                    referencedStates.Add(automata.GetStateLike(newTransition.To));
        //                    referencedStates.Add(automata.GetStateLike(newTransition.From));
        //                    referencedTransitions.Add(automata.GetTransitionLike(newTransition));
        //                } else {
        //                    if (referencedStates.Contains(newTransition.From) &&
        //                        referencedStates.Contains(newTransition.To)) {
        //                        referencedTransitions.Add(automata.GetTransitionLike(newTransition));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if(newStates.Count != 0){
        //        ConvertStatesToDFA(automata, newStates, referencedStates, referencedTransitions);
        //    } else {

        //        // take unique transitions
        //        referencedTransitions = referencedTransitions
        //            .GroupBy(x => x.ToString())
        //            .Select(g => g.First())
        //            .ToList();

        //        referencedStates = referencedTransitions.Select(x => x.From).ToList();
        //        referencedStates.AddRange(referencedTransitions.Select(x => x.To));
        //        referencedStates = referencedStates
        //            .GroupBy(x => x.Name)
        //            .Select(g => g.First())
        //            .ToList();

        //        if (referencedStates.Count > 0 && referencedTransitions.Count > 0) {
        //            automata.SetStates(referencedStates.ToArray());
        //            automata.SetTransitions(referencedTransitions.ToArray());
        //        }
        //    }
        //    return;
        //}
    }
}
