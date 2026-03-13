using System.Text.Json.Serialization;
using Flow_Wpf.Nodify.ViewModels;

namespace Flow_Wpf.Nodify.Services
{
    /// <summary>
    /// Flow data model for JSON serialization
    /// </summary>
    public class FlowData
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "Untitled Flow";

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("nodes")]
        public List<NodeData> Nodes { get; set; } = new();

        [JsonPropertyName("connections")]
        public List<ConnectionData> Connections { get; set; } = new();

        [JsonPropertyName("metadata")]
        public FlowMetadata? Metadata { get; set; }
    }

    /// <summary>
    /// Serializable node data
    /// </summary>
    public class NodeData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

        [JsonPropertyName("type")]
        public string NodeType { get; set; } = "Process";

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "Node";

        [JsonPropertyName("data")]
        public object? Data { get; set; }
    }

    /// <summary>
    /// Serializable connection data
    /// </summary>
    public class ConnectionData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

        [JsonPropertyName("from")]
        public string FromNodeId { get; set; } = "";

        [JsonPropertyName("to")]
        public string ToNodeId { get; set; } = "";

        [JsonPropertyName("label")]
        public string? Label { get; set; }
    }

    /// <summary>
    /// Flow metadata
    /// </summary>
    public class FlowMetadata
    {
        [JsonPropertyName("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonPropertyName("modified")]
        public DateTime Modified { get; set; } = DateTime.Now;

        [JsonPropertyName("author")]
        public string? Author { get; set; }
    }
}
