using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomataCLI.Struct {

    public class GroupedState : State {
        private List<State> SubStates {get; set;}

        public State[] GetSubStates() 
            => this.SubStates.ToArray();

        public void SetSubStates(List<State> states) 
            => this.SubStates = states;

        public GroupedState(List<State> subStates) 
            : base(String.Join("_", 
                subStates.Select(x => x.Name)), 
                subStates.Any(x => x.IsFinal)) {
            
            SetSubStates(subStates);
        }
    }
}