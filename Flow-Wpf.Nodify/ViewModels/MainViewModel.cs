using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Flow_Wpf.Nodify.ViewModels
{
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
        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new();
        
        // Connections collection
        public ObservableCollection<ConnectionViewModel> Connections { get; set; } = new();

        // Commands will be added in Phase 5
    }

    public class NodeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public double X { get; set; }
        public double Y { get; set; }
        public string Title { get; set; } = "Node";
        public NodeType Type { get; set; }
    }

    public enum NodeType
    {
        Start,
        Process,
        Decision,
        Loop,
        End
    }

    public class ConnectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public NodeViewModel? From { get; set; }
        public NodeViewModel? To { get; set; }
        public string? Label { get; set; }
    }
}
