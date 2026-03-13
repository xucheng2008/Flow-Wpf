namespace FlowChartEditor.Models;

/// <summary>
/// 节点类型枚举
/// </summary>
public enum NodeType
{
    /// <summary>矩形 - 处理步骤</summary>
    Rectangle,
    
    /// <summary>菱形 - 判断条件</summary>
    Diamond,
    
    /// <summary>圆形 - 开始/结束</summary>
    Circle,
    
    /// <summary>圆角矩形 - 输入/输出</summary>
    RoundedRectangle
}

/// <summary>
/// 连接线类型
/// </summary>
public enum ConnectionType
{
    /// <summary>贝塞尔曲线</summary>
    Bezier,
    
    /// <summary>正交折线（直角）</summary>
    Orthogonal,
    
    /// <summary>直线</summary>
    Straight
}

/// <summary>
/// 连接点类型
/// </summary>
public enum ConnectionPointType
{
    /// <summary>输入点</summary>
    Input,
    
    /// <summary>输出点</summary>
    Output,
    
    /// <summary>双向点</summary>
    Both
}
