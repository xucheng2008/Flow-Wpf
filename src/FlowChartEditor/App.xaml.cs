using System.Windows;

namespace FlowChartEditor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 设置全局异常处理
            DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"发生错误：{args.Exception.Message}", "错误", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
