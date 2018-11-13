using System;
using AutomataCLI;
using AutomataCLI.Struct;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AutomataCLI {
    public class AutomataConverter{
        public class AutomataConverterException : Exception
        {
            const String MESSAGE_BASE = "Invalid conversion. ";
            public AutomataConverterException(String supplement) : base($"{MESSAGE_BASE}{supplement}") { }
        }
        public static Automata ToNFA(Automata convertedAutomata){
            
            if(convertedAutomata.GetAutomataType() == AutomataType.AFN){
                return convertedAutomata;
            }
            
            var newAutomata = new Automata();
            newAutomata.AddSymbols(convertedAutomata.GetSymbols());
            newAutomata.AddStates(convertedAutomata.GetStates());

            // var newStates = new List<GroupedState>();
            // foreach (State state in newAutomata.GetStates()){
            //     foreach (String symbol in newAutomata.GetSymbols()) {

            //         var transitions = convertedAutomata.GetTransitionsLike(state, symbol);

            //         switch (transitions.Count) {
            //             case 0: break;
            //             case 1:
            //                 newAutomata.AddTransition(transitions.First());
            //                 break;
            //             default:
            //                 var newGroupedState = new GroupedState(
            //                     transitions.Select(x => x.To).ToList()
            //                 );

            //                 if (!newAutomata.ContainsStateName(newGroupedState.Name))
            //                 {
            //                     newAutomata.AddState(newGroupedState);
            //                     newStates.Add(newGroupedState);
            //                 }

            //                 newAutomata.AddTransition(new Transition(
            //                     state,
            //                     symbol,
            //                     newGroupedState
            //                 ));
            //                 break;
            //         }
            //     }
            // }

            // var hasNewStates = newStates.Count != 0;

            // while(hasNewStates) {
            //     foreach (State state in newStates) {
            //         foreach (String symbol in newAutomata.GetSymbols()) {

            //             var transitions = convertedAutomata.GetTransitionsLike(state, symbol);

            //             switch (transitions.Count)
            //             {
            //                 case 0: break;
            //                 case 1:
            //                     newAutomata.AddTransition(transitions.First());
            //                     break;
            //                 default:
            //                     var newGroupedState = new GroupedState(
            //                         transitions.Select(x => x.To).ToList()
            //                     );

            //                     if (!newAutomata.ContainsStateName(newGroupedState.Name)) {
            //                         newAutomata.AddState(newGroupedState);
            //                         newStates.Add(newGroupedState);
            //                     }

            //                     newAutomata.AddTransition(new Transition(
            //                         state,
            //                         symbol,
            //                         newGroupedState
            //                     ));
            //                     break;
            //             }
            //         }
            //     }
            // }
            
            return new Automata();
        }
        public static Automata ToNFAe(){
            return new Automata();
        }
    }
}