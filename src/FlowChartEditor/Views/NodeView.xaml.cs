using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlowChartEditor.ViewModels;

namespace FlowChartEditor.Views
{
    /// <summary>
    /// NodeView.xaml 的交互逻辑
    /// </summary>
    public partial class NodeView : UserControl
    {
        public static readonly DependencyProperty NodeProperty =
            DependencyProperty.Register(nameof(Node), typeof(NodeViewModel), typeof(NodeView),
                new PropertyMetadata(null, OnNodeChanged));

        public NodeViewModel? Node
        {
            get => (NodeViewModel?)GetValue(NodeProperty);
            set => SetValue(NodeProperty, value);
        }

        public NodeView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // 当 DataContext 变化时，重新绑定
            if (DataContext is NodeViewModel node)
            {
                Node = node;
            }
        }

        private static void OnNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeView view)
            {
                view.UpdateVisualStates();
            }
        }

        private void UpdateVisualStates()
        {
            // 更新视觉状态
            if (Node != null)
            {
                // 可以在这里添加视觉状态切换逻辑
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            
            // 选中当前节点
            if (DataContext is NodeViewModel node)
            {
                node.IsSelected = true;
            }
            
            CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // 处理拖拽逻辑（由父容器 FlowChartCanvas 处理）
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            // 可以添加悬停效果
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            // 移除悬停效果
        }
    }
}
