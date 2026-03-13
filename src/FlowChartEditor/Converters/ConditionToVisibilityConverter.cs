using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FlowChartEditor.Models;

namespace FlowChartEditor.Converters
{
    /// <summary>
    /// 根据节点类型显示条件文本
    /// 仅菱形节点（Diamond）显示条件文本
    /// </summary>
    public class ConditionToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将节点类型转换为可见性
        /// </summary>
        /// <param name="value">节点类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>Visibility 枚举值</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeType nodeType)
            {
                // 仅菱形节点显示条件文本
                return nodeType == NodeType.Diamond 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        /// <summary>
        /// 反向转换（不支持）
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
