using System.Windows;
using FlowChartEditor.ViewModels;

namespace FlowChartEditor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private FlowChartViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化 ViewModel
            ViewModel = new FlowChartViewModel();
            DataContext = ViewModel;
            
            // 创建示例流程图
            CreateSampleFlowchart();
        }

        /// <summary>
        /// 创建示例流程图（匹配参考图）
        /// </summary>
        private void CreateSampleFlowchart()
        {
            // 开始节点
            var start = new NodeViewModel(NodeType.Circle, 100, 50)
            {
                Label = "TST00",
                FillColor = System.Windows.Media.Brushes.LightGreen
            };
            ViewModel.Nodes.Add(start);

            // 判断节点
            var decision1 = new NodeViewModel(NodeType.Diamond, 100, 150)
            {
                Label = "if TST00",
                Condition = "Yes",
                FillColor = System.Windows.Media.Brushes.LightYellow
            };
            ViewModel.Nodes.Add(decision1);

            // 处理节点
            var process1 = new NodeViewModel(NodeType.Rectangle, 100, 280)
            {
                Label = "TST01",
                FillColor = System.Windows.Media.Brushes.LightBlue
            };
            ViewModel.Nodes.Add(process1);

            // 循环判断
            var loopDecision = new NodeViewModel(NodeType.Diamond, 100, 380)
            {
                Label = "for",
                Condition = "Yes",
                FillColor = System.Windows.Media.Brushes.LightYellow
            };
            ViewModel.Nodes.Add(loopDecision);

            // 处理节点 2
            var process2 = new NodeViewModel(NodeType.Rectangle, 100, 510)
            {
                Label = "TST02",
                FillColor = System.Windows.Media.Brushes.LightBlue
            };
            ViewModel.Nodes.Add(process2);

            // 循环判断 2
            var loopDecision2 = new NodeViewModel(NodeType.Diamond, 100, 610)
            {
                Label = "repeat",
                Condition = "No",
                FillColor = System.Windows.Media.Brushes.LightYellow
            };
            ViewModel.Nodes.Add(loopDecision2);

            // 结束节点
            var end = new NodeViewModel(NodeType.Circle, 100, 740)
            {
                Label = "End",
                FillColor = System.Windows.Media.Brushes.LightCoral
            };
            ViewModel.Nodes.Add(end);

            // 添加连接
            ViewModel.AddConnection(start, decision1);
            ViewModel.AddConnection(decision1, process1, "Yes");
            ViewModel.AddConnection(process1, loopDecision);
            ViewModel.AddConnection(loopDecision, process2, "Yes");
            ViewModel.AddConnection(process2, loopDecision2);
            ViewModel.AddConnection(loopDecision2, end, "No");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Flow-Wpf v1.0\n\nWPF Canvas-based flowchart editor for industrial automation.\n\n© 2026",
                "About Flow-Wpf",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
