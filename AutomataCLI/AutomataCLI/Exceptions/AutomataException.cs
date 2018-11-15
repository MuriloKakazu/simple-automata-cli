using System;

namespace AutomataCLI.Exceptions {
    public class AutomataException : Exception {
        protected static String DEFAULT_TITLE;

        static AutomataException() {
            DEFAULT_TITLE = "Invalid automata";
            // MESSAGE_BASE = "Invalid automata.";
            // MESSAGE_INVALID_STATE = "Invalid state:";
            // MESSAGE_INVALID_TRANSITION = "Invalid transition:";
            // MESSAGE_INVALID_SYMBOL = "Invalid symbol:";
            // MESSAGE_DUPLICATE_STATE = "Duplicate state found:";
            // MESSAGE_DUPLICATE_TRANSITION = "Duplicate transition found:";
            // MESSAGE_STATE_NOT_FOUND = "Could not find state:";
            // MESSAGE_SYMBOL_NOT_FOUND = "Could not find symbol:";
            // MESSAGE_TRANSITION_NOT_FOUND = "Could not find transition:";
            // SUPPLEMENT_VALUE_IS_NULL = "Value is null.";
        }

        static String FormatException(String title, Object supplement, Type targetType) {
            if (String.IsNullOrWhiteSpace(title)) {
                title = DEFAULT_TITLE;
            }

            if (supplement == null) {
                supplement = "null";
            }

            if (targetType == null) {
                targetType = supplement?.GetType();
            }

            supplement = $"{supplement?.ToString()} ({targetType?.Name}).";
            return $"{title}{(String.IsNullOrWhiteSpace(supplement?.ToString()) ? "." : $": {supplement}.")}";
        }

        public AutomataException(String title = "", Object supplement = null, Type targetType = null) :
            base(FormatException(title, supplement, targetType)) {}
    }
}