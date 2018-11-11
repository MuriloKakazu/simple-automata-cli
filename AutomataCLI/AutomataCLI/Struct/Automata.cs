using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Struct {
    public class Automata {
        public class AutomataException : Exception {
            private static String MESSAGE_BASE;
            public static  String MESSAGE_DUPLICATE_STATE;
            public static  String MESSAGE_DUPLICATE_TRANSITION;
            public static  String MESSAGE_STATE_NOT_FOUND;
            public static  String MESSAGE_SYMBOL_NOT_FOUND;
            public static  String MESSAGE_TRANSITION_NOT_FOUND;

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
            public AutomataException(String reason, String supplement = "") :
                base($"{MESSAGE_BASE} {reason} {supplement}") { }
        }

        protected AutomataType AutomataType { get; set; }
        protected List<String> Symbols { get; set; }
        protected List<State> States { get; set; }
        protected List<Transition> Transitions { get; set; }
        protected State InitialState { get; set; }
        protected List<State> FinalStates { get; set; }

        #region automata methods

        // ctor
        public Automata() {
            Symbols     = new List<String>();
            States      = new List<State>();
            Transitions = new List<Transition>();
            FinalStates = new List<State>();
        }

        public void SetAutomataType(AutomataType automataType) {
            AutomataType = automataType;
        }

        public AutomataType GetAutomataType() {
            return AutomataType;
        }

        public void Clear() {
            ClearSymbols(false);
            ClearStates(false);
            ClearTransitions();
        }

        public override String ToString() {
            String commaSeparator = ", ";
            String type           = AutomataType.ToString();
            String states         = "";
            String initialState   = InitialState.ToString();
            String finalStates    = "";
            String transitions    = "";

            States.ForEach(
                x => states += x.ToString() + commaSeparator
            );
            FinalStates.ForEach(
                x => finalStates += x.ToString() + commaSeparator
            );
            Transitions.ForEach(
                x => transitions += x.ToString() + commaSeparator
            );

            return
                $"("                                 +
                    "{type}{commaSeparator}"         +
                    "{states}{commaSeparator}"       +
                    "{initialState}{commaSeparator}" +
                    "{finalStates}{commaSeparator}"  +
                    "{transitions}"                  +
                ")";
        }

        #endregion

        #region symbol methods

        public void AddSymbol(String symbol) {
            Symbols.Add(symbol);
        }

        public void AddSymbols(String[] symbols) {
            symbols.ToList().ForEach(
                x => AddSymbol(x)
            );
        }

        public String[] GetSymbols() {
            return Symbols.ToArray();
        }

        public void RemoveSymbol(String symbol, Boolean removeDependencies = false) {
            if (removeDependencies) {
                RemoveTransitions(Transitions.Where(
                    x => x.Input == symbol
                ).ToArray());
            }

            Symbols.Remove(symbol);
        }

        public void RemoveSymbols(String[] symbols, Boolean removeDependencies = false) {
            symbols.ToList().ForEach(
                x => RemoveSymbol(x, removeDependencies)
            );
        }

        public void SetSymbols(String[] symbols, Boolean removeOldDependencies = false) {
            ClearSymbols(removeOldDependencies);
            AddSymbols(symbols);
        }

        public Boolean ContainsSymbol(String symbol) {
            return Symbols.Exists(
                x => x == symbol
            );
        }

        public void ClearSymbols(Boolean removeDependencies = false) {
            RemoveSymbols(Symbols.ToArray(), removeDependencies);
        }

        #endregion

        #region state methods

        public void AddState(State state) {
            if (ContainsStateName(state.Name)) {
                throw new AutomataException(AutomataException.MESSAGE_DUPLICATE_STATE, state.Name);
            }

            States.Add(state);
            RefreshFinalStates();
        }

        public void AddState(String stateName, Boolean isFinal = false) {
            AddState(new State(stateName, isFinal));
        }

        public void AddStates(State[] states) {
            states.ToList().ForEach(
                x => AddState(x)
            );
        }

        public State[] GetStates() {
            return States.ToArray();
        }

        public State[] GetFinalStates() {
            return FinalStates.ToArray();
        }

        public State GetInitialState() {
            return InitialState;
        }

        public void RemoveState(State state, Boolean removeDependencies = false) {
            if (removeDependencies) {
                RemoveTransitions(Transitions.Where(
                    x => x.From == state || x.To == state
                ).ToArray());
            }

            if (InitialState == state) {
                InitialState = null;
            }

            States.Remove(state);
            RefreshFinalStates();
        }

        public void RemoveState(String stateName, Boolean removeDependencies = false) {
            RemoveState(States.Find(
                x => x.Name == stateName
            ), removeDependencies);
        }

        public void RemoveStates(State[] states, Boolean removeDependencies = false) {
            states.ToList().ForEach(
                x => RemoveState(x, removeDependencies)
            );
        }

        public void RemoveStates(String[] states, Boolean removeDependencies = false) {
            states.ToList().ForEach(
                x => RemoveState(x, removeDependencies)
            );
        }

        public void SetStates(State[] states, Boolean removeOldDependencies = false) {
            ClearStates(removeOldDependencies);
            AddStates(states);
        }

        public Boolean ContainsStateName(String stateName) {
            return States.Exists(
                x => x.Name == stateName
            );
        }

        public Boolean ContainsState(State state) {
            return States.Exists(
                x => x == state
            );
        }

        public void SetInitialState(State state) {
            if (!ContainsState(state)) {
                throw new AutomataException(AutomataException.MESSAGE_STATE_NOT_FOUND, state.Name);
            }

            InitialState = state;
        }

        private void RefreshFinalStates() {
            FinalStates = States.Where(
                x => x.IsFinal
            ).ToList();
        }

        public void ClearStates(Boolean removeDependencies = false) {
            RemoveStates(States.ToArray());
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

            Transitions.Add(transition);
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
                States.Find(
                    x => x.Name == stateFrom
                ), input,
                States.Find(
                    x => x.Name == stateTo
                )
            );
        }

        public void AddTransitions(Transition[] transitions) {
            transitions.ToList().ForEach(
                x => AddTransition(x)
            );
        }

        public void RemoveTransition(Transition transition) {
            Transitions.Remove(transition);
        }

        public void RemoveTransition(State fromState, String input, State toState) {
            RemoveTransition(Transitions.Find(
                x => x.From == fromState && x.Input == input && x.To == toState
            ));
        }

        public void RemoveTransitions(Transition[] transitions) {
            transitions.ToList().ForEach(
                x => RemoveTransition(x)
            );
        }

        public void SetTransitions(Transition[] transitions) {
            ClearTransitions();
            AddTransitions(transitions);
        }

        public Boolean ContainsTransition(State fromState, String input, State toState) {
            return Transitions.Exists(
                x => x.From  == fromState &&
                     x.Input == input     &&
                     x.To    == toState
            );
        }

        public Boolean ContainsTransition(Transition transition) {
            return Transitions.Exists(
                x => x == transition
            );
        }

        public void ClearTransitions() {
            RemoveTransitions(Transitions.ToArray());
        }

        #endregion
    }
}
