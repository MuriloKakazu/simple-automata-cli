using System;
using AutomataCLI.AutomataOperators;
using AutomataCLI.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataCLI_Tests.StructTests {
    [TestClass]
    public class AutomataConverterTests {
        [TestMethod]
        public void TestConversion_NDFA_To_DFA_0() {
            /*
             *      L = {a,b}*{a}{a,b}{a,b} (AFN)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b"
            };

            State[] states = new State[] {
                new State("q1", false),
                new State("q2", false),
                new State("q3", false),
                new State("q4", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "a", states[0]),
                new Transition(states[0], "b", states[0]),
                new Transition(states[0], "a", states[1]),
                new Transition(states[1], "a", states[2]),
                new Transition(states[1], "b", states[2]),
                new Transition(states[2], "a", states[3]),
                new Transition(states[2], "b", states[3])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFN);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "aabb",
                "aaabb",
                "ababababababababababb",
                "ababababaaaaaaaaaaabb",
                "aaaaaaaaaaaaaaaaaaaaa",
                "bbbbbbbbbbbbbbbbbbabb",
                "aaaaaaaaaaaaaabbabaab",
                "ababababababababababa"
            };

            String[] invalidInputs = new String[] {
                "abababbbbbbbbbbbbbbbbb",
                "ababababababbababaabba",
                "Xababababababababaaabb",
                "ababababbabababababXab",
                "ab",
                "bba",
                "a",
                "b",
                ""
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }

            Automata convertedAutomata = AutomataConverter.ToDFA(automata);

            reader = new AutomataReader(convertedAutomata);

            Assert.AreEqual(AutomataType.AFD, convertedAutomata.GetAutomataType());

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestConversion_NDFAe_To_DFA_0() {
            /*
             *      L = {a,b}*{a}{a,b}{a,b} (AFNe)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b"
            };

            State[] states = new State[] {
                new State("q1", false),
                new State("q2", false),
                new State("q3", false),
                new State("q4", false),
                new State("q5", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "a", states[0]),
                new Transition(states[0], "b", states[0]),
                new Transition(states[0], "a", states[1]),
                new Transition(states[1], "a", states[2]),
                new Transition(states[1], "b", states[2]),
                new Transition(states[2], "a", states[3]),
                new Transition(states[2], "b", states[3]),
                new Transition(states[3], "@", states[4])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "aabb",
                "aaabb",
                "ababababababababababb",
                "ababababaaaaaaaaaaabb",
                "aaaaaaaaaaaaaaaaaaaaa",
                "bbbbbbbbbbbbbbbbbbabb",
                "aaaaaaaaaaaaaabbabaab",
                "ababababababababababa"
            };

            String[] invalidInputs = new String[] {
                "abababbbbbbbbbbbbbbbbb",
                "ababababababbababaabba",
                "Xababababababababaaabb",
                "ababababbabababababXab",
                "ab",
                "bba",
                "a",
                "b",
                ""
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }

            Automata convertedAutomata = AutomataConverter.ToDFA(automata);

            reader = new AutomataReader(convertedAutomata);

            Assert.AreEqual(AutomataType.AFD, convertedAutomata.GetAutomataType());

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestConversion_NDFAe_To_DFA_1() {
            /*
             *      L = {a}{a,b}* (AFNe)
             */

            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "a", states[1]),
                new Transition(states[1], "a", states[2]),
                new Transition(states[1], "b", states[2]),
                new Transition(states[1], "@", states[2]),
                new Transition(states[2], "a", states[2]),
                new Transition(states[2], "b", states[2])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                "aabb",
                "aaabb",
                "ababababababababababb",
                "ababababaaaaaaaaaaabb",
                "aaaaaaaaaaaaaaaaaaaaa",
                "abbbbbbbbbbbbbbbbbabb",
                "aaaaaaaaaaaaaabbabaab",
                "ababababababababababa",
                "a",
                "aa"
            };

            String[] invalidInputs = new String[] {
                "bbababbbbbbbbbbbbbbbbb",
                "bbabababababbababaabba",
                "Xababababababababaaabb",
                "bbabababbabababababXab",
                "bb",
                "bba",
                "b",
                "b",
                ""
            };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }

            Automata convertedAutomata = AutomataConverter.ToDFA(automata);

            reader = new AutomataReader(convertedAutomata);

            Assert.AreEqual(AutomataType.AFD, convertedAutomata.GetAutomataType());

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestConversion_NDFAe_To_DFA_2() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", false),
                new State("q3", false),
                new State("q4", false),
                new State("q5", false),
                new State("q6", false),
                new State("q7", false),
                new State("q8", false),
                new State("q9", false),
                new State("q10", false),
                new State("q11", false),
                new State("q12", false),
                new State("q13", false),
                new State("q14", false),
                new State("q15", false),
                new State("q16", false),
                new State("q17", false),
                new State("q18", false),
                new State("q19", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "@", states[1]),
                new Transition(states[1], "@", states[2]),
                new Transition(states[1], "@", states[4]),
                new Transition(states[2], "a", states[3]),
                new Transition(states[3], "@", states[6]),
                new Transition(states[4], "@", states[5]),
                new Transition(states[5], "@", states[6]),
                new Transition(states[6], "@", states[7]),
                new Transition(states[7], "@", states[8]),
                new Transition(states[7], "@", states[18]),
                new Transition(states[8], "@", states[9]),
                new Transition(states[9], "@", states[10]),
                new Transition(states[9], "@", states[12]),
                new Transition(states[10], "b", states[11]),
                new Transition(states[11], "@", states[16]),
                new Transition(states[12], "b", states[13]),
                new Transition(states[13], "@", states[14]),
                new Transition(states[14], "a", states[15]),
                new Transition(states[15], "@", states[16]),
                new Transition(states[16], "@", states[17]),
                new Transition(states[17], "@", states[18]),
                new Transition(states[17], "@", states[8]),
                new Transition(states[18], "@", states[19])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                 "",
                 "a",
                 "b",
                 "bbbbb",
                 "aba",
                 "abab",
                 "ba",
                 "bab",
                 "bbbbbbbbbbbbbbb",
                 "bbbbbbbbbbbbabbbbb"
             };

            String[] invalidInputs = new String[] {
                 "aa",
                 "aab",
                 "abaa",
                 "ababbbbbbbbaa",
                 "bbbbbbbbbbbbbbaa",
                 "abbbbbX",
                 "Xbbbbbaaabbbb",
                 "ababababababababababaaab",
                 "babababaabab"
             };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }

            Automata convertedAutomata = AutomataConverter.ToDFA(automata);

            reader = new AutomataReader(convertedAutomata);

            Assert.AreEqual(AutomataType.AFD, convertedAutomata.GetAutomataType());

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }

        [TestMethod]
        public void TestConversion_NDFAe_To_DFA_3() {
            Automata automata = new Automata();

            String[] symbols = new String[] {
                "a", "b"
            };

            State[] states = new State[] {
                new State("q0", false),
                new State("q1", false),
                new State("q2", false),
                new State("q3", false),
                new State("q4", false),
                new State("q5", false),
                new State("q6", false),
                new State("q7", false),
                new State("q8", false),
                new State("q9", false),
                new State("q10", false),
                new State("q11", false),
                new State("q12", false),
                new State("q13", false),
                new State("q14", false),
                new State("q15", false),
                new State("q16", false),
                new State("q17", false),
                new State("q18", false),
                new State("q19", true)
            };

            Transition[] transitions = new Transition[] {
                new Transition(states[0], "@", states[1]),
                new Transition(states[1], "@", states[2]),
                new Transition(states[1], "@", states[4]),
                new Transition(states[2], "a", states[3]),
                new Transition(states[3], "@", states[6]),
                new Transition(states[4], "@", states[5]),
                new Transition(states[5], "@", states[6]),
                new Transition(states[6], "@", states[7]),
                new Transition(states[7], "@", states[8]),
                new Transition(states[7], "@", states[18]),
                new Transition(states[8], "@", states[9]),
                new Transition(states[9], "@", states[10]),
                new Transition(states[9], "@", states[12]),
                new Transition(states[10], "b", states[11]),
                new Transition(states[11], "@", states[16]),
                new Transition(states[12], "b", states[13]),
                new Transition(states[13], "@", states[14]),
                new Transition(states[14], "a", states[15]),
                new Transition(states[15], "@", states[16]),
                new Transition(states[16], "@", states[17]),
                new Transition(states[17], "@", states[18]),
                new Transition(states[17], "@", states[8]),
                new Transition(states[18], "@", states[19])
            };

            automata.AddSymbols(symbols);
            automata.AddStates(states);
            automata.AddTransitions(transitions);
            automata.SetAutomataType(AutomataType.AFNe);
            automata.SetInitialState(states[0]);

            String[] validInputs = new String[] {
                 "",
                 "a",
                 "b",
                 "bbbbb",
                 "aba",
                 "abab",
                 "ba",
                 "bab",
                 "bbbbbbbbbbbbbbb",
                 "bbbbbbbbbbbbabbbbb"
             };

            String[] invalidInputs = new String[] {
                 "aa",
                 "aab",
                 "abaa",
                 "ababbbbbbbbaa",
                 "bbbbbbbbbbbbbbaa",
                 "abbbbbX",
                 "Xbbbbbaaabbbb",
                 "ababababababababababaaab",
                 "babababaabab"
             };

            AutomataReader reader = new AutomataReader(automata);

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }

            Automata convertedAutomata = AutomataConverter.ToDFA(automata);

            reader = new AutomataReader(convertedAutomata);

            Assert.AreEqual(AutomataType.AFD, convertedAutomata.GetAutomataType());

            foreach (var x in validInputs) {
                Assert.IsTrue(reader.Matches(x), $"{x} was supposed to match");
            }

            foreach (var x in invalidInputs) {
                Assert.IsFalse(reader.Matches(x), $"{x} was not supposed to match");
            }
        }
    }
}