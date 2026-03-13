using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using FlowChartEditor.Models;

namespace FlowChartEditor.Converters
{
    /// <summary>
    /// 将节点类型转换为对应的几何形状
    /// 用于在视图中绘制不同形状的节点
    /// </summary>
    public class NodeTypeToShapeConverter : IValueConverter
    {
        /// <summary>
        /// 转换节点类型为几何形状
        /// </summary>
        /// <param name="value">节点类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>Geometry 对象</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not NodeType nodeType)
                return Geometry.Empty;

            return nodeType switch
            {
                // 矩形 - 处理步骤
                NodeType.Rectangle => Geometry.Parse("M0,0 L120,0 L120,60 L0,60 Z"),

                // 菱形 - 判断条件
                NodeType.Diamond => Geometry.Parse("M60,0 L120,30 L60,60 L0,30 Z"),

                // 圆形 - 开始/结束
                NodeType.Circle => new EllipseGeometry(new Point(60, 30), 30, 30),

                // 圆角矩形 - 输入/输出
                NodeType.RoundedRectangle => CreateRoundedRectangle(0, 0, 120, 60, 10, 10),

                _ => Geometry.Parse("M0,0 L120,0 L120,60 L0,60 Z")
            };
        }

        /// <summary>
        /// 反向转换（不支持）
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建圆角矩形几何
        /// </summary>
        private static Geometry CreateRoundedRectangle(double x, double y, double width, double height, double radiusX, double radiusY)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                // 起点：左上角（圆角后）
                context.BeginFigure(new Point(x + radiusX, y), false, false);

                // 上边
                context.LineTo(new Point(x + width - radiusX, y), true, false);

                // 右上角圆角
                context.QuadraticBezierTo(
                    new Point(x + width, y),
                    new Point(x + width, y + radiusY),
                    true, true);

                // 右边
                context.LineTo(new Point(x + width, y + height - radiusY), true, false);

                // 右下角圆角
                context.QuadraticBezierTo(
                    new Point(x + width, y + height),
                    new Point(x + width - radiusX, y + height),
                    true, true);

                // 下边
                context.LineTo(new Point(x + radiusX, y + height), true, false);

                // 左下角圆角
                context.QuadraticBezierTo(
                    new Point(x, y + height),
                    new Point(x, y + height - radiusY),
                    true, true);

                // 左边
                context.LineTo(new Point(x, y + radiusY), true, false);

                // 左上角圆角
                context.QuadraticBezierTo(
                    new Point(x, y),
                    new Point(x + radiusX, y),
                    true, true);

                context.Close();
            }
            geometry.Freeze();
            return geometry;
        }
    }
}
