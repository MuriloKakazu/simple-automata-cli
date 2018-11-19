using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomataCLI.Struct {

    public class GroupedState : State {
        private List<State> SubStates {get; set;}

        public State[] GetSubStates()
            => this.SubStates.ToArray();

        public void SetSubStates(IEnumerable<State> states) {
            SubStates.Clear();
            states.ToList().ForEach(
                x => AddSubState(x)
            );
        }

        public GroupedState(IEnumerable<State> subStates) {
            SubStates = new List<State>();
            SetSubStates(subStates);
        }

        public void AddSubState(State newState) {
            if (newState is GroupedState) {
                var goupedState = (GroupedState) newState;
                goupedState.SubStates.ForEach(
                    x => AddSubState(x)
                );
            } else {
                if (!ContainsSubStateLike(newState)) {
                    SubStates.Add(newState);
                    //Name = (Name == null) ? 
                    //    newState.Name : 
                    //    String.Join("_", Name, newState.Name);
                    FixName();
                    if (newState.IsFinal) {
                        this.IsFinal = true;
                    }
                }
            }
        }

        protected void FixName() {
            SubStates = SubStates.OrderBy(x => x.Name).ToList();
            Name = String.Join("_", SubStates);
        }

        public void AddSubStates(IEnumerable<State> newStates) {
            newStates.ToList().ForEach(
                x => AddSubState(x)
            );
        }

        protected Boolean ContainsSubStateLike(String name) {
            return SubStates.Any(x => x.Name == name);
        }

        protected Boolean ContainsSubStateLike(State state) {
            return ContainsSubStateLike(state?.Name);
        }
    }
}