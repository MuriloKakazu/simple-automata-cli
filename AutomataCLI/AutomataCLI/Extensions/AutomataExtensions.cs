using System;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Exceptions;
using AutomataCLI.Struct;

namespace AutomataCLI.Extensions {
    public static class AutomataExtensions {
        public static IEnumerable<Transition> GetTransitionsFromState(this Automata automata, State state) {
            if (!automata.ContainsState(state)) {
                throw new InvalidValueException(
                    state?.Name, typeof(State)
                );
            }

            State automataStateInstance = automata.GetStateLike(state);
            return automata.GetTransitions().ToList().Where(
                x => x.From == automataStateInstance
            );
        }

        public static IEnumerable<Transition> GetTransitionsFromState(this Automata automata, State state, String symbol) {
            if (!automata.ContainsState(state)) {
                throw new InvalidValueException(
                    state?.Name,
                    typeof(State)
                );
            }
            if (!automata.ContainsSymbol(symbol)) {
                throw new InvalidValueException(
                    symbol
                );
            }

            State automataStateInstance = automata.GetStateLike(state);
            return automata.GetTransitions().ToList().Where(
                x => x.From  == automataStateInstance &&
                     x.Input == symbol
            );
        }

        public static IEnumerable<Transition> GetTransitionsToState(this Automata automata, State state) {
            if (!automata.ContainsState(state)) {
                throw new InvalidValueException(
                    state?.Name,
                    typeof(State)
                );
            }

            State automataStateInstance = automata.GetStateLike(state);
            return automata.GetTransitions().ToList().Where(
                x => x.To == automataStateInstance
            );
        }

        public static IEnumerable<Transition> GetTransitionsToState(this Automata automata, State state, String symbol) {
            if (!automata.ContainsState(state)) {
                throw new InvalidValueException(
                    state?.Name,
                    typeof(Transition)
                );
            }
            if (!automata.ContainsSymbol(symbol)) {
                throw new InvalidValueException(
                    symbol
                );
            }

            State automataStateInstance = automata.GetStateLike(state);
            return automata.GetTransitions().ToList().Where(
                x => x.To    == automataStateInstance &&
                     x.Input == symbol
            );
        }

        public static IEnumerable<Transition> GetTransitionsWithSymbol(this Automata automata, String symbol) {
            if (!automata.ContainsSymbol(symbol)) {
                throw new InvalidValueException(
                    symbol
                );
            }

            return automata.GetTransitions().ToList().Where(
                x => x.Input == symbol
            );
        }
    }
}