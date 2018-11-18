using System;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataCLI_Tests.StructTests {
    [TestClass]
    public class AutomataTests {
        [TestMethod]
        public void TestCreateValidAutomata_Full() {
            Automata automata = new Automata();

            String[] symbols =  new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);

            Assert.AreEqual(AutomataType.AFNe, automata.GetAutomataType());
            Assert.IsTrue(symbols.Length     == automata.GetSymbols().Length);
            Assert.IsTrue(states.Length      == automata.GetStates().Length);
            Assert.IsTrue(transitions.Length == automata.GetTransitions().Length);

            foreach (var x in symbols) {
                Assert.IsTrue(automata.ContainsSymbol(x));
            }

            foreach (var x in states) {
                Assert.IsTrue(automata.ContainsState(x));
            }

            foreach (var x in transitions) {
                Assert.IsTrue(automata.ContainsTransition(x));
            }
        }

        [TestMethod]
        public void TestAddSymbols() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
            };
            String extraSymbol = "d";

            automata.AddSymbols(symbols);
            automata.AddSymbol(extraSymbol);

            Assert.IsTrue((symbols.Length + 1) == (automata.GetSymbols().Length));

            foreach (var x in symbols) {
                Assert.IsTrue(automata.ContainsSymbol(x));
            }
            Assert.IsTrue(automata.ContainsSymbol(extraSymbol));
        }

        [TestMethod]
        public void TestRemoveSymbols() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
            };

            String[] removeSymbols = new String[] {
                symbols[0], symbols[1]
            };


            automata.AddSymbols(symbols);
            automata.RemoveSymbols(removeSymbols);

            List<String> expectedSymbols = new List<String>();
            symbols.ToList().ForEach(
                x => {
                    if (!removeSymbols.Contains(x)) {
                        expectedSymbols.Add(x);
                    }
                }
            );

            Assert.IsTrue(expectedSymbols.Count == automata.GetSymbols().Length);

            foreach (var x in expectedSymbols) {
                Assert.IsTrue(automata.ContainsSymbol(x));
            }
        }

        [TestMethod]
        public void TestSetSymbols() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
            };

            automata.SetSymbols(symbols);

            Assert.IsTrue(symbols.Length == automata.GetSymbols().Length);

            foreach (var x in symbols) {
                Assert.IsTrue(automata.ContainsSymbol(x));
            }

            String[] newSymbols = new String[] {
                "d", "e", "f", "g",
                "h", "i", "j", "k"
            };

            automata.SetSymbols(newSymbols);

            Assert.IsTrue(newSymbols.Length == automata.GetSymbols().Length);

            foreach (var x in newSymbols) {
                Assert.IsTrue(automata.ContainsSymbol(x));
            }
        }

        [TestMethod]
        public void TestClearSymbols() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
            };

            automata.SetSymbols(symbols);

            Assert.IsTrue(symbols.Length == automata.GetSymbols().Length);

            foreach (var x in symbols) {
                Assert.IsTrue(automata.ContainsSymbol(x));
            }

            automata.ClearSymbols();

            Assert.AreEqual(0, automata.GetSymbols().Length);

            foreach (var x in symbols) {
                Assert.IsFalse(automata.ContainsSymbol(x));
            }
        }

        [TestMethod]
        public void TestRemoveSymbolDependencies() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);

            String[] removeSymbols = new String[] {
                symbols[0],
                symbols[1]
            };

            automata.RemoveSymbols(removeSymbols, removeDependencies: true);

            Transition[] expectedRemainingTransitions = transitions.Where(
                x => !removeSymbols.Contains(x.Input)
            ).ToArray();

            Transition[] removedTransitions = transitions.Where(
                x => removeSymbols.Contains(x.Input)
            ).ToArray();

            Assert.AreEqual(expectedRemainingTransitions.Length, automata.GetTransitions().Length);

            foreach (var x in removeSymbols) {
                Assert.IsFalse(automata.ContainsSymbol(x));
            }

            foreach (var x in removedTransitions) {
                Assert.IsFalse(automata.ContainsTransition(x));
            }
        }

        [TestMethod]
        public void TestAddStates() {
            Automata automata = new Automata();

            State[] states = new State[] {
                new State("x", false),
                new State("y", false),
                new State("z", true)
            };

            String  extraState = "k";
            Boolean extraStateIsFinal = true;

            automata.AddStates(states);
            automata.AddState(extraState, extraStateIsFinal);

            Assert.IsTrue((states.Length + 1) == (automata.GetStates().Length));

            foreach (var x in states) {
                Assert.IsTrue(automata.ContainsState(x));
            }
            Assert.IsTrue(automata.ContainsState(extraState));

            List<State> finalStates = states.ToList().Where(
                x => x.IsFinal
            ).ToList();

            Assert.AreEqual((finalStates.Count + 1), (automata.GetFinalStates().Length));

            foreach (var x in finalStates) {
                Assert.IsTrue(automata.GetFinalStates().Contains(x));
                Assert.IsTrue(automata.GetStateLike(x).IsFinal);
            }
            Assert.IsTrue(automata.GetStateLike(extraState).IsFinal);
        }

        [TestMethod]
        public void TestRemoveStates() {
            Automata automata = new Automata();

            State[] states = new State[] {
                new State("x", false),
                new State("y", false),
                new State("z", true)
            };

            String[] removeStates = new String[] {
                "y", "z"
            };

            automata.AddStates(states);
            automata.RemoveStates(removeStates);

            List<String> expectedStates = states.ToList().Where(
                x => !removeStates.Contains(x.Name)
            ).ToList().Select(
                x => x.Name
            ).ToList();

            Assert.IsTrue(expectedStates.Count == automata.GetStates().Length);

            foreach (var x in expectedStates) {
                Assert.IsTrue(automata.ContainsState(x));
            }
        }

        [TestMethod]
        public void TestSetStates() {
            Automata automata = new Automata();

            State[] states = new State[] {
                new State("x", false),
                new State("y", false),
                new State("z", true)
            };

            automata.SetStates(states);

            Assert.IsTrue(states.Length == automata.GetStates().Length);

            foreach (var x in states) {
                Assert.IsTrue(automata.ContainsState(x));
            }

            State[] newStates = new State[] {
                new State("a", false),
                new State("b", false),
                new State("c", true),
                new State("d", true),
                new State("e", true),
                new State("f", true),
                new State("g", true)
            };

            automata.SetStates(newStates);

            Assert.IsTrue(newStates.Length == automata.GetStates().Length);

            foreach (var x in newStates) {
                Assert.IsTrue(automata.ContainsState(x));
            }
        }

        [TestMethod]
        public void TestClearStates() {
            Automata automata = new Automata();

            State[] states = new State[] {
                new State("x", false),
                new State("y", false),
                new State("z", true)
            };

            automata.SetStates(states);

            Assert.IsTrue(states.Length == automata.GetStates().Length);

            foreach (var x in states) {
                Assert.IsTrue(automata.ContainsState(x));
            }

            automata.ClearStates();

            Assert.AreEqual(0, automata.GetStates().Length);

            foreach (var x in states) {
                Assert.IsFalse(automata.ContainsState(x));
            }
        }

        [TestMethod]
        public void TestRemoveStateDependencies() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);

            State[] removeStates = new State[] {
                states[0],
                states[1]
            };

            automata.RemoveStates(removeStates, removeDependencies: true);

            Transition[] expectedRemainingTransitions = transitions.Where(
                x => !removeStates.Contains(x.From) && !removeStates.Contains(x.To)
            ).ToArray();

            Transition[] removedTransitions = transitions.Where(
                x => removeStates.Contains(x.From) || removeStates.Contains(x.To)
            ).ToArray();

            Assert.AreEqual(expectedRemainingTransitions.Length, automata.GetTransitions().Length);

            foreach (var x in removeStates) {
                Assert.IsFalse(automata.ContainsState(x));
            }

            foreach (var x in removedTransitions) {
                Assert.IsFalse(automata.ContainsTransition(x));
            }
        }

        [TestMethod]
        public void TestSetInitialState() {
            Automata automata = new Automata();

            State[] states = new State[] {
                new State("x", false),
                new State("y", false),
                new State("z", true)
            };

            automata.SetStates(states);
            automata.SetInitialState(states[0]);

            Assert.IsTrue(states.Length == automata.GetStates().Length);

            foreach (var x in states) {
                Assert.IsTrue(automata.ContainsState(x));
            }

            Assert.IsTrue(states[0] == automata.GetInitialState());
        }

        [TestMethod]
        public void TestAddTransitions() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);

            Assert.IsTrue(transitions.Length == automata.GetTransitions().Length);

            foreach (var x in transitions) {
                Assert.IsTrue(automata.ContainsTransition(x));
            }
        }

        [TestMethod]
        public void TestRemoveTransitions() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            Transition[] removeTransitions = new Transition[] {
                transitions[0],
                transitions[1]
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);
            automata.RemoveTransitions(removeTransitions);

            List<Transition> expectedTransitions = transitions.Where(
                x => !removeTransitions.Contains(x)
            ).ToList();

            Assert.IsTrue(expectedTransitions.Count == automata.GetTransitions().Length);

            foreach (var x in expectedTransitions) {
                Assert.IsTrue(automata.ContainsTransition(x));
            }
        }

        [TestMethod]
        public void TestSetTransitions() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);

            Assert.IsTrue(transitions.Length == automata.GetTransitions().Length);

            foreach (var x in transitions) {
                Assert.IsTrue(automata.ContainsTransition(x));
            }

            Transition[] newTransitions = new Transition[] {
                new Transition(states[0], symbols[2], states[1]),
                new Transition(states[1], symbols[1], states[0]),
                new Transition(states[2], symbols[2], states[1]),
                new Transition(states[1], symbols[2], states[0]),
            };

            automata.SetTransitions(newTransitions);

            Assert.IsTrue(newTransitions.Length == automata.GetTransitions().Length);

            foreach (var x in newTransitions) {
                Assert.IsTrue(automata.ContainsTransition(x));
            }
        }

        [TestMethod]
        public void TestClearTransitions() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b", "c"
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
                new Transition(states[0], symbols[0], states[2]),
            };

            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetSymbols(symbols);
            automata.SetStates(states);
            automata.SetInitialState(states[0]);
            automata.SetTransitions(transitions);

            Assert.IsTrue(transitions.Length == automata.GetTransitions().Length);

            foreach (var x in transitions) {
                Assert.IsTrue(automata.ContainsTransition(x));
            }

            automata.ClearTransitions();

            Assert.AreEqual(0, automata.GetTransitions().Length);

            foreach (var x in transitions) {
                Assert.IsFalse(automata.ContainsTransition(x));
            }
        }
    }
}
