using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Struct {
    public class Automata {
        public AutomataType Type { get; protected set; }
        public List<State> States { get; protected set; }
        public List<Transition> Transitions { get; protected set; }
        public State InitialState { get; protected set; }
        public List<State> FinalStates { get; protected set; }

        public Automata() {
            States      = new List<State>();
            Transitions = new List<Transition>();
            FinalStates = new List<State>();
        }

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
            this.RefreshFinalStates();
        }

        public void SetInitialState(State state) {
            this.InitialState = state;
        }

        private void RefreshFinalStates() {
            if (States != null) {
                this.FinalStates = States.Where(
                    x => x.IsFinal
                ).ToList();
            }
        }

        public void ClearStates() {
            this.States.Clear();
        }

        public override String ToString() {
            String newLine      = Environment.NewLine;
            String type         = this.Type.ToString() + newLine;
            String states       = "";
            String initialState = this.InitialState.ToString() + newLine;
            String finalStates  = "";
            String transitions  = "";

            this.States.ForEach(
                x => states += x.ToString() + newLine
            );
            this.FinalStates.ForEach(
                x => finalStates += x.ToString() + newLine
            );
            this.Transitions.ForEach(
                x => transitions += x.ToString() + newLine
            );

            return $"{type}{states}{initialState}{finalStates}{transitions}";
        }
    }
}
