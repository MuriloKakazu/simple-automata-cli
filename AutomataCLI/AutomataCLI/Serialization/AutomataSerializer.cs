using System;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Struct;

namespace AutomataCLI.Serialization {

    public class AutomataSerializerException : Exception {
        const String EXCEPTION_MESSAGE = "Invalid Automata. ";
        public AutomataSerializerException(String reason) : base($"{EXCEPTION_MESSAGE}{reason}") { }
    }

    // TO DO
    public class AutomataSerializer {
        static HashSet<String> ValidAutomataTypes = new HashSet<String>();

        static AutomataSerializer() {
            Enum.GetNames(typeof(AutomataType)).ToList().ForEach(
                x => ValidAutomataTypes.Add(x)
            );
        }

        public static Automata Deserialize(String plainText) {
            Char newLine = '\n';
            Char commaSeparator = ',';
            String[] textLines = plainText.Split(newLine);
            Automata deserializedAutomata = new Automata();

            if (textLines.Length >= 7) {
                String type = textLines[0];
                List<String> states = textLines[1].Split(commaSeparator).ToList();
                List<String> symbols = textLines[2].Split(commaSeparator).ToList();
                String initialState = textLines[3];
                List<String> finalStates = textLines[4].Split(commaSeparator).ToList();

                deserializedAutomata.SetType(DeserializeType(type));

                states.ForEach(
                    x => {
                        if (!deserializedAutomata.ContainsState(x)) {
                            deserializedAutomata.AddState(DeserializeState(x));
                        } else {
                            throw new AutomataSerializerException($"State \"{x}\" is defined more than once.");
                        }
                    }
                );

                symbols.ForEach(
                    x => deserializedAutomata.AddSymbol(x)
                );

                if (deserializedAutomata.ContainsState(initialState)) {
                    deserializedAutomata.SetInitialState(deserializedAutomata.States.Find(x => x.Name == initialState));
                } else {
                    throw new AutomataSerializerException($"Initial state \"{initialState}\" is invalid.");
                }

            } else {
                throw new AutomataSerializerException("Not enough arguments.");
            }

            return deserializedAutomata;
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

        public static String SerializeType(AutomataType type) {
            return type.ToString();
        }

        public static AutomataType DeserializeType(String plainText) {
            if (ValidAutomataTypes.Contains(plainText)) {
                return (AutomataType) Enum.Parse(typeof(AutomataType), plainText);
            }
            else {
                throw new AutomataSerializerException($"Automata type \"{plainText}\" is invalid.");
            }
        }

        private static State DeserializeState(String plainText) {
            return new State(plainText, isFinal: false);
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