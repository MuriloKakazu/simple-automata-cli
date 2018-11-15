using System;
using AutomataCLI.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataCLI_Tests.StructTests {
    [TestClass]
    public class AutomataTests {
        [TestMethod]
        public void TestCreateValidAutomata_Full() {
            Automata automata = new Automata();

            String[] symbols =  new String[] {
                "a", "b", "c", Automata.SYMBOL_SPONTANEOUS_TRANSITION
            };

            State[] states = new State[] {
                new State("x", false),
                new State("y", false),
                new State("z", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], symbols[0], states[1]),
                new Transition(states[1], symbols[1], states[2]),
                new Transition(states[2], symbols[2], states[0]),
                new Transition(states[0], symbols[0], states[3]),
            };

            String input = "ab";

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);
        }
    }
}
