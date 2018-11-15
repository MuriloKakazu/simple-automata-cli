using System;
using AutomataCLI.Exceptions;
using AutomataCLI.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataCLI_Tests.StructTests {
    [TestClass]
    public class StateTests {
        [TestMethod]
        public void TestCreateValidState() {
            String stateName = "sampleState";
            State state = new State(stateName);

            Assert.AreEqual(stateName, state.Name);
        }

        [TestMethod]
        public void TestCreateValidState_FinalState() {
            String stateName = "sampleState";
            State state = new State(stateName, isFinal: true);

            Assert.AreEqual(true, state.IsFinal);
        }

        [TestMethod]
        [ExpectedException(typeof(AutomataException))]
        public void TestCreateInvalidState() {
            State state = new State(null);
        }
    }
}
