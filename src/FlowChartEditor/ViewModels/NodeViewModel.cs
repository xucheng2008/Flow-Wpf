using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using FlowChartEditor.Models;

namespace FlowChartEditor.ViewModels
{
    /// <summary>
    /// 节点视图模型，用于 MVVM 模式中的节点数据包装
    /// </summary>
    public partial class NodeViewModel : ObservableObject
    {
        /// <summary>
        /// 节点唯一标识符
        /// </summary>
        [ObservableProperty]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// 节点类型
        /// </summary>
        [ObservableProperty]
        private NodeType _type;

        /// <summary>
        /// X 坐标位置
        /// </summary>
        [ObservableProperty]
        private double _x;

        /// <summary>
        /// Y 坐标位置
        /// </summary>
        [ObservableProperty]
        private double _y;

        /// <summary>
        /// 节点宽度
        /// </summary>
        [ObservableProperty]
        private double _width = 120;

        /// <summary>
        /// 节点高度
        /// </summary>
        [ObservableProperty]
        private double _height = 60;

        /// <summary>
        /// 节点标签文本
        /// </summary>
        [ObservableProperty]
        private string _label = string.Empty;

        /// <summary>
        /// 条件文本（适用于决策节点）
        /// </summary>
        [ObservableProperty]
        private string _condition = string.Empty;

        /// <summary>
        /// 填充颜色
        /// </summary>
        [ObservableProperty]
        private Brush _fillColor = Brushes.LightBlue;

        /// <summary>
        /// 是否被选中
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        /// <summary>
        /// 节点名称
        /// </summary>
        [ObservableProperty]
        private string _name = string.Empty;

        /// <summary>
        /// 构造函数，初始化默认值
        /// </summary>
        public NodeViewModel()
        {
            _id = Guid.NewGuid();
            _width = 120;
            _height = 60;
            _fillColor = Brushes.LightBlue;
        }

        /// <summary>
        /// 使用指定参数创建节点视图模型
        /// </summary>
        /// <param name="type">节点类型</param>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        public NodeViewModel(NodeType type, double x, double y)
        {
            _id = Guid.NewGuid();
            _type = type;
            _x = x;
            _y = y;
            _width = 120;
            _height = 60;
            _fillColor = type switch
            {
                NodeType.Rectangle => Brushes.LightBlue,
                NodeType.Diamond => Brushes.LightYellow,
                NodeType.Circle => Brushes.LightGreen,
                NodeType.RoundedRectangle => Brushes.LightSalmon,
                _ => Brushes.LightGray
            };
            _name = type.ToString();
            _label = type.ToString();
        }
    }
}
