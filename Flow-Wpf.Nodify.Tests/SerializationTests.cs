using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow_Wpf.Nodify.ViewModels;
using Flow_Wpf.Nodify.Services;
using System.IO;
using System.Threading.Tasks;

namespace Flow_Wpf.Nodify.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void FlowSerializer_ToFlowData_ConvertsViewModel()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var initialNodeCount = viewModel.Nodes.Count;

            // Act
            var flowData = FlowSerializer.ToFlowData(viewModel);

            // Assert
            Assert.AreEqual(initialNodeCount, flowData.Nodes.Count);
            Assert.AreEqual("1.0", flowData.Version);
        }

        [TestMethod]
        public void FlowSerializer_FromFlowData_CreatesViewModel()
        {
            // Arrange
            var flowData = new FlowData
            {
                Version = "1.0",
                Title = "Test Flow"
            };
            flowData.Nodes.Add(new NodeData
            {
                Id = "test1",
                NodeType = "Start",
                X = 100,
                Y = 100,
                Title = "Start"
            });

            // Act
            var viewModel = FlowSerializer.FromFlowData(flowData);

            // Assert
            Assert.AreEqual(1, viewModel.Nodes.Count);
            Assert.AreEqual("Start", viewModel.Nodes[0].Title);
        }

        [TestMethod]
        public async Task FlowSerializer_SaveAndLoad_RoundTrip()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".flow";
            try
            {
                var viewModel = new MainViewModel();
                var initialNodeCount = viewModel.Nodes.Count;

                // Act - Save
                await FlowSerializer.SaveViewModelAsync(viewModel, tempFile);

                // Act - Load
                var loadedViewModel = await FlowSerializer.LoadViewModelAsync(tempFile);

                // Assert
                Assert.AreEqual(initialNodeCount, loadedViewModel.Nodes.Count);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void FlowData_Serialization_IncludesMetadata()
        {
            // Arrange
            var flowData = new FlowData
            {
                Title = "Test",
                Description = "Test Description"
            };

            // Act
            flowData.Metadata = new FlowMetadata
            {
                Author = "Test User"
            };

            // Assert
            Assert.AreEqual("Test", flowData.Title);
            Assert.AreEqual("Test Description", flowData.Description);
            Assert.AreEqual("Test User", flowData.Metadata.Author);
        }

        [TestMethod]
        public void ConnectionData_Serialization_PreservesLinks()
        {
            // Arrange
            var flowData = new FlowData();
            flowData.Nodes.Add(new NodeData { Id = "node1", Title = "From" });
            flowData.Nodes.Add(new NodeData { Id = "node2", Title = "To" });
            flowData.Connections.Add(new ConnectionData
            {
                FromNodeId = "node1",
                ToNodeId = "node2",
                Label = "yes"
            });

            // Act
            var viewModel = FlowSerializer.FromFlowData(flowData);

            // Assert
            Assert.AreEqual(1, viewModel.Connections.Count);
            Assert.AreEqual("yes", viewModel.Connections[0].Label);
        }
    }
}
