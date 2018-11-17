using System;

namespace AutomataCLI.Exceptions {
    public class InvalidValueException : AutomataException {

        static InvalidValueException() {
            DEFAULT_TITLE = "Invalid value";
        }

        public InvalidValueException(Object supplement = null, Type valueType = null) :
            base(DEFAULT_TITLE, supplement, valueType) { }
    }
}