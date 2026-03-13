using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Flow_Wpf.Nodify.ViewModels
{
    /// <summary>
    /// Node ViewModel with full MVVM support
    /// </summary>
    public class NodeViewModel : INotifyPropertyChanged
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

        // Node position
        private double _x, _y;
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }

        // Node title
        private string _title = "Node";
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        // Node type
        private NodeType _type = NodeType.Process;
        public NodeType Type { get => _type; set => SetProperty(ref _type, value); }

        // Node ID for unique identification
        private string _id = Guid.NewGuid().ToString("N")[..8];
        public string Id { get => _id; set => SetProperty(ref _id, value); }

        // Input and output connections
        public ObservableCollection<ConnectionViewModel> Inputs { get; set; } = new();
        public ObservableCollection<ConnectionViewModel> Outputs { get; set; } = new();

        // Selection state
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        // Custom data for node (can store any business logic data)
        public object? Data { get; set; }

        public override string ToString() => $"{Type}: {Title}";
    }

    /// <summary>
    /// Connection ViewModel linking two nodes
    /// </summary>
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _id = Guid.NewGuid().ToString("N")[..8];
        public string Id { get => _id; set => SetProperty(ref _id, value); }

        private NodeViewModel? _from;
        public NodeViewModel? From { get => _from; set => SetProperty(ref _from, value); }

        private NodeViewModel? _to;
        public NodeViewModel? To { get => _to; set => SetProperty(ref _to, value); }

        private string? _label;
        public string? Label { get => _label; set => SetProperty(ref _label, value); }

        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        public override string ToString() => $"{From?.Title} -> {To?.Title}";
    }

    /// <summary>
    /// Node types matching flowchart requirements
    /// </summary>
    public enum NodeType
    {
        Start,      // Rounded rectangle, dark
        Process,    // Rectangle, white
        Decision,   // Diamond, light blue
        Loop,       // Rectangle with badge, orange
        End         // Rounded rectangle, dark
    }
}
