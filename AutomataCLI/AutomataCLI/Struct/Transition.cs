using System;
using AutomataCLI.Exceptions;
using AutomataCLI.Utils;

namespace AutomataCLI.Struct {
    public class Transition {
        public String Input { get; protected set; }
        public State From { get; protected set; }
        public State To { get; protected set; }

        public Transition(State from, String input, State to) {
            ValidationUtils.EnsureNotNull(
                from, new InvalidValueException(
                    from?.Name,
                    typeof(State)
                )
            );
            ValidationUtils.EnsureNotNull(
                to, new InvalidValueException(
                    to?.Name,
                    typeof(State)
                )
            );
            ValidationUtils.EnsureNotNull(
                input, new InvalidValueException(
                    input
                )
            );

            this.From  = from;
            this.Input = input?.Trim();
            this.To    = to;
        }

        public override String ToString()
            => $"({From}, {Input}, {To})";
    }
}