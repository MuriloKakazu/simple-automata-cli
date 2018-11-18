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

        public static Automata ToDFA(Automata oldAutomata) {

            if(oldAutomata.GetAutomataType() == AutomataType.AFD){
                return oldAutomata;
            }

            var newAutomata = new Automata();
            newAutomata.AddSymbols(oldAutomata.GetSymbols());
            newAutomata.AddStates(oldAutomata.GetStates());
            newAutomata.AddTransitions(oldAutomata.GetTransitions());

            ConvertStatesToDFA(newAutomata, newAutomata.GetStates().ToList());

            newAutomata.SetInitialState(oldAutomata.GetInitialState());
            newAutomata.SetAutomataType(AutomataType.AFD);
            newAutomata.RemoveSymbol(Automata.SYMBOL_SPONTANEOUS_TRANSITION);
            newAutomata.RefreshFinalStates();

            return newAutomata;
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
                //// take unique states
                //referencedStates = referencedStates
                //    .GroupBy(x => x.Name)
                //    .Select(g => g.First())
                //    .ToList();

                // set prior states as final if transition is spontaneous to a final state
                referencedTransitions.ForEach(
                    x => {
                        if (x.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION && x.To.IsFinal) {
                            x.From.SetIsFinal(true);
                        }
                    }
                );

                //var spontaneousFinalStates = referencedTransitions.Where(
                //    x => x.Input == Automata.SYMBOL_SPONTANEOUS_TRANSITION &&
                //         x.To.IsFinal
                //).Select(x => x.To);

                //// remove spontenous final states
                //referencedStates.RemoveAll(
                //    x => spontaneousFinalStates.Contains(x)
                //);

                // take unique, non-spontaneous, transitions
                referencedTransitions = referencedTransitions
                    .Where(x => x.Input != Automata.SYMBOL_SPONTANEOUS_TRANSITION)
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
                } else {
                    automata.GetTransitionsWithSymbol(Automata.SYMBOL_SPONTANEOUS_TRANSITION)
                        .ToList().ForEach(
                            x => {
                                if (x.To.IsFinal) {
                                    x.From.SetIsFinal(true);
                    }});
                    automata.RemoveSymbol(
                        Automata.SYMBOL_SPONTANEOUS_TRANSITION,
                        removeDependencies: true
                    );
                }
            }
            return;
        }
    }
}
