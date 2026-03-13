using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FlowChartEditor.Models;
using FlowChartEditor.Services;
using FlowChartEditor.ViewModels;

namespace FlowChartEditor.Views
{
    /// <summary>
    /// ConnectionView.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty SourcePointProperty =
            DependencyProperty.Register(nameof(SourcePoint), typeof(Point), typeof(ConnectionView),
                new PropertyMetadata(new Point(0, 0), OnPointChanged));

        public static readonly DependencyProperty TargetPointProperty =
            DependencyProperty.Register(nameof(TargetPoint), typeof(Point), typeof(ConnectionView),
                new PropertyMetadata(new Point(0, 0), OnPointChanged));

        public static readonly DependencyProperty ConnectionTypeProperty =
            DependencyProperty.Register(nameof(ConnectionType), typeof(ConnectionType), typeof(ConnectionView),
                new PropertyMetadata(ConnectionType.Orthogonal, OnPointChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(ConnectionView),
                new PropertyMetadata(string.Empty, OnLabelChanged));

        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register(nameof(LabelPosition), typeof(Point), typeof(ConnectionView),
                new PropertyMetadata(new Point(0, 0), OnLabelPositionChanged));

        #endregion

        public Point SourcePoint
        {
            get => (Point)GetValue(SourcePointProperty);
            set => SetValue(SourcePointProperty, value);
        }

        public Point TargetPoint
        {
            get => (Point)GetValue(TargetPointProperty);
            set => SetValue(TargetPointProperty, value);
        }

        public ConnectionType ConnectionType
        {
            get => (ConnectionType)GetValue(ConnectionTypeProperty);
            set => SetValue(ConnectionTypeProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public Point LabelPosition
        {
            get => (Point)GetValue(LabelPositionProperty);
            set => SetValue(LabelPositionProperty, value);
        }

        public ConnectionView()
        {
            InitializeComponent();
        }

        private static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConnectionView view)
            {
                view.UpdateConnection();
            }
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConnectionView view)
            {
                view.UpdateLabel();
            }
        }

        private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConnectionView view)
            {
                view.UpdateLabelPosition();
            }
        }

        /// <summary>
        /// 更新连接线路径
        /// </summary>
        private void UpdateConnection()
        {
            var geometry = ConnectionType switch
            {
                ConnectionType.Bezier => PathGenerator.CreateBezierPath(SourcePoint, TargetPoint),
                ConnectionType.Orthogonal => PathGenerator.CreateOrthogonalPath(SourcePoint, TargetPoint),
                ConnectionType.Straight => PathGenerator.CreateStraightPath(SourcePoint, TargetPoint),
                _ => PathGenerator.CreateOrthogonalPath(SourcePoint, TargetPoint)
            };

            ConnectionPath.Data = geometry;

            // 更新箭头位置和旋转
            UpdateArrowHead(geometry);
        }

        /// <summary>
        /// 更新箭头位置
        /// </summary>
        private void UpdateArrowHead(PathGeometry geometry)
        {
            if (geometry.Figures.Count == 0 || geometry.Figures[0].Segments.Count == 0)
                return;

            // 获取终点附近的点来计算箭头方向
            var lastSegment = geometry.Figures[0].Segments[geometry.Figures[0].Segments.Count - 1];
            
            if (lastSegment is LineSegment lineSegment)
            {
                var endPoint = lineSegment.Point;
                var prevPoint = geometry.Figures[0].Segments.Count > 1
                    ? geometry.Figures[0].Segments[geometry.Figures[0].Segments.Count - 2].Point
                    : geometry.Figures[0].StartPoint;

                // 设置箭头位置在终点
                Canvas.SetLeft(ArrowHeadElement, endPoint.X - 5);
                Canvas.SetTop(ArrowHeadElement, endPoint.Y - 5);

                // 计算旋转角度
                double angle = Math.Atan2(endPoint.Y - prevPoint.Y, endPoint.X - prevPoint.X) * 180 / Math.PI;
                ArrowHeadElement.RenderTransform = new RotateTransform(angle, 5, 5);
                ArrowHeadElement.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 更新标签显示
        /// </summary>
        private void UpdateLabel()
        {
            if (!string.IsNullOrEmpty(Label))
            {
                LabelText.Text = Label;
                LabelBorder.Visibility = Visibility.Visible;
            }
            else
            {
                LabelBorder.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 更新标签位置
        /// </summary>
        private void UpdateLabelPosition()
        {
            Canvas.SetLeft(LabelBorder, LabelPosition.X - LabelBorder.ActualWidth / 2);
            Canvas.SetTop(LabelBorder, LabelPosition.Y - LabelBorder.ActualHeight / 2);
        }

        /// <summary>
        /// 从连接视图模型更新视图
        /// </summary>
        public void UpdateFromViewModel(ConnectionViewModel connectionVm, 
            NodeViewModel sourceNode, NodeViewModel targetNode)
        {
            // 计算连接点位置
            SourcePoint = CalculateConnectionPoint(sourceNode, connectionVm.SourcePointName);
            TargetPoint = CalculateConnectionPoint(targetNode, connectionVm.TargetPointName);
            
            ConnectionType = connectionVm.Type;
            Label = connectionVm.Label;
            
            UpdateConnection();
            UpdateLabel();
        }

        /// <summary>
        /// 计算连接点的实际位置
        /// </summary>
        private Point CalculateConnectionPoint(NodeViewModel node, string pointName)
        {
            return pointName.ToLower() switch
            {
                "top" => new Point(node.X + node.Width / 2, node.Y),
                "bottom" => new Point(node.X + node.Width / 2, node.Y + node.Height),
                "left" => new Point(node.X, node.Y + node.Height / 2),
                "right" => new Point(node.X + node.Width, node.Y + node.Height / 2),
                _ => new Point(node.X + node.Width / 2, node.Y + node.Height / 2)
            };
        }
    }
}
