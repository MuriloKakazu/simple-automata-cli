using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Struct {
    public class Automata {
        public class AutomataException : Exception {
            private static String MESSAGE_BASE;
            public static String  MESSAGE_DUPLICATE_STATE;
            public static String  MESSAGE_DUPLICATE_TRANSITION;
            public static String  MESSAGE_STATE_NOT_FOUND;
            public static String  MESSAGE_SYMBOL_NOT_FOUND;
            public static String  MESSAGE_TRANSITION_NOT_FOUND;

            // static ctor
            static AutomataException() {
                MESSAGE_BASE                 = "Invalid automata.";
                MESSAGE_DUPLICATE_STATE      = "Duplicate state found:";
                MESSAGE_DUPLICATE_TRANSITION = "Duplicate transition found:";
                MESSAGE_STATE_NOT_FOUND      = "Could not find state:";
                MESSAGE_SYMBOL_NOT_FOUND     = "Could not find symbol:";
                MESSAGE_TRANSITION_NOT_FOUND = "Could not find transition:";
            }

            // ctor
            public AutomataException(String reason, String supplement) :
                base($"{MESSAGE_BASE} {reason} {supplement}") { }
        }

        public AutomataType Type { get; protected set; }
        public List<String> Symbols { get; protected set; }
        public List<State> States { get; protected set; }
        public List<Transition> Transitions { get; protected set; }
        public State InitialState { get; protected set; }
        public List<State> FinalStates { get; protected set; }

        #region automata methods

        // ctor
        public Automata() {
            Symbols = new List<String>();
            States = new List<State>();
            Transitions = new List<Transition>();
            FinalStates = new List<State>();
        }

        public void SetType(AutomataType type) {
            this.Type = type;
        }

        public void Clear() {
            this.ClearStates();
        }

        public override String ToString() {
            String newLine = Environment.NewLine;
            String type = this.Type.ToString() + newLine;
            String states = "";
            String initialState = this.InitialState.ToString() + newLine;
            String finalStates = "";
            String transitions = "";

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

        #endregion

        #region symbol methods

        public void AddSymbol(String symbol) {
            this.Symbols.Add(symbol);
        }

        public void AddSymbols(String[] symbols) {
            symbols.ToList().ForEach(
                x => AddSymbol(x)
            );
        }

        public void RemoveSymbol(String symbol, Boolean removeDependencies = false) {
            this.Symbols.Remove(symbol);
            if (removeDependencies) {

            }
        }

        public void RemoveSymbols(String[] symbols, Boolean removeDependencies = false) {
            symbols.ToList().ForEach(
                x => RemoveSymbol(x, removeDependencies)
            );
        }

        public void SetSymbols(String[] symbols, Boolean removeOldDependencies = false) {
            this.Symbols = new List<String>(symbols);
        }

        public Boolean ContainsSymbol(String symbol) {
            return this.Symbols.Exists(
                x => x == symbol
            );
        }

        public void ClearSymbols(Boolean removeDependencies = false) {
            this.Symbols.Clear();
            if (removeDependencies) {

            }
        }

        #endregion

        #region state methods

        public void AddState(State state, Boolean isFinal = false) {
            if (ContainsStateName(state.Name)) {
                throw new AutomataException(AutomataException.MESSAGE_DUPLICATE_STATE, state.Name);
            }

            this.States.Add(state);
            this.RefreshFinalStates();
        }

        public void AddState(String stateName, Boolean isFinal = false) {
            AddState(new State(stateName, isFinal));
        }

        public void AddStates(State[] states, Boolean isFinal = false) {
            states.ToList().ForEach(
                x => AddState(x, isFinal)
            );
        }

        public void RemoveState(State state, Boolean removeDependencies = false) {
            this.States.Remove(state);
            if (removeDependencies) {
                this.Transitions.Where(
                    x => x.From == state || x.To == state
                );
            }
            this.RefreshFinalStates();
        }

        public void RemoveState(String stateName, Boolean removeDependencies = false) {
            RemoveState(this.States.Find(
                x => x.Name == stateName
            ), removeDependencies);
        }

        public void RemoveStates(State[] states, Boolean removeDependencies) {
            states.ToList().ForEach(
                x => RemoveState(x, removeDependencies)
            );
        }

        public void RemoveStates(String[] states, Boolean removeDependencies) {
            states.ToList().ForEach(
                x => RemoveState(x, removeDependencies)
            );
        }

        public void SetStates(ICollection<State> stateCollection, Boolean removeOldDependencies = false) {
            this.States = new List<State>(stateCollection);
            this.RefreshFinalStates();
        }

        public Boolean ContainsStateName(String stateName) {
            return this.States.Exists(x => x.Name == stateName);
        }

        public Boolean ContainsState(State state) {
            return this.States.Exists(x => x == state);
        }

        public void SetInitialState(State state) {
            if (!ContainsState(state)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, state.Name);
            }

            this.InitialState = state;
        }

        private void RefreshFinalStates() {
            if (States != null) {
                this.FinalStates = States.Where(
                    x => x.IsFinal
                ).ToList();
            }
        }

        public void ClearStates(Boolean removeDependencies = false) {
            this.States.Clear();
        }

        #endregion

        #region transition methods

        public void AddTransition(Transition transition) {
            if (!ContainsState(transition.From)) {
                throw new AutomataException(
                    AutomataException.MESSAGE_STATE_NOT_FOUND, transition.From.Name
                );
            }
            if (!ContainsState(transition.To)) {
                throw new AutomataException(
                    AutomataException.MESSAGE_STATE_NOT_FOUND, transition.To.Name
                );
            }
            if (!ContainsSymbol(transition.Input)) {
                throw new AutomataException(
                    AutomataException.MESSAGE_SYMBOL_NOT_FOUND, transition.Input
                );
            }
            if (ContainsTransition(transition.From, transition.Input, transition.To)) {
                throw new AutomataException(
                    AutomataException.MESSAGE_DUPLICATE_TRANSITION, transition.ToString()
                );
            }

            this.Transitions.Add(transition);
        }

        public void AddTransition(State stateFrom, String input, State stateTo) {
            AddTransition(new Transition(
                stateFrom, input, stateTo
            ));
        }

        public void AddTransition(String stateFrom, String input, String stateTo) {
            if (!ContainsStateName(stateFrom)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, stateFrom);
            }
            if (!ContainsStateName(stateTo)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, stateTo);
            }

            AddTransition(
                this.States.Find(
                    x => x.Name == stateFrom
                ), input,
                this.States.Find(
                    x => x.Name == stateTo
                )
            );
        }

        public void RemoveTransition(Transition transition, Boolean removeDependencies = false) {
            this.Transitions.Remove(transition);

            if (removeDependencies) {
                this.RemoveStates(new State[] {
                    transition.From,
                    transition.To
                }, false);

                this.RemoveSymbol(transition.Input, false);
            }
        }

        public void RemoveTransition(State fromState, String input, State toState, Boolean removeDependencies = false) {
            RemoveTransition(this.Transitions.Find(
                x => x.From == fromState && x.Input == input && x.To == toState
            ), removeDependencies);
        }

        public Boolean ContainsTransition(State fromState, String input, State toState) {
            return this.Transitions.Exists(
                x => x.From == fromState && x.Input == input && toState == x.To
            );
        }

        public Boolean ContainsTransition(Transition transition) {
            return this.Transitions.Exists(
                x => x == transition
            );
        }

        public void ClearTransitions(Boolean removeDependencies = false) {
            this.Transitions.Clear();
        }

        #endregion
    }
}
