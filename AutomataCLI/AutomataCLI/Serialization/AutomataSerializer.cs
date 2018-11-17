using System;
using System.Collections.Generic;
using System.Linq;
using AutomataCLI.Extensions;
using AutomataCLI.Struct;

namespace AutomataCLI.Serialization {

    public class AutomataSerializerException : Exception {
        const String MESSAGE_BASE = "Invalid automata. ";
        public AutomataSerializerException(String supplement) : base($"{MESSAGE_BASE}{supplement}") { }
    }

    // TO DO
    public class AutomataSerializer {
        static HashSet<String> ValidAutomataTypes = new HashSet<String>();

        const Char COMMA_SEPARATOR_CHAR = ',';
        const Char NEWLINE_CHAR = '\n';
        const String EOF_STR = "####";

        static AutomataSerializer() {
            Enum.GetNames(typeof(AutomataType)).ToList().ForEach(
                x => ValidAutomataTypes.Add(x)
            );
        }

        public static Automata Deserialize(String plainText, Boolean displayDebug = false) {
            Boolean abandon = false;
            // Char newLine = '\n';
            // Char commaSeparator = ',';
            String[] textLines = plainText.Split(NEWLINE_CHAR);
            Automata deserializedAutomata = new Automata();

            if (textLines.Length >= 7) {
                String type = textLines[0].Trim();
                List<String> states = textLines[1].Split(COMMA_SEPARATOR_CHAR).ToList();
                List<String> symbols = textLines[2].Split(COMMA_SEPARATOR_CHAR).ToList();
                String initialState = textLines[3].Trim();
                List<String> finalStates = textLines[4].Split(COMMA_SEPARATOR_CHAR).ToList();
                List<String> transitions = new List<String>();

                for (Int32 i = 5; i < textLines.Length - 1; i++) {
                    if (!String.IsNullOrWhiteSpace(textLines[i]) && textLines[i] != EOF_STR) {
                        transitions.Add(textLines[i]);
                    }
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

                symbols.ForEach(
                    x => {
                        try {
                            deserializedAutomata.AddSymbol(x);
                            if (displayDebug) {
                                Console.WriteLine($"Symbol \"{x}\" loaded.");
                            }
                        } catch (Exception e) {
                            abandon = !HandleException(e, x, "Symbol");
                            if (abandon) {
                                throw new AutomataSerializerException("Serialization was abandoned.");
                            }
                        }
                    }
                );

                states.ForEach(
                    x => {
                        try {
                            deserializedAutomata.AddState(DeserializeState(x, finalStates.ToArray()));
                            if (displayDebug) {
                                Console.WriteLine($"State \"{x}\" loaded.");
                            }
                        } catch (Exception e) {
                            abandon = !HandleException(e, x, "State");
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
                            if (displayDebug) {
                                Console.WriteLine($"Transition \"{x}\" loaded.");
                            }
                        } catch (Exception e) {
                            abandon = !HandleException(e, x, "Transition");
                            if (abandon) {
                                throw new AutomataSerializerException("Serialization was abandoned.");
                            }
                        }
                    }
                );

                if (deserializedAutomata.ContainsState(initialState)) {
                    deserializedAutomata.SetInitialState(
                        deserializedAutomata.GetStates().ToList().Find(
                            x => x.Name == initialState
                        )
                    );
                    if (displayDebug) {
                        Console.WriteLine($"State \"{initialState}\" set as initializer.");
                    }
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
            String serializedAutomata = "";

            automata.GetStates().ToList().ForEach(
                x => serializedAutomata += SerializeState(x) + COMMA_SEPARATOR_CHAR
            );
            serializedAutomata += NEWLINE_CHAR;

            automata.GetSymbols().ToList().ForEach(
                x => serializedAutomata += x + COMMA_SEPARATOR_CHAR
            );
            serializedAutomata += NEWLINE_CHAR;

            serializedAutomata += SerializeState(automata.GetInitialState()) + NEWLINE_CHAR;

            automata.GetFinalStates().ToList().ForEach(
                x => serializedAutomata += SerializeState(x) + COMMA_SEPARATOR_CHAR
            );
            serializedAutomata += NEWLINE_CHAR;

            automata.GetTransitions().ToList().ForEach(
                x => serializedAutomata += SerializeTransition(x) + NEWLINE_CHAR
            );
            serializedAutomata += EOF_STR;

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

        private static State DeserializeState(String plainText, String[] finalStates) {
            return new State(plainText, finalStates.Contains(plainText));
        }

        private static String SerializeState(State state) {
            return state.Name;
        }

        private static Transition DeserializeTransition(String plainText, Automata automata) {
            String[] transitionMembers = plainText.Replace("(", "").Replace(")", "").Split(',');

            if (transitionMembers.Length != 3) {
                throw new AutomataSerializerException($"Invalid transition: {plainText}");
            }

            String stateFrom = transitionMembers[0].Trim();
            String input     = transitionMembers[1].Trim();
            String stateTo   = transitionMembers[2].Trim();

            automata.EnsureContainsSymbol(input);
            automata.EnsureContainsState(stateFrom);
            automata.EnsureContainsState(stateTo);

            return new Transition(
                automata.GetStateLike(stateFrom),
                input,
                automata.GetStateLike(stateTo)
            );
        }

        private static String SerializeTransition(Transition transition) {
            return transition.ToString();
        }
    }
}