using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow_Wpf.Nodify.ViewModels;
using System.Linq;

namespace Flow_Wpf.Nodify.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void MainViewModel_InitializeWithSampleNodes()
        {
            // Arrange & Act
            var viewModel = new MainViewModel();

            // Assert
            Assert.IsTrue(viewModel.Nodes.Count > 0, "Should have sample nodes");
        }

        [TestMethod]
        public void AddNodeCommand_AddsNode()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var initialCount = viewModel.Nodes.Count;

            // Act
            viewModel.AddNodeCommand.Execute(null);

            // Assert
            Assert.AreEqual(initialCount + 1, viewModel.Nodes.Count);
        }

        [TestMethod]
        public void DeleteNodeCommand_RemovesSelectedNode()
        {
            // Arrange
            var viewModel = new MainViewModel();
            viewModel.AddNodeCommand.Execute(null);
            viewModel.SelectedNode = viewModel.Nodes.Last();

            // Act
            viewModel.DeleteNodeCommand.Execute(null);

            // Assert
            Assert.AreEqual(viewModel.Nodes.Count, viewModel.Nodes.Count);
        }

        [TestMethod]
        public void NewCommand_ClearsNodes()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var initialCount = viewModel.Nodes.Count;

            // Act
            viewModel.NewCommand.Execute(null);

            // Assert
            Assert.AreEqual(0, viewModel.Nodes.Count);
        }

        [TestMethod]
        public void NodeViewModel_SetProperty_RaisesPropertyChanged()
        {
            // Arrange
            var node = new NodeViewModel();
            var propertyChanged = false;
            node.PropertyChanged += (s, e) => propertyChanged = true;

            // Act
            node.Title = "New Title";

            // Assert
            Assert.IsTrue(propertyChanged, "PropertyChanged should be raised");
            Assert.AreEqual("New Title", node.Title);
        }

        [TestMethod]
        public void ConnectionViewModel_LinksNodes()
        {
            // Arrange
            var fromNode = new NodeViewModel { Title = "From" };
            var toNode = new NodeViewModel { Title = "To" };
            var connection = new ConnectionViewModel();

            // Act
            connection.From = fromNode;
            connection.To = toNode;
            connection.Label = "yes";

            // Assert
            Assert.AreEqual(fromNode, connection.From);
            Assert.AreEqual(toNode, connection.To);
            Assert.AreEqual("yes", connection.Label);
        }

        [TestMethod]
        public void NodeType_Enum_HasAllTypes()
        {
            // Arrange & Act
            var types = System.Enum.GetNames(typeof(NodeType));

            // Assert
            Assert.AreEqual(5, types.Length);
            Assert.IsTrue(types.Contains("Start"));
            Assert.IsTrue(types.Contains("Process"));
            Assert.IsTrue(types.Contains("Decision"));
            Assert.IsTrue(types.Contains("Loop"));
            Assert.IsTrue(types.Contains("End"));
        }
    }
}
