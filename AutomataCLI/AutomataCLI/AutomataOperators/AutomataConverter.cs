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

            var newAutomata = new Automata();
            newAutomata.AddSymbols(automata.GetSymbols());
            newAutomata.AddStates(automata.GetStates());
            newAutomata.AddTransitions(automata.GetTransitions());
            newAutomata.SetInitialState(automata.GetInitialState());

            if (originalType == AutomataType.AFNe) {
                newAutomata = ToNDFA(automata);
            }

            MakeTransitionsDeterministic(newAutomata);

            newAutomata.SetAutomataType(AutomataType.AFD);
            newAutomata.RefreshFinalStates();

            return newAutomata;
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

            Automata newAutomata = new Automata();
            newAutomata.AddSymbols(automata.GetSymbols());
            newAutomata.AddStates(automata.GetStates());
            newAutomata.AddTransitions(automata.GetTransitions());
            newAutomata.SetInitialState(automata.GetInitialState());

            if (originalType == AutomataType.AFNe) {
                MergeSpontaneousTransitions(newAutomata);
            }

            newAutomata.SetAutomataType(AutomataType.AFN);

            return newAutomata;
        }

        public static void MergeSpontaneousTransitions(Automata automata) {
            Transition spontaneousTransition = automata.GetTransitionsWithSymbol(
                Automata.SYMBOL_SPONTANEOUS_TRANSITION).FirstOrDefault();

            var oldStates = new List<State> {
                spontaneousTransition.From,
                spontaneousTransition.To
            };

            var newState = new GroupedState(oldStates);
            State newInitialState = null;

            if (oldStates.Contains(automata.GetInitialState())) {
                newInitialState = newState;
            }

            var transitionsFromOldStates = new List<Transition>();
            var transitionsToOldStates = new List<Transition>();

            if (automata.ContainsState(spontaneousTransition.From)) {
                transitionsFromOldStates.AddRange(
                    automata.GetTransitionsFromState(spontaneousTransition.From)
                );
                transitionsToOldStates.AddRange(
                    automata.GetTransitionsToState(spontaneousTransition.From)
                );
            }
            if (automata.ContainsState(spontaneousTransition.To)) {
                transitionsFromOldStates.AddRange(
                    automata.GetTransitionsFromState(spontaneousTransition.To)
                );
                transitionsToOldStates.AddRange(
                    automata.GetTransitionsToState(spontaneousTransition.To)
                );
            }

            var transitionsFromNewState = new List<Transition>();
            transitionsFromOldStates.ForEach(
                y => {
                    State newTo = oldStates.Contains(y.To) ? newState : y.To;
                    if (newTo == newState && y.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION) {
                    } else {
                        transitionsFromNewState.Add(new Transition(
                            newState, y.Input, newTo
                        ));
                    }
                }
            );

            var transitionsToNewState = new List<Transition>();
            transitionsToOldStates.ForEach(
                y => {
                    State newFrom = oldStates.Contains(y.From) ? newState : y.From;
                    if (newFrom == newState && y.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION) {
                    } else {
                        transitionsToNewState.Add(new Transition(
                            newFrom, y.Input, newState
                        ));
                    }
                }
            );

            var oldTransitions = new List<Transition>();
            oldTransitions.AddRange(transitionsFromOldStates);
            oldTransitions.AddRange(transitionsToOldStates);

            var newTransitions = new List<Transition>();
            newTransitions.AddRange(transitionsFromNewState);
            newTransitions.AddRange(transitionsToNewState);

            // take new and unique transitions only
            newTransitions = newTransitions
                .Where(y => !automata.ContainsTransition(y))
                .GroupBy(y => y.ToString()).Select(y => y.First())
                .ToList();

            automata.AddState(newState);
            automata.RemoveTransitions(oldTransitions.ToArray());
            automata.AddTransitions(newTransitions.ToArray());
            automata.RemoveStates(oldStates.ToArray());

            if (newInitialState != null) {
                automata.SetInitialState(newInitialState);
            }

            automata.RefreshFinalStates();

            if (automata.GetTransitionsWithSymbol(Automata.SYMBOL_SPONTANEOUS_TRANSITION).Count() > 0) {
                MergeSpontaneousTransitions(automata);
            }
        }

        public static void MakeTransitionsDeterministic(Automata automata, State targetState = null) {
            if (targetState == null) {
                targetState = automata.GetInitialState();
            }
            var transitionsGrouping = automata.GetTransitionsFromState(targetState)
                .GroupBy(x => x.Input).ToDictionary(t => t.Key, t => t.ToList()); ;
            
            foreach (var key in transitionsGrouping.Keys) {
                transitionsGrouping.TryGetValue(key, out var aggregatedTransitions);

                if (aggregatedTransitions.Count > 0) {
                    var oldStates = aggregatedTransitions.Select(x => x.To).ToList();
                    var newState = new GroupedState(oldStates.ToList());
                    var newTransitions = new List<Transition>();

                    aggregatedTransitions.ForEach(
                        x => {
                            newTransitions.Add(new Transition(
                                targetState, key, newState
                            ));
                        }
                    );

                    var transitionsToOldStates   = new List<Transition>();
                    var transitionsFromOldStates = new List<Transition>();

                    oldStates.ForEach(
                        x => {
                            transitionsToOldStates.AddRange(automata.GetTransitionsToState(x));
                        }
                    );

                    oldStates.ForEach(
                        x => {
                            transitionsFromOldStates.AddRange(automata.GetTransitionsFromState(x));
                        }
                    );

                    transitionsToOldStates.ForEach(
                        x => {
                            State fromState = oldStates.Contains(x.From) ? newState : x.From;
                            newTransitions.Add(new Transition(
                                fromState, x.Input, newState
                            ));
                        }
                    );

                    transitionsFromOldStates.ForEach(
                        x => {
                            State toState = oldStates.Contains(x.To) ? newState : x.To;
                            newTransitions.Add(new Transition(
                                newState, x.Input, toState
                            ));
                        }
                    );

                    // take new and unique transitions only
                    newTransitions = newTransitions.Where(x => !automata.ContainsTransition(x))
                        .GroupBy(x => x.ToString()).Select(x => x.First()).ToList();

                    if (!automata.ContainsState(newState)) {
                        automata.AddState(newState);
                    }
                    //automata.RemoveTransitions(aggregatedTransitions.ToArray());
                    if (newTransitions.Count > 0) {
                        automata.AddTransitions(newTransitions.ToArray());
                    }
                    //automata.RemoveStates(oldStates.ToArray());

                    MakeTransitionsDeterministic(automata, newState);
                }
            }
        }

        public static void ConvertStatesToDFA(
            Automata automata, List<State> states,
            List<State>      referencedStates      = null,
            List<Transition> referencedTransitions = null) {

            if (referencedStates == null) {
                referencedStates = new List<State>();
            }
            if (referencedTransitions == null) {
                referencedTransitions = new List<Transition>();
            }

            List<State> newStates = new List<State>();
            foreach (var state in states) {
                foreach (var symbol in automata.GetSymbols()) {
                    if (state is GroupedState) {
                        var groupedState = (GroupedState) state;

                        var transitions = new List<Transition>();
                        foreach (var subState in groupedState.GetSubStates()) {
                            transitions.AddRange(automata.GetTransitionsFromState(subState, symbol).ToList());
                        }

                        // remove repeated transitions
                        transitions.RemoveAll(x => x.To is GroupedState);

                        // take unique states
                        var subStateList = transitions
                            .GroupBy(x => x.To.Name)
                            .Select(g => g.First())
                            .Select(t => t.To).ToList();

                        State newState;
                        if (subStateList.Count > 0) {
                            newState = new GroupedState(subStateList);
                            if (!automata.ContainsState(newState)) {
                                automata.AddState(newState);
                                newStates.Add(newState);
                            }

                            var newTransition = new Transition(
                                automata.GetStateLike(state),
                                symbol,
                                automata.GetStateLike(newState)
                            );

                            if (!automata.ContainsTransition(newTransition)) {
                                automata.AddTransition(newTransition);
                                referencedStates.Add(automata.GetStateLike(newTransition.To));
                                referencedStates.Add(automata.GetStateLike(newTransition.From));
                            }
                            referencedTransitions.Add(automata.GetTransitionLike(newTransition));
                        }
                    } else {
                        var transitions = automata.GetTransitionsFromState(state, symbol).ToList();
                        if (transitions.Count == 0) {
                            continue;
                        }

                        var toStateList = transitions.Select(t => t.To).ToList();

                        State newState;
                        if (transitions.Count > 1) {
                            newState = new GroupedState(toStateList);

                            if (!automata.ContainsState(newState)) {
                                newStates.Add(newState);
                                automata.AddState(newState);
                            }
                        } else {
                            newState = transitions.First().To;
                        }

                        var newTransition = new Transition(
                            automata.GetStateLike(state),
                            symbol,
                            automata.GetStateLike(newState)
                        );
                        if (!automata.ContainsTransition(newTransition)) {
                            automata.AddTransition(newTransition);
                            referencedStates.Add(automata.GetStateLike(newTransition.To));
                            referencedStates.Add(automata.GetStateLike(newTransition.From));
                            referencedTransitions.Add(automata.GetTransitionLike(newTransition));
                        } else {
                            if (referencedStates.Contains(newTransition.From) &&
                                referencedStates.Contains(newTransition.To)) {
                                referencedTransitions.Add(automata.GetTransitionLike(newTransition));
                            }
                        }
                    }
                }
            }

            if(newStates.Count != 0){
                ConvertStatesToDFA(automata, newStates, referencedStates, referencedTransitions);
            } else {

                // take unique transitions
                referencedTransitions = referencedTransitions
                    .GroupBy(x => x.ToString())
                    .Select(g => g.First())
                    .ToList();

                referencedStates = referencedTransitions.Select(x => x.From).ToList();
                referencedStates.AddRange(referencedTransitions.Select(x => x.To));
                referencedStates = referencedStates
                    .GroupBy(x => x.Name)
                    .Select(g => g.First())
                    .ToList();

                if (referencedStates.Count > 0 && referencedTransitions.Count > 0) {
                    automata.SetStates(referencedStates.ToArray());
                    automata.SetTransitions(referencedTransitions.ToArray());
                }
            }
            return;
        }
    }
}
