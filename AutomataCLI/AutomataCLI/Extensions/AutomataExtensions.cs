using System;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Exceptions;
using AutomataCLI.Struct;

namespace AutomataCLI.Extensions {
    public static class AutomataExtensions {
        public static IEnumerable<Transition> GetTransitionsFromState(this Automata automata, State state) {
            if (!automata.ContainsState(state)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, state?.Name);
            }

            return automata.GetTransitions().ToList().Where(
                x => x.From == state
            );
        }

        public static IEnumerable<Transition> GetTransitionsFromState(this Automata automata, State state, String symbol) {
            if (!automata.ContainsState(state)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, state?.Name);
            }
            if (!automata.ContainsSymbol(symbol)) {
                throw new AutomataException(AutomataException.MESSAGE_SYMBOL_NOT_FOUND, symbol);
            }

            return automata.GetTransitions().ToList().Where(
                x => x.From  == state &&
                     x.Input == symbol
            );
        }

        public static IEnumerable<Transition> GetTransitionsToState(this Automata automata, State state) {
            if (!automata.ContainsState(state)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, state?.Name);
            }

            return automata.GetTransitions().ToList().Where(
                x => x.To == state
            );
        }

        public static IEnumerable<Transition> GetTransitionsToState(this Automata automata, State state, String symbol) {
            if (!automata.ContainsState(state)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, state?.Name);
            }
            if (!automata.ContainsSymbol(symbol)) {
                throw new AutomataException(AutomataException.MESSAGE_SYMBOL_NOT_FOUND, symbol);
            }

            return automata.GetTransitions().ToList().Where(
                x => x.To    == state &&
                     x.Input == symbol
            );
        }

        public static IEnumerable<Transition> GetTransitionsWithSymbol(this Automata automata, String symbol) {
            if (!automata.ContainsSymbol(symbol)) {
                throw new AutomataException(AutomataException.MESSAGE_SYMBOL_NOT_FOUND, symbol);
            }

            return automata.GetTransitions().ToList().Where(
                x => x.Input == symbol
            );
        }
    }
}