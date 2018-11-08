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
            this.RefreshFinalStates();
        }

        public void RemoveState(State state) {
            this.States.Remove(state);
            this.RefreshFinalStates();
        }

        public void SetStates(ICollection<State> stateCollection) {
            States = new List<State>(stateCollection);
        }

        public void SetInitialState(State state) {
            this.InitialState = state;
        }

        private void RefreshFinalStates() {
            if (States != null) {
                this.FinalStates = States.Where(x => x.IsFinal);
            }
        }

        public void ClearStates() {
            this.States.Clear();
        }
    }
}
