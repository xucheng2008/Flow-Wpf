using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlowChartEditor.Models;

/// <summary>
/// 节点数据模型
/// </summary>
public partial class NodeModel : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private NodeType _type;
    
    [ObservableProperty]
    private double _x;
    
    [ObservableProperty]
    private double _y;
    
    [ObservableProperty]
    private double _width = 120;
    
    [ObservableProperty]
    private double _height = 60;
    
    [ObservableProperty]
    private string _label = string.Empty;
    
    [ObservableProperty]
    private string _condition = string.Empty;
    
    [ObservableProperty]
    private Brush _fillColor = Brushes.White;
    
    [ObservableProperty]
    private bool _isSelected;
    
    /// <summary>
    /// 连接点集合
    /// </summary>
    public ObservableCollection<ConnectionPoint> ConnectionPoints { get; set; } = new();
    
    /// <summary>
    /// 获取连接点的实际位置（相对于画布）
    /// </summary>
    public System.Windows.Point GetConnectionPointPosition(ConnectionPoint point)
    {
        return point.Type switch
        {
            ConnectionPointType.Top => new System.Windows.Point(X + Width / 2, Y),
            ConnectionPointType.Bottom => new System.Windows.Point(X + Width / 2, Y + Height),
            ConnectionPointType.Left => new System.Windows.Point(X, Y + Height / 2),
            ConnectionPointType.Right => new System.Windows.Point(X + Width, Y + Height / 2),
            _ => new System.Windows.Point(X + Width * point.X, Y + Height * point.Y)
        };
    }
}

/// <summary>
/// 连接点模型
/// </summary>
public class ConnectionPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ConnectionPointType Type { get; set; }
    public double X { get; set; }  // 归一化位置 (0-1)
    public double Y { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 连接线数据模型
/// </summary>
public partial class ConnectionModel : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [ObservableProperty]
    private Guid _sourceNodeId;
    
    [ObservableProperty]
    private Guid _targetNodeId;
    
    [ObservableProperty]
    private string _sourcePointName = string.Empty;
    
    [ObservableProperty]
    private string _targetPointName = string.Empty;
    
    [ObservableProperty]
    private ConnectionType _type = ConnectionType.Orthogonal;
    
    [ObservableProperty]
    private string _label = string.Empty;
    
    [ObservableProperty]
    private bool _isSelected;
}

/// <summary>
/// 完整流程图数据模型
/// </summary>
public class FlowChartModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ObservableCollection<NodeModel> Nodes { get; set; } = new();
    public ObservableCollection<ConnectionModel> Connections { get; set; } = new();
    public double Zoom { get; set; } = 1.0;
    public double PanX { get; set; } = 0;
    public double PanY { get; set; } = 0;
}
