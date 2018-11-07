using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Struct {
    class Automata {
        public AutomataType Type { get; protected set; }
        public List<State> States { get; protected set; }
        public List<Transition> Transitions { get; protected set; }
        public State InitialState { get; protected set; }
        public List<State> FinalStates { get; protected set; }

        public void AddState(State state) {
            this.States.Add(state);
        }

        public void RemoveState(State state) {
            this.States.Remove(state);
        }

        public void SetInitialState(State state) {

        }

        public void ClearStates() {
            this.States.Clear();
        }
    }
}
