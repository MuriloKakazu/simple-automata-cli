using System;
using System.Collections.Generic;
using AutomataCLI.AutomataOperators;
using AutomataCLI.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataCLI_Tests.StructTests {
    [TestClass]
    public class AutomataReaderTests {
        [TestMethod]
        public void TestMatchAutomata0() {
            /*
             *      L = {0,1}*{00} (AFD)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "1", states[0]),
                new Transition(states[0], "0", states[1]),
                new Transition(states[1], "0", states[2]),
                new Transition(states[1], "1", states[0]),
                new Transition(states[2], "1", states[0]),
                new Transition(states[2], "0", states[2]),
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFD);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "101001011011011011010101010000011111111110000000000001100",
                "101010100001111111010101000000000000000000000000000001100",
                "000000000000000000000000000000000000000000000000000000000",
                "111111111111111111111111110011111111111111111111111100000",
                "100101010101010101010101001101010101011111111111000011100",
                "111111111111111111111111111111110000000000000000000000000"
            };

            String[] invalidInputs = new String[] {
                "111111111111111111111111111120000000000000000000000000000",
                "222222222220000000000000000000000000000000000000000000000",
                "111111111111111111111111111111111111111111111111111111110",
                "000000000000000000000000000000000000000000000000000000001",
                "111111111111111111111111111111111111111111111111111111111",
                "111111111111111111111111111111111111111111111111111111001"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x));
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x));
            }
        }

        [TestMethod]
        public void TestMatchAutomata1() {
            /*
             *      L = {0,1}*{00} (AFN)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "1", states[0]),
                new Transition(states[0], "0", states[0]),
                new Transition(states[0], "0", states[1]),
                new Transition(states[1], "0", states[2])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFN);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "101001011011011011010101010000011111111110000000000001100",
                "101010100001111111010101000000000000000000000000000001100",
                "000000000000000000000000000000000000000000000000000000000",
                "111111111111111111111111110011111111111111111111111100000",
                "100101010101010101010101001101010101011111111111000011100",
                "111111111111111111111111111111110000000000000000000000000"
            };

            String[] invalidInputs = new String[] {
                "111111111111111111111111111120000000000000000000000000000",
                "222222222220000000000000000000000000000000000000000000000",
                "111111111111111111111111111111111111111111111111111111110",
                "000000000000000000000000000000000000000000000000000000001",
                "111111111111111111111111111111111111111111111111111111111",
                "111111111111111111111111111111111111111111111111111111001"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x));
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x));
            }
        }

        [TestMethod]
        public void TestMatchAutomata2() {
            /*
             *      L = {0,1}*{1}{0,1}{0,1} (AFD)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", false),
                new State("q3", true),
                new State("q4", true),
                new State("q5", false),
                new State("q6", true),
                new State("q7", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "0", states[0]),
                new Transition(states[0], "1", states[1]),
                new Transition(states[1], "0", states[5]),
                new Transition(states[1], "1", states[2]),
                new Transition(states[2], "0", states[7]),
                new Transition(states[2], "1", states[3]),
                new Transition(states[3], "0", states[7]),
                new Transition(states[3], "1", states[3]),
                new Transition(states[4], "0", states[0]),
                new Transition(states[4], "1", states[1]),
                new Transition(states[5], "0", states[4]),
                new Transition(states[5], "1", states[6]),
                new Transition(states[6], "0", states[5]),
                new Transition(states[6], "1", states[2]),
                new Transition(states[7], "0", states[4]),
                new Transition(states[7], "1", states[6])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFD);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "1011010101010010101010101100",
                "1010101101101011010101010101",
                "1111111111111111111111111110",
                "1111111111111111111111111111",
                "0000000000011000000000000111",
                "1111111111111111111111110100"
            };

            String[] invalidInputs = new String[] {
                "1111111111111111111111111200",
                "0000000000000000000000000011",
                "1111101001011101010101000120",
                "1111111111111111111111100000",
                "1010011010101010010101110112",
                "0000000000000000000000000000"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x));
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x));
            }
        }

        [TestMethod]
        public void TestMatchAutomata3() {
            /*
             *      L = {0,1}*{1}{0,1}{0,1} (AFN)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", false),
                new State("q3", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "0", states[0]),
                new Transition(states[0], "1", states[0]),
                new Transition(states[0], "1", states[1]),
                new Transition(states[1], "0", states[2]),
                new Transition(states[1], "1", states[2]),
                new Transition(states[2], "0", states[3]),
                new Transition(states[2], "1", states[3])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFN);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "1011010101010010101010101100",
                "1010101101101011010101010101",
                "1111111111111111111111111110",
                "1111111111111111111111111111",
                "0000000000011000000000000111",
                "1111111111111111111111110100"
            };

            String[] invalidInputs = new String[] {
                "1111111111111111111111111200",
                "0000000000000000000000000011",
                "1111101001011101010101000120",
                "1111111111111111111111100000",
                "1010011010101010010101110112",
                "0000000000000000000000000000"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x));
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x));
            }
        }

        [TestMethod]
        public void TestMatchAutomata4() {
            /*
             *      L = {0}*{1}*{2}* (AFNe)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1", "2", "@"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "0", states[0]),
                new Transition(states[0], "@", states[1]),
                new Transition(states[1], "1", states[1]),
                new Transition(states[1], "@", states[2]),
                new Transition(states[2], "2", states[2])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "0000011111112222222222222222",
                "0000000000000000000000000012",
                "2",
                "1",
                "11111111111111",
                "0000022222222222222222222",
                "0",
            };

            String[] invalidInputs = new String[] {
                "0000111111111111222222222223",
                "1222222222222222222222222223",
                "1111222111112222222222222222",
                "0000001111111222222222222221",
                "3000000000111111111111222222",
                "3"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestMatchAutomata5() {
            /*
             *      L = {0}*{1}*{2}* (AFN)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1", "2"
            };

            State[] states = new State[] {
                new State("q0", true),
                new State("q1", true),
                new State("q2", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "0", states[0]),
                new Transition(states[0], "0", states[1]),
                new Transition(states[0], "0", states[2]),
                new Transition(states[0], "1", states[1]),
                new Transition(states[0], "1", states[2]),
                new Transition(states[0], "2", states[2]),
                new Transition(states[1], "1", states[1]),
                new Transition(states[1], "1", states[2]),
                new Transition(states[1], "2", states[2]),
                new Transition(states[2], "2", states[2])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFN);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "0000011111112222222222222222",
                "0000000000000000000000000012",
                "2",
                "1",
                "11111111111111",
                "0000022222222222222222222",
                "0",
            };

            String[] invalidInputs = new String[] {
                "0000111111111111222222222223",
                "1222222222222222222222222223",
                "1111222111112222222222222222",
                "0000001111111222222222222221",
                "3000000000111111111111222222",
                "3"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestMatchAutomata6() {
            /*
             *      L = {@,+,-}{0,...,9}*{.}{0,...,9}* (AFNe)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                "+", "-", ".", "@"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", false),
                new State("q3", false),
                new State("q4", false),
                new State("q5", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "@", states[1]),
                new Transition(states[0], "+", states[1]),
                new Transition(states[0], "-", states[1]),
                new Transition(states[1], "0", states[1]),
                new Transition(states[1], "1", states[1]),
                new Transition(states[1], "2", states[1]),
                new Transition(states[1], "3", states[1]),
                new Transition(states[1], "4", states[1]),
                new Transition(states[1], "5", states[1]),
                new Transition(states[1], "6", states[1]),
                new Transition(states[1], "7", states[1]),
                new Transition(states[1], "8", states[1]),
                new Transition(states[1], "9", states[1]),
                new Transition(states[1], "0", states[4]),
                new Transition(states[1], "1", states[4]),
                new Transition(states[1], "2", states[4]),
                new Transition(states[1], "3", states[4]),
                new Transition(states[1], "4", states[4]),
                new Transition(states[1], "5", states[4]),
                new Transition(states[1], "6", states[4]),
                new Transition(states[1], "7", states[4]),
                new Transition(states[1], "8", states[4]),
                new Transition(states[1], "9", states[4]),
                new Transition(states[1], ".", states[2]),
                new Transition(states[2], "0", states[3]),
                new Transition(states[2], "1", states[3]),
                new Transition(states[2], "2", states[3]),
                new Transition(states[2], "3", states[3]),
                new Transition(states[2], "4", states[3]),
                new Transition(states[2], "5", states[3]),
                new Transition(states[2], "6", states[3]),
                new Transition(states[2], "7", states[3]),
                new Transition(states[2], "8", states[3]),
                new Transition(states[2], "9", states[3]),
                new Transition(states[3], "0", states[3]),
                new Transition(states[3], "1", states[3]),
                new Transition(states[3], "2", states[3]),
                new Transition(states[3], "3", states[3]),
                new Transition(states[3], "4", states[3]),
                new Transition(states[3], "5", states[3]),
                new Transition(states[3], "6", states[3]),
                new Transition(states[3], "7", states[3]),
                new Transition(states[3], "8", states[3]),
                new Transition(states[3], "9", states[3]),
                new Transition(states[4], ".", states[3]),
                new Transition(states[3], "@", states[5]),
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "+.020165451210",
                ".451541321531",
                "58146514.561456",
                "-854654.",
                "-585.5126513",
                "-.4754648",
                "256.2662562",
            };

            String[] invalidInputs = new String[] {
                "egsrdhjdtsjsgfnsgbn",
                "5641684651",
                "6846515143.6456145654.561465123",
                "-.",
                ".",
                "+98465.8645a"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestMatchAutomata7() {
            /*
             *      L = {@,+,-}{0,...,9}*{.}{0,...,9}* (AFD)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                "+", "-", "."
            };

            State[] originalStates = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", false),
                new State("q3", false),
                new State("q4", false),
                new State("q5", true)
            };

            State[] states = new State[] {
                new GroupedState(new List<State> {
                    originalStates[0],
                    originalStates[1]
                }),
                originalStates[1],
                originalStates[2],
                new GroupedState(new List<State> {
                    originalStates[1],
                    originalStates[4]
                }),
                new GroupedState(new List<State> {
                    originalStates[2],
                    originalStates[3],
                    originalStates[5]
                }),
                new GroupedState(new List<State> {
                    originalStates[3],
                    originalStates[5]
                }),
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "+", states[1]),
                new Transition(states[0], "-", states[1]),
                new Transition(states[0], ".", states[2]),
                new Transition(states[0], "0", states[3]),
                new Transition(states[0], "1", states[3]),
                new Transition(states[0], "2", states[3]),
                new Transition(states[0], "3", states[3]),
                new Transition(states[0], "4", states[3]),
                new Transition(states[0], "5", states[3]),
                new Transition(states[0], "6", states[3]),
                new Transition(states[0], "7", states[3]),
                new Transition(states[0], "8", states[3]),
                new Transition(states[0], "9", states[3]),
                new Transition(states[1], "0", states[3]),
                new Transition(states[1], "1", states[3]),
                new Transition(states[1], "2", states[3]),
                new Transition(states[1], "3", states[3]),
                new Transition(states[1], "4", states[3]),
                new Transition(states[1], "5", states[3]),
                new Transition(states[1], "6", states[3]),
                new Transition(states[1], "7", states[3]),
                new Transition(states[1], "8", states[3]),
                new Transition(states[1], "9", states[3]),
                new Transition(states[1], ".", states[2]),
                new Transition(states[2], "0", states[5]),
                new Transition(states[2], "1", states[5]),
                new Transition(states[2], "2", states[5]),
                new Transition(states[2], "3", states[5]),
                new Transition(states[2], "4", states[5]),
                new Transition(states[2], "5", states[5]),
                new Transition(states[2], "6", states[5]),
                new Transition(states[2], "7", states[5]),
                new Transition(states[2], "8", states[5]),
                new Transition(states[2], "9", states[5]),
                new Transition(states[3], "0", states[3]),
                new Transition(states[3], "1", states[3]),
                new Transition(states[3], "2", states[3]),
                new Transition(states[3], "3", states[3]),
                new Transition(states[3], "4", states[3]),
                new Transition(states[3], "5", states[3]),
                new Transition(states[3], "6", states[3]),
                new Transition(states[3], "7", states[3]),
                new Transition(states[3], "8", states[3]),
                new Transition(states[3], "9", states[3]),
                new Transition(states[3], ".", states[4]),
                new Transition(states[4], "0", states[5]),
                new Transition(states[4], "1", states[5]),
                new Transition(states[4], "2", states[5]),
                new Transition(states[4], "3", states[5]),
                new Transition(states[4], "4", states[5]),
                new Transition(states[4], "5", states[5]),
                new Transition(states[4], "6", states[5]),
                new Transition(states[4], "7", states[5]),
                new Transition(states[4], "8", states[5]),
                new Transition(states[4], "9", states[5]),
                new Transition(states[5], "0", states[5]),
                new Transition(states[5], "1", states[5]),
                new Transition(states[5], "2", states[5]),
                new Transition(states[5], "3", states[5]),
                new Transition(states[5], "4", states[5]),
                new Transition(states[5], "5", states[5]),
                new Transition(states[5], "6", states[5]),
                new Transition(states[5], "7", states[5]),
                new Transition(states[5], "8", states[5]),
                new Transition(states[5], "9", states[5])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "+.020165451210",
                ".451541321531",
                "58146514.561456",
                "-854654.",
                "-585.5126513",
                "-.4754648",
                "256.2662562",
            };

            String[] invalidInputs = new String[] {
                "egsrdhjdtsjsgfnsgbn",
                "5641684651",
                "6846515143.6456145654.561465123",
                "-.",
                ".",
                "+98465.8645a"
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }
    }
}