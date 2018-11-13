using System;

namespace AutomataCLI.Exceptions {
    public class AutomataException : Exception {
        private static String MESSAGE_BASE;
        public  static String MESSAGE_INVALID_STATE;
        public  static String MESSAGE_INVALID_TRANSITION;
        public  static String MESSAGE_DUPLICATE_STATE;
        public  static String MESSAGE_DUPLICATE_TRANSITION;
        public  static String MESSAGE_STATE_NOT_FOUND;
        public  static String MESSAGE_SYMBOL_NOT_FOUND;
        public  static String MESSAGE_TRANSITION_NOT_FOUND;

        // static ctor
        static AutomataException() {
            MESSAGE_BASE = "Invalid automata.";
            MESSAGE_INVALID_STATE = "Invalid state:";
            MESSAGE_INVALID_TRANSITION = "Invalid transition:";
            MESSAGE_DUPLICATE_STATE = "Duplicate state found:";
            MESSAGE_DUPLICATE_TRANSITION = "Duplicate transition found:";
            MESSAGE_STATE_NOT_FOUND = "Could not find state:";
            MESSAGE_SYMBOL_NOT_FOUND = "Could not find symbol:";
            MESSAGE_TRANSITION_NOT_FOUND = "Could not find transition:";
        }

        // ctor
        public AutomataException(String reason, String supplement = "") :
            base($"{MESSAGE_BASE} {reason} {supplement}") { }
    }
}