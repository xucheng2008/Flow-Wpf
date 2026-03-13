using CommunityToolkit.Mvvm.ComponentModel;
using System;
using FlowChartEditor.Models;

namespace FlowChartEditor.ViewModels
{
    /// <summary>
    /// 连接视图模型，表示两个节点之间的连接
    /// </summary>
    public partial class ConnectionViewModel : ObservableObject
    {
        /// <summary>
        /// 连接的唯一标识符
        /// </summary>
        [ObservableProperty]
        private Guid _id;

        /// <summary>
        /// 源节点 ID
        /// </summary>
        [ObservableProperty]
        private Guid _sourceNodeId;

        /// <summary>
        /// 目标节点 ID
        /// </summary>
        [ObservableProperty]
        private Guid _targetNodeId;

        /// <summary>
        /// 源端点名称
        /// </summary>
        [ObservableProperty]
        private string _sourcePointName = string.Empty;

        /// <summary>
        /// 目标端点名称
        /// </summary>
        [ObservableProperty]
        private string _targetPointName = string.Empty;

        /// <summary>
        /// 连接类型
        /// </summary>
        [ObservableProperty]
        private ConnectionType _type = ConnectionType.Orthogonal;

        /// <summary>
        /// 连接标签
        /// </summary>
        [ObservableProperty]
        private string _label = string.Empty;

        /// <summary>
        /// 是否被选中
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConnectionViewModel()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// 构造函数，带参数
        /// </summary>
        /// <param name="sourceNodeId">源节点 ID</param>
        /// <param name="targetNodeId">目标节点 ID</param>
        /// <param name="sourcePointName">源端点名称</param>
        /// <param name="targetPointName">目标端点名称</param>
        public ConnectionViewModel(Guid sourceNodeId, Guid targetNodeId, string sourcePointName = "", string targetPointName = "")
        {
            _id = Guid.NewGuid();
            _sourceNodeId = sourceNodeId;
            _targetNodeId = targetNodeId;
            _sourcePointName = sourcePointName;
            _targetPointName = targetPointName;
        }
    }
}
