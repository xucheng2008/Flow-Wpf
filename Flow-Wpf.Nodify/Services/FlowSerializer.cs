using System.Text.Json;
using System.Text.Json.Serialization;
using Flow_Wpf.Nodify.ViewModels;
using Flow_Wpf.Nodify.Services;

namespace Flow_Wpf.Nodify.Services
{
    /// <summary>
    /// Flow serialization service - save/load flows as JSON
    /// </summary>
    public static class FlowSerializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Serialize MainViewModel to FlowData
        /// </summary>
        public static FlowData ToFlowData(MainViewModel viewModel)
        {
            var flowData = new FlowData
            {
                Version = "1.0",
                Title = "Flow Chart",
                Description = "Created with Flow-Wpf.Nodify",
                Metadata = new FlowMetadata
                {
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                }
            };

            // Convert nodes
            foreach (var node in viewModel.Nodes)
            {
                flowData.Nodes.Add(new NodeData
                {
                    Id = node.Id,
                    NodeType = node.Type.ToString(),
                    X = node.X,
                    Y = node.Y,
                    Title = node.Title,
                    Data = node.Data
                });
            }

            // Convert connections
            foreach (var conn in viewModel.Connections)
            {
                flowData.Connections.Add(new ConnectionData
                {
                    Id = conn.Id,
                    FromNodeId = conn.From?.Id ?? "",
                    ToNodeId = conn.To?.Id ?? "",
                    Label = conn.Label
                });
            }

            return flowData;
        }

        /// <summary>
        /// Deserialize FlowData to MainViewModel
        /// </summary>
        public static MainViewModel FromFlowData(FlowData flowData)
        {
            var viewModel = new MainViewModel();
            viewModel.Nodes.Clear();
            viewModel.Connections.Clear();

            // Create node lookup dictionary
            var nodeLookup = new Dictionary<string, NodeViewModel>();

            // Convert nodes
            foreach (var nodeData in flowData.Nodes)
            {
                var node = new NodeViewModel
                {
                    Id = nodeData.Id,
                    X = nodeData.X,
                    Y = nodeData.Y,
                    Title = nodeData.Title,
                    Data = nodeData.Data
                };

                // Parse node type
                if (Enum.TryParse<NodeType>(nodeData.NodeType, out var nodeType))
                {
                    node.Type = nodeType;
                }

                viewModel.Nodes.Add(node);
                nodeLookup[node.Id] = node;
            }

            // Convert connections
            foreach (var connData in flowData.Connections)
            {
                if (nodeLookup.TryGetValue(connData.FromNodeId, out var fromNode) &&
                    nodeLookup.TryGetValue(connData.ToNodeId, out var toNode))
                {
                    viewModel.Connections.Add(new ConnectionViewModel
                    {
                        Id = connData.Id,
                        From = fromNode,
                        To = toNode,
                        Label = connData.Label
                    });
                }
            }

            return viewModel;
        }

        /// <summary>
        /// Save flow to JSON file
        /// </summary>
        public static async Task SaveAsync(FlowData flowData, string filePath)
        {
            var json = JsonSerializer.Serialize(flowData, JsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Load flow from JSON file
        /// </summary>
        public static async Task<FlowData> LoadAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var flowData = JsonSerializer.Deserialize<FlowData>(json, JsonOptions);
            return flowData ?? throw new InvalidOperationException("Failed to deserialize flow data");
        }

        /// <summary>
        /// Save ViewModel to file
        /// </summary>
        public static async Task SaveViewModelAsync(MainViewModel viewModel, string filePath)
        {
            var flowData = ToFlowData(viewModel);
            flowData.Metadata!.Modified = DateTime.Now;
            await SaveAsync(flowData, filePath);
        }

        /// <summary>
        /// Load ViewModel from file
        /// </summary>
        public static async Task<MainViewModel> LoadViewModelAsync(string filePath)
        {
            var flowData = await LoadAsync(filePath);
            return FromFlowData(flowData);
        }
    }
}
