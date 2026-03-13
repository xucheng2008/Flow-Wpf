using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlowChartEditor.Controls
{
    /// <summary>
    /// 箭头控件，用于连接线末端
    /// </summary>
    public class ArrowHead : Control
    {
        static ArrowHead()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ArrowHead),
                new FrameworkPropertyMetadata(typeof(ArrowHead)));
        }

        #region Dependency Properties

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(ArrowHead),
                new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(double), typeof(ArrowHead),
                new PropertyMetadata(10.0));

        #endregion

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        /// <summary>
        /// 创建箭头几何形状
        /// </summary>
        public static Geometry CreateArrowGeometry(double size = 10)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                // 箭头形状：三角形
                context.BeginFigure(new Point(0, 0), true, true);
                context.LineTo(new Point(size, size / 2), true, false);
                context.LineTo(new Point(0, size), true, false);
                context.Close();
            }
            geometry.Freeze();
            return geometry;
        }

        /// <summary>
        /// 创建带有角度的箭头
        /// </summary>
        public static Path CreateArrowPath(Point start, Point end, double size = 10, Brush? fill = null)
        {
            var arrow = new Path
            {
                Fill = fill ?? Brushes.Black,
                Width = size,
                Height = size
            };

            // 计算旋转角度
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X) * 180 / Math.PI;
            arrow.RenderTransform = new RotateTransform(angle, size / 2, size / 2);
            arrow.Data = CreateArrowGeometry(size);

            return arrow;
        }
    }

    /// <summary>
    /// 箭头辅助类
    /// </summary>
    public static class ArrowHelper
    {
        /// <summary>
        /// 在指定位置创建箭头
        /// </summary>
        public static Path CreateArrow(Point position, double angle, double size = 10, Brush? fill = null)
        {
            var arrow = new Path
            {
                Fill = fill ?? Brushes.Black,
                Data = ArrowHead.CreateArrowGeometry(size),
                Width = size,
                Height = size
            };

            Canvas.SetLeft(arrow, position.X - size / 2);
            Canvas.SetTop(arrow, position.Y - size / 2);

            arrow.RenderTransform = new RotateTransform(angle, size / 2, size / 2);
            arrow.RenderTransformOrigin = new Point(0.5, 0.5);

            return arrow;
        }

        /// <summary>
        /// 计算线段末端的角度
        /// </summary>
        public static double CalculateAngle(Point start, Point end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X) * 180 / Math.PI;
        }
    }
}
