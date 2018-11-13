using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomataCLI.Struct {

    public class GroupedState : State {
        private List<State> GroupedStates {get; set;}

        public List<State> GetGroupedStates() => this.GroupedStates;
        public void SetGroupedStates(List<State> states) => this.GroupedStates = states;
        public GroupedState(List<State> groupedStates) 
            : base(String.Join("_", groupedStates.Select(x => x.Name)), groupedStates.Any(x => x.IsFinal)) {
            
            SetGroupedStates(groupedStates);
        }
    }
}