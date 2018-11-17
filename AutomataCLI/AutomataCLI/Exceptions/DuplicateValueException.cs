using System;

namespace AutomataCLI.Exceptions {
    public class DuplicateValueException : AutomataException {

        static DuplicateValueException() {
            DEFAULT_TITLE = "Duplicate value";
        }

        public DuplicateValueException(Object supplement = null, Type valueType = null) :
            base(DEFAULT_TITLE, supplement, valueType) { }
    }
}