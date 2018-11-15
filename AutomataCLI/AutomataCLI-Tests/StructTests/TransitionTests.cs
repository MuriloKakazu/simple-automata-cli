using System;
using AutomataCLI.Exceptions;
using AutomataCLI.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataCLI_Tests.StructTests {
    [TestClass]
    public class TransitionTests {
        [TestMethod]
        public void TestCreateValidTransition() {
            State  stateFrom = new State("sampleState0");
            State  stateTo   = new State("sampleState1");
            String input     = "@";

            Transition transition = new Transition(
                stateFrom, input, stateTo
            );

            Assert.AreEqual(stateFrom, transition.From);
            Assert.AreEqual(stateTo,   transition.To);
            Assert.AreEqual(input,     transition.Input);
        }

        [TestMethod]
        public void TestCreateValidTransition_SameStates() {
            State stateFrom = new State("sampleState0");
            State stateTo   = stateFrom;
            String input    = "@";

            Transition transition = new Transition(
                stateFrom, input, stateTo
            );

            Assert.AreEqual(transition.From, transition.To);
            Assert.AreEqual(stateFrom, transition.From);
            Assert.AreEqual(stateTo, transition.To);
            Assert.AreEqual(input, transition.Input);
        }

        [TestMethod]
        [ExpectedException(typeof(AutomataException))]
        public void TestCreateInvalidTransition_FromStateNull() {
            State  stateTo   = new State("sampleState1");
            String input     = "@";

            Transition transition = new Transition(
                null, input, stateTo
            );
        }

        [TestMethod]
        [ExpectedException(typeof(AutomataException))]
        public void TestCreateInvalidTransition_ToStateNull() {
            State  stateFrom = new State("sampleState0");
            String input     = "@";

            Transition transition = new Transition(
                stateFrom, input, null
            );
        }

        [TestMethod]
        [ExpectedException(typeof(AutomataException))]
        public void TestCreateInvalidTransition_InputNull() {
            State stateFrom = new State("sampleState0");
            State stateTo   = new State("sampleState1");

            Transition transition = new Transition(
                stateFrom, null, stateTo
            );
        }
    }
}
