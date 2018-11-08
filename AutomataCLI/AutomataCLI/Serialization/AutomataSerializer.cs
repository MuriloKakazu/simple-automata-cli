using System;
using AutomataCLI.Struct;

namespace AutomataCLI.Serialization {
    // TO DO
    public class AutomataSerializer {
        public static Automata Deserialize(String plainText) {
            return null;
        }

        public static String Serialize(Automata automata) {
            String newLine = Environment.NewLine;
            String EOF = "####";
            String serializedAutomata = "";

            automata.States.ForEach(
                x => serializedAutomata += SerializeState(x)
            );
            serializedAutomata += newLine;

            automata.Symbols.ForEach(
                x => serializedAutomata += x
            );
            serializedAutomata += newLine;

            serializedAutomata += SerializeState(automata.InitialState) + newLine;

            automata.FinalStates.ForEach(
                x => serializedAutomata += SerializeState(x)
            );
            serializedAutomata += newLine;

            automata.Transitions.ForEach(
                x => serializedAutomata += SerializeTransition(x) + newLine
            );
            serializedAutomata += EOF;

            return serializedAutomata;
        }

        private static State DeserializeState(String plainText) {
            return null;
        }

        private static String SerializeState(State state) {
            return state.Name;
        }

        private static Transition DeserializeTransition(String plainText) {
            return null;
        }

        private static String SerializeTransition(Transition transition) {
            return transition.ToString();
        }
    }
}