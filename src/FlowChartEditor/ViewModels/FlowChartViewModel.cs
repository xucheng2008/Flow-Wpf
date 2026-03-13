using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace FlowChartEditor.ViewModels
{
    /// <summary>
    /// 流程图主视图模型，管理所有节点和连接的集合
    /// </summary>
    public partial class FlowChartViewModel : ObservableObject
    {
        /// <summary>
        /// 节点集合
        /// </summary>
        public ObservableCollection<NodeViewModel> Nodes { get; } = new();

        /// <summary>
        /// 连接集合
        /// </summary>
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

        /// <summary>
        /// 当前缩放比例
        /// </summary>
        [ObservableProperty]
        private double _zoom = 1.0;

        /// <summary>
        /// 当前选中的节点
        /// </summary>
        [ObservableProperty]
        private NodeViewModel? _selectedNode;

        /// <summary>
        /// 流程图标题
        /// </summary>
        [ObservableProperty]
        private string _title = "FlowChart";

        /// <summary>
        /// 添加节点命令
        /// </summary>
        [ICommand]
        private void AddNode(NodeType type)
        {
            var node = new NodeViewModel(type, 100 + Nodes.Count * 20, 100 + Nodes.Count * 20)
            {
                Label = $"{type} {Nodes.Count + 1}"
            };
            Nodes.Add(node);
        }

        /// <summary>
        /// 删除选中节点命令
        /// </summary>
        [ICommand]
        private void DeleteSelectedNode()
        {
            if (SelectedNode == null) return;

            // 删除与此节点相关的所有连接
            var relatedConnections = Connections
                .Where(c => c.SourceNodeId == SelectedNode.Id || c.TargetNodeId == SelectedNode.Id)
                .ToList();

            foreach (var conn in relatedConnections)
            {
                Connections.Remove(conn);
            }

            Nodes.Remove(SelectedNode);
            SelectedNode = null;
        }

        /// <summary>
        /// 选择节点命令
        /// </summary>
        [ICommand]
        private void SelectNode(NodeViewModel? node)
        {
            // 取消之前的选中状态
            if (SelectedNode != null)
            {
                SelectedNode.IsSelected = false;
            }

            SelectedNode = node;

            // 设置新的选中状态
            if (SelectedNode != null)
            {
                SelectedNode.IsSelected = true;
            }
        }

        /// <summary>
        /// 添加连接
        /// </summary>
        /// <param name="sourceNode">源节点</param>
        /// <param name="targetNode">目标节点</param>
        /// <param name="label">连接标签</param>
        public void AddConnection(NodeViewModel sourceNode, NodeViewModel targetNode, string label = "")
        {
            var connection = new ConnectionViewModel(sourceNode.Id, targetNode.Id)
            {
                Label = label,
                Type = ConnectionType.Orthogonal
            };
            Connections.Add(connection);
        }

        /// <summary>
        /// 缩放命令
        /// </summary>
        [ICommand]
        private void ZoomIn()
        {
            Zoom = Math.Min(3.0, Zoom + 0.1);
        }

        /// <summary>
        /// 缩小命令
        /// </summary>
        [ICommand]
        private void ZoomOut()
        {
            Zoom = Math.Max(0.5, Zoom - 0.1);
        }

        /// <summary>
        /// 重置缩放
        /// </summary>
        [ICommand]
        private void ResetZoom()
        {
            Zoom = 1.0;
        }
    }
}
