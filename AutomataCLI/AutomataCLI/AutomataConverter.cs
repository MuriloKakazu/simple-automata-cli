using System;
using AutomataCLI;
using AutomataCLI.Struct;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Extensions;

namespace AutomataCLI {
    public class AutomataConverter{
        public class AutomataConverterException : Exception {
            const String MESSAGE_BASE = "Invalid conversion. ";
            public AutomataConverterException(String supplement) : base($"{MESSAGE_BASE}{supplement}") { }
        }
        public static Automata ToNFA(Automata convertedAutomata) {
            
            if(convertedAutomata.GetAutomataType() == AutomataType.AFN){
                return convertedAutomata;
            }

            var newAutomata = new Automata();
            newAutomata.AddSymbols(convertedAutomata.GetSymbols());
            newAutomata.AddStates(convertedAutomata.GetStates());
            newAutomata.AddTransitions(convertedAutomata.GetTransitions());

            ConvertStatesToDFA(newAutomata, newAutomata.GetStates().ToList());
            
            return newAutomata;
        }

        public static void ConvertStatesToDFA(Automata automata, List<State> states) {

            List<State> newStates = new List<State>();
            foreach (State state in states) {
                foreach (String symbol in automata.GetSymbols()) {
                    var transitions = automata.GetTransitionsFromState(state, symbol).ToList();

                    if (transitions.Count == 0) continue;

                    State newState;
                    if (transitions.Count > 1) {
                        newState = new GroupedState(transitions.Select(t => t.To).ToList());
                        if(!automata.ContainsState(newState)){
                            newStates.Add(newState);
                            automata.AddState(newState);
                        }
                    } else {
                        newState = transitions.First().To;
                    }
                    Transition newTransition = new Transition(state, symbol, newState);
                    if(!automata.ContainsTransition(newTransition)){
                        automata.AddTransition(newTransition);
                    }
                }
            }

            if(newStates.Count != 0){
                ConvertStatesToDFA(automata, newStates);
            }
            return;
        }
    }
}
