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

            String input = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

            State state1 = new State("Q0", false),
                  state2 = new State("Q1", false),
                  state3 = new State("Q3", false),
                  state4 = new State("Q4", true );

            automata.States = new List<State>(){
                state1,
                state2,
                state3,
                state4
            };

            automata.InitialState = state1;

            automata.Transitions = new List<Transition>(){
                new Transition(state1, "a", state2),
                new Transition(state2, "a", state1),
                new Transition(state2, "a", state3),
                new Transition(state2, "b", state3),
                new Transition(state3, "b", state3),
                new Transition(state3, "b", state4)
            };

            Console.WriteLine(new AutomataReader(automata).Matches(input));
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalMilliseconds.ToString());
            Console.ReadKey();
        }
    }
}