using System;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Struct;

namespace AutomataCLI.Serialization {

    public class AutomataSerializerException : Exception {
        const String MESSAGE_BASE = "Invalid automata. ";
        public AutomataSerializerException(String supplement) : base($"{MESSAGE_BASE}{supplement}") { }
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
            Boolean abandon = false;
            Char newLine = '\n';
            Char commaSeparator = ',';
            String[] textLines = plainText.Split(newLine);
            Automata deserializedAutomata = new Automata();

            if (textLines.Length >= 7) {
                String type = textLines[0];
                List<String> states = textLines[1].Split(commaSeparator).ToList();
                List<String> symbols = textLines[2].Split(commaSeparator).ToList();
                String initialState = textLines[3].Trim();
                List<String> finalStates = textLines[4].Split(commaSeparator).ToList();
                List<String> transitions = new List<String>();

                for (Int32 i = 5; i < textLines.Length; i++) {
                    transitions.Add(textLines[i]);
                }

                states.ForEach(
                    x => x = x.Trim()
                );
                symbols.ForEach(
                    x => x = x.Trim()
                );
                finalStates.ForEach(
                    x => x = x.Trim()
                );
                transitions.ForEach(
                    x => x = x.Trim()
                );

                deserializedAutomata.SetAutomataType(DeserializeType(type));

                states.ForEach(
                    x => {
                        try {
                            deserializedAutomata.AddState(DeserializeState(x));
                        } catch (Exception e) {
                            abandon = !HandleException(e, x, "State");
                            if (abandon) {
                                throw new AutomataSerializerException("Serialization was abandoned.");
                            }
                        }
                    }
                );

                symbols.ForEach(
                    x => {
                        try {
                            deserializedAutomata.AddSymbol(x);
                        } catch (Exception e) {
                            abandon = !HandleException(e, x, "Symbol");
                            if (abandon) {
                                throw new AutomataSerializerException("Serialization was abandoned.");
                            }
                        }
                    }
                );

                transitions.ForEach(
                    x => {
                        try {
                            deserializedAutomata.AddTransition(DeserializeTransition(x, deserializedAutomata));
                        } catch (Exception e) {
                            abandon = !HandleException(e, x, "Transition");
                            if (abandon) {
                                throw new AutomataSerializerException("Serialization was abandoned.");
                            }
                        }
                    }
                );

                if (deserializedAutomata.ContainsStateName(initialState)) {
                    deserializedAutomata.SetInitialState(
                        deserializedAutomata.GetStates().ToList().Find(
                            x => x.Name == initialState
                        )
                    );
                } else {
                    throw new AutomataSerializerException($"Initial state \"{initialState}\" is invalid.");
                }

            } else {
                throw new AutomataSerializerException("Not enough arguments.");
            }

            return deserializedAutomata;
        }

        protected static Boolean HandleException(Exception e, String memberName, String memberType) {
            Console.WriteLine($"Error: {e.Message}.");
            Console.WriteLine($"Would you like to skip this error? ({memberType} \"{memberName}\" will be ignored)");
            Console.WriteLine($"Type \"Y\" to continue:");
            return Console.ReadLine().ToUpper() == "Y";
        }

        public static String Serialize(Automata automata) {
            String commaSeparator = ", ";
            String newLine = Environment.NewLine;
            String EOF = "####";
            String serializedAutomata = "";

            automata.GetStates().ToList().ForEach(
                x => serializedAutomata += SerializeState(x) + commaSeparator
            );
            serializedAutomata += newLine;

            automata.GetSymbols().ToList().ForEach(
                x => serializedAutomata += x + commaSeparator
            );
            serializedAutomata += newLine;

            serializedAutomata += SerializeState(automata.GetInitialState()) + newLine;

            automata.GetFinalStates().ToList().ForEach(
                x => serializedAutomata += SerializeState(x) + commaSeparator
            );
            serializedAutomata += newLine;

            automata.GetTransitions().ToList().ForEach(
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

        private static Transition DeserializeTransition(String plainText, Automata automata) {
            String[] transitionMembers = plainText.Replace("(", "").Replace(")", "").Split(',');

            if (transitionMembers.Length != 3) {
                throw new AutomataSerializerException($"Invalid transition: {plainText}");
            }

            return new Transition(
                automata.GetStateLike(transitionMembers[0]), // stateFrom
                transitionMembers[1],                        // input
                automata.GetStateLike(transitionMembers[2])  // stateTo
            );
        }

        private static String SerializeTransition(Transition transition) {
            return transition.ToString();
        }
    }
}