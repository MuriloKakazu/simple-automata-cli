using System;

namespace AutomataCLI.Struct {
    public class Transition {
        public Char Input { get; protected set; }
        public State From { get; protected set; }
        public State To { get; protected set; }

        public Transition(State from, Char input, State to) {
            this.From = from;

            this.Input = input;
            this.To    = to;
        }

        public override String ToString()
            => $"({From}, {Input}, {To})";
    }
}