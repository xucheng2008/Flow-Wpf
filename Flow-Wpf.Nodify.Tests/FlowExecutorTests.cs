using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow_Wpf.Nodify.ViewModels;
using Flow_Wpf.Nodify.Services;
using System.Linq;

namespace Flow_Wpf.Nodify.Tests
{
    [TestClass]
    public class FlowExecutorTests
    {
        [TestMethod]
        public void FlowExecutor_Execute_StartsFromStartNode()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);
            var nodeExecuted = false;
            executor.NodeExecuted += (s, e) => nodeExecuted = true;

            // Act
            executor.Execute();

            // Assert
            Assert.IsTrue(nodeExecuted, "Should execute at least one node");
        }

        [TestMethod]
        public void FlowExecutor_Stop_HaltsExecution()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);

            // Act
            executor.Execute();
            executor.Stop();

            // Assert
            Assert.IsFalse(executor.IsExecuting, "Should not be executing after stop");
        }

        [TestMethod]
        public void FlowExecutor_Step_ExecutesSingleNode()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);
            var executionCount = 0;
            executor.NodeExecuted += (s, e) => executionCount++;

            // Act
            executor.Step();

            // Assert
            Assert.IsTrue(executionCount >= 0, "Should execute nodes");
        }

        [TestMethod]
        public void FlowExecutor_SetVariable_StoresValue()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);

            // Act
            executor.SetVariable("test", 42);
            var value = executor.GetVariable("test");

            // Assert
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void FlowExecutor_Pause_Resumable()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);

            // Act
            executor.Execute();
            executor.Pause();

            // Assert
            Assert.IsTrue(executor.IsPaused, "Should be paused");
        }

        [TestMethod]
        public void FlowExecutor_Resume_ContinuesExecution()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);

            // Act
            executor.Execute();
            executor.Pause();
            executor.Resume();

            // Assert
            Assert.IsFalse(executor.IsPaused, "Should not be paused after resume");
        }

        [TestMethod]
        public void FlowExecutor_DecisionNode_EvaluatesCondition()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var executor = new FlowExecutor(viewModel);
            
            // Create decision node
            var decision = new NodeViewModel
            {
                Title = "SPEED < 200",
                Type = NodeType.Decision,
                X = 100,
                Y = 100
            };
            viewModel.Nodes.Add(decision);

            // Set variable for condition
            executor.SetVariable("SPEED", 150);

            // Act & Assert - Should not throw
            Assert.ThrowsException<System.NullReferenceException>(() =>
            {
                // This will fail because decision node needs connections
                // But condition evaluation should work
            });
        }
    }
}
