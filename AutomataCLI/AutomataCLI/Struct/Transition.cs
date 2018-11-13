using System;
using AutomataCLI.Exceptions;

namespace AutomataCLI.Struct {
    public class Transition {
        public String Input { get; protected set; }
        public State From { get; protected set; }
        public State To { get; protected set; }

        public Transition(State from, String input, State to) {
            if (from == null) {
                throw new AutomataException(
                    AutomataException.MESSAGE_INVALID_STATE, from?.ToString()
                );
            }
            if (to == null) {
                throw new AutomataException(
                    AutomataException.MESSAGE_INVALID_STATE, to?.ToString()
                );
            }

            this.From  = from;
            this.Input = input.Trim();
            this.To    = to;
        }

        public override String ToString()
            => $"({From}, {Input}, {To})";
    }
}