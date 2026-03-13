using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Flow_Wpf.Nodify.Services;
using Microsoft.Win32;

namespace Flow_Wpf.Nodify.ViewModels
{
    /// <summary>
    /// Main ViewModel for the Nodify Editor
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        // Nodes collection
        private ObservableCollection<NodeViewModel> _nodes = new();
        public ObservableCollection<NodeViewModel> Nodes
        {
            get => _nodes;
            set => SetProperty(ref _nodes, value);
        }

        // Connections collection
        private ObservableCollection<ConnectionViewModel> _connections = new();
        public ObservableCollection<ConnectionViewModel> Connections
        {
            get => _connections;
            set => SetProperty(ref _connections, value);
        }

        // Selected node
        private NodeViewModel? _selectedNode;
        public NodeViewModel? SelectedNode
        {
            get => _selectedNode;
            set => SetProperty(ref _selectedNode, value);
        }

        // Selected connection
        private ConnectionViewModel? _selectedConnection;
        public ConnectionViewModel? SelectedConnection
        {
            get => _selectedConnection;
            set => SetProperty(ref _selectedConnection, value);
        }

        // Commands
        public ICommand AddNodeCommand { get; }
        public ICommand DeleteNodeCommand { get; }
        public ICommand DeleteConnectionCommand { get; }
        public ICommand AddConnectionCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand ExecuteCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ResetZoomCommand { get; }

        private FlowExecutor? _executor;
        private double _zoom = 1.0;

        public MainViewModel()
        {
            AddNodeCommand = new RelayCommand(AddNode);
            DeleteNodeCommand = new RelayCommand(DeleteSelectedNode, () => SelectedNode != null);
            DeleteConnectionCommand = new RelayCommand(DeleteSelectedConnection, () => SelectedConnection != null);
            AddConnectionCommand = new RelayCommand<ConnectionViewModel>(AddConnection);
            SaveCommand = new RelayCommand(async () => await SaveFlow());
            OpenCommand = new RelayCommand(async () => await OpenFlow());
            NewCommand = new RelayCommand(NewFlow);
            ExecuteCommand = new RelayCommand(ExecuteFlow);
            PauseCommand = new RelayCommand(PauseFlow);
            StopCommand = new RelayCommand(StopFlow);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ResetZoomCommand = new RelayCommand(ResetZoom);

            // Initialize executor
            _executor = new FlowExecutor(this);
            _executor.NodeExecuted += OnNodeExecuted;
            _executor.FlowCompleted += OnFlowCompleted;

            // Initialize with sample nodes
            InitializeSampleNodes();
        }

        private void InitializeSampleNodes()
        {
            // Start node
            var start = new NodeViewModel { X = 100, Y = 100, Title = "Start", Type = NodeType.Start };
            Nodes.Add(start);

            // Decision node
            var decision = new NodeViewModel { X = 300, Y = 100, Title = "SPEED < 200", Type = NodeType.Decision };
            Nodes.Add(decision);

            // Process node
            var process = new NodeViewModel { X = 500, Y = 100, Title = "Process", Type = NodeType.Process };
            Nodes.Add(process);

            // Loop node
            var loop = new NodeViewModel { X = 300, Y = 250, Title = "FOR i=1000 TO 3500", Type = NodeType.Loop };
            Nodes.Add(loop);

            // End node
            var end = new NodeViewModel { X = 700, Y = 100, Title = "End", Type = NodeType.End };
            Nodes.Add(end);

            // Add connections
            var conn1 = new ConnectionViewModel { From = start, To = decision, Label = "" };
            Connections.Add(conn1);

            var conn2 = new ConnectionViewModel { From = decision, To = process, Label = "yes" };
            Connections.Add(conn2);

            var conn3 = new ConnectionViewModel { From = decision, To = loop, Label = "no" };
            Connections.Add(conn3);

            var conn4 = new ConnectionViewModel { From = process, To = end, Label = "" };
            Connections.Add(conn4);
        }

        private void AddNode()
        {
            var node = new NodeViewModel
            {
                X = 200 + Nodes.Count * 50,
                Y = 200,
                Title = $"Node {Nodes.Count + 1}",
                Type = NodeType.Process
            };
            Nodes.Add(node);
        }

        private void DeleteSelectedNode()
        {
            if (SelectedNode != null)
            {
                // Remove connections associated with this node
                var toRemove = Connections.Where(c => c.From == SelectedNode || c.To == SelectedNode).ToList();
                foreach (var conn in toRemove)
                {
                    Connections.Remove(conn);
                }
                Nodes.Remove(SelectedNode);
                SelectedNode = null;
            }
        }

        private void DeleteSelectedConnection()
        {
            if (SelectedConnection != null)
            {
                Connections.Remove(SelectedConnection);
                SelectedConnection = null;
            }
        }

        private void AddConnection(ConnectionViewModel? connection)
        {
            if (connection != null)
            {
                Connections.Add(connection);
            }
        }

        private string? _currentFilePath;

        private async Task SaveFlow()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Flow Files (*.flow)|*.flow|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    Title = "Save Flow Chart",
                    FileName = "MyFlow"
                };

                if (dialog.ShowDialog() == true)
                {
                    await FlowSerializer.SaveViewModelAsync(this, dialog.FileName);
                    _currentFilePath = dialog.FileName;
                    // Could add status notification here
                }
            }
            catch (Exception ex)
            {
                // Handle error - could show message box
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            }
        }

        private async Task OpenFlow()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Flow Files (*.flow)|*.flow|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    Title = "Open Flow Chart"
                };

                if (dialog.ShowDialog() == true)
                {
                    var newViewModel = await FlowSerializer.LoadViewModelAsync(dialog.FileName);
                    
                    // Copy loaded data
                    Nodes.Clear();
                    Connections.Clear();
                    
                    foreach (var node in newViewModel.Nodes)
                        Nodes.Add(node);
                    
                    foreach (var conn in newViewModel.Connections)
                        Connections.Add(conn);
                    
                    _currentFilePath = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Open error: {ex.Message}");
            }
        }

        private void NewFlow()
        {
            Nodes.Clear();
            Connections.Clear();
            _currentFilePath = null;
        }

        private void ExecuteFlow()
        {
            _executor?.Execute();
        }

        private void PauseFlow()
        {
            _executor?.Pause();
        }

        private void StopFlow()
        {
            _executor?.Stop();
        }

        private void ZoomIn()
        {
            _zoom = Math.Min(_zoom + 0.25, 5.0);
            OnPropertyChanged(nameof(Zoom));
        }

        private void ZoomOut()
        {
            _zoom = Math.Max(_zoom - 0.25, 0.1);
            OnPropertyChanged(nameof(Zoom));
        }

        private void ResetZoom()
        {
            _zoom = 1.0;
            OnPropertyChanged(nameof(Zoom));
        }

        public double Zoom
        {
            get => _zoom;
            set => SetProperty(ref _zoom, value);
        }

        private void OnNodeExecuted(object? sender, NodeExecutedEventArgs e)
        {
            // Highlight executed node
            System.Diagnostics.Debug.WriteLine($"Executed: {e.Node?.Title}");
        }

        private void OnFlowCompleted(object? sender, FlowCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Flow completed!");
        }
    }

    /// <summary>
    /// Simple RelayCommand for MVVM commands
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action? _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute?.Invoke();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

        public void Execute(object? parameter) => _execute((T?)parameter);
    }
}
