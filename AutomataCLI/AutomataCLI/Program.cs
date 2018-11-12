using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataCLI.Struct;
using System.Diagnostics;

namespace AutomataCLI {
    class Program {
        static void Main(string[] args) {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Automata automata = new Automata();

            String input = "123";

            State state0 = new State("Q0", false),
                  state1 = new State("Q1", false),
                  state2 = new State("Q2", true),
                  state3 = new State("Q3", false),
                  state4 = new State("Q4", false);

            automata.States = new List<State>(){
                state0,
                state1,
                state2,
                state3,
                state4
            };

            automata.InitialState = state1;

            automata.Transitions = new List<Transition>(){
                new Transition(state0, "1", state0),
                new Transition(state0, "2", state0),
                new Transition(state0, "3", state0),
                new Transition(state0, "1", state1),
                new Transition(state0, "2", state3),
                new Transition(state0, "3", state4),

                new Transition(state1, "1", state1),
                new Transition(state1, "2", state1),
                new Transition(state1, "3", state1),
                new Transition(state1, "1", state2),

                new Transition(state3, "1", state3),
                new Transition(state3, "2", state3),
                new Transition(state3, "3", state3),
                new Transition(state3, "2", state2),

                new Transition(state4, "1", state4),
                new Transition(state4, "2", state4),
                new Transition(state4, "3", state4),
                new Transition(state4, "3", state2),
            };

            automata.RefreshFinalStates();

            Console.WriteLine(new AutomataReader(automata).Matches(input));
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalMilliseconds.ToString());
            Console.ReadKey();
        }
    }
}