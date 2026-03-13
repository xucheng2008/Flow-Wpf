using System.Windows;
using Flow_Wpf.Nodify.ViewModels;

namespace Flow_Wpf.Nodify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            
            // Initialize with sample nodes for testing
            InitializeSampleNodes();
        }

        private void InitializeSampleNodes()
        {
            // Add sample nodes for testing
            ViewModel.Nodes.Add(new NodeViewModel 
            { 
                X = 100, Y = 100, 
                Title = "Start", 
                Type = NodeType.Start 
            });
            
            ViewModel.Nodes.Add(new NodeViewModel 
            { 
                X = 300, Y = 100, 
                Title = "SPEED < 200", 
                Type = NodeType.Decision 
            });
            
            ViewModel.Nodes.Add(new NodeViewModel 
            { 
                X = 500, Y = 100, 
                Title = "Process", 
                Type = NodeType.Process 
            });

            ViewModel.Nodes.Add(new NodeViewModel 
            { 
                X = 300, Y = 250, 
                Title = "FOR i=1000 TO 3500", 
                Type = NodeType.Loop 
            });
        }
    }
}
