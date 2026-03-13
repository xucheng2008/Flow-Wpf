using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FlowChartEditor.Models;

namespace FlowChartEditor.Controls
{
    public class FlowChartCanvas : Canvas
    {
        // Properties to hold the collection of nodes and connections
        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register("Nodes", typeof(ObservableCollection<NodeViewModel>), 
                typeof(FlowChartCanvas), new PropertyMetadata(null, OnNodesChanged));

        public static readonly DependencyProperty ConnectionsProperty =
            DependencyProperty.Register("Connections", typeof(ObservableCollection<ConnectionViewModel>), 
                typeof(FlowChartCanvas), new PropertyMetadata(null));

        // Dependency property for grid visibility
        public static readonly DependencyProperty ShowGridProperty =
            DependencyProperty.Register("ShowGrid", typeof(bool), 
                typeof(FlowChartCanvas), new PropertyMetadata(true, OnShowGridChanged));

        // Properties
        public ObservableCollection<NodeViewModel> Nodes
        {
            get => (ObservableCollection<NodeViewModel>)GetValue(NodesProperty);
            set => SetValue(NodesProperty, value);
        }

        public ObservableCollection<ConnectionViewModel> Connections
        {
            get => (ObservableCollection<ConnectionViewModel>)GetValue(ConnectionsProperty);
            set => SetValue(ConnectionsProperty, value);
        }

        public bool ShowGrid
        {
            get => (bool)GetValue(ShowGridProperty);
            set => SetValue(ShowGridProperty, value);
        }

        // Private fields for interaction
        private NodeViewModel _draggedNode;
        private Point _startPoint;
        private Line _connectionLine;
        private bool _isCreatingConnection = false;
        private bool _isPanning = false;
        private Point _panStartPoint;
        private Point _mouseDownPosition;
        private NodeViewModel _sourceNodeForConnection;

        // Grid properties
        private const double GridSize = 20.0;

        static FlowChartCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowChartCanvas), 
                new FrameworkPropertyMetadata(typeof(FlowChartCanvas)));
        }

        public FlowChartCanvas()
        {
            Loaded += FlowChartCanvas_Loaded;
            Unloaded += FlowChartCanvas_Unloaded;
            
            // Enable mouse events
            Focusable = true;
            FocusVisualStyle = null;
            Background = Brushes.White;
            
            // Enable mouse capture for drag operations
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseWheel += OnMouseWheel;
        }

        private void FlowChartCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCanvas();
        }

        private void FlowChartCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up event handlers
            MouseLeftButtonDown -= OnMouseLeftButtonDown;
            MouseMove -= OnMouseMove;
            MouseLeftButtonUp -= OnMouseLeftButtonUp;
            MouseWheel -= OnMouseWheel;
        }

        private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (FlowChartCanvas)d;
            if (e.OldValue is ObservableCollection<NodeViewModel> oldNodes)
            {
                oldNodes.CollectionChanged -= canvas.OnNodesCollectionChanged;
            }
            if (e.NewValue is ObservableCollection<NodeViewModel> newNodes)
            {
                newNodes.CollectionChanged += canvas.OnNodesCollectionChanged;
            }
            canvas.UpdateCanvas();
        }

        private static void OnShowGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (FlowChartCanvas)d;
            canvas.InvalidateVisual();
        }

        private void OnNodesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCanvas();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Draw grid if enabled
            if (ShowGrid)
            {
                DrawGrid(dc);
            }
        }

        private void DrawGrid(DrawingContext dc)
        {
            var pen = new Pen(Brushes.LightGray, 1.0);
            pen.Freeze();

            // Calculate visible area
            var visibleRect = new Rect(RenderSize);

            // Draw vertical lines
            for (double x = visibleRect.Left - (visibleRect.Left % GridSize); x <= visibleRect.Right; x += GridSize)
            {
                dc.DrawLine(pen, new Point(x, visibleRect.Top), new Point(x, visibleRect.Bottom));
            }

            // Draw horizontal lines
            for (double y = visibleRect.Top - (visibleRect.Top % GridSize); y <= visibleRect.Bottom; y += GridSize)
            {
                dc.DrawLine(pen, new Point(visibleRect.Left, y), new Point(visibleRect.Right, y));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            
            _mouseDownPosition = e.GetPosition(this);
            
            // Check if we clicked on a node
            var element = InputHitTest(_mouseDownPosition) as FrameworkElement;
            var nodeViewModel = FindAncestorViewModel(element);

            if (nodeViewModel != null)
            {
                // Check if we clicked on a connection point
                var pos = e.GetPosition(this);
                var connectionPoint = GetConnectionPointAt(pos, nodeViewModel);
                
                if (connectionPoint != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    // Start connection creation
                    StartConnectionCreation(nodeViewModel, connectionPoint.Value);
                }
                else
                {
                    // Start node dragging
                    _draggedNode = nodeViewModel;
                    _startPoint = e.GetPosition(this);
                    
                    // Select this node, deselect others
                    foreach (var node in Nodes)
                    {
                        node.IsSelected = node == _draggedNode;
                    }
                    
                    CaptureMouse();
                }
            }
            else
            {
                // Clicked on background - deselect all
                foreach (var node in Nodes)
                {
                    node.IsSelected = false;
                }
                
                // Check if we're holding space for panning
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    _isPanning = true;
                    _panStartPoint = e.GetPosition(this);
                    CaptureMouse();
                }
            }
            
            Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var currentPosition = e.GetPosition(this);

            if (_isCreatingConnection && _connectionLine != null)
            {
                // Update rubber band line during connection creation
                _connectionLine.X2 = currentPosition.X;
                _connectionLine.Y2 = currentPosition.Y;
            }
            else if (_draggedNode != null)
            {
                // Update node position during drag
                var delta = new Vector(
                    currentPosition.X - _startPoint.X,
                    currentPosition.Y - _startPoint.Y
                );

                _draggedNode.X += delta.X;
                _draggedNode.Y += delta.Y;

                // Update the start point for incremental movement
                _startPoint = currentPosition;

                // Update all connections connected to this node
                UpdateConnections();
            }
            else if (_isPanning)
            {
                // Handle panning
                var delta = new Vector(
                    currentPosition.X - _panStartPoint.X,
                    currentPosition.Y - _panStartPoint.Y
                );
                
                // Apply panning to parent scroll viewer or transform
                var parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
                while (parent != null)
                {
                    if (parent is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - delta.X);
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta.Y);
                        break;
                    }
                    else if (parent is ZoomBorder zoomBorder)
                    {
                        // Update the translate transform of the zoom border
                        var transform = zoomBorder.Content as UIElement;
                        if (transform?.RenderTransform is TranslateTransform tt)
                        {
                            tt.X -= delta.X;
                            tt.Y -= delta.Y;
                        }
                        break;
                    }
                    parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
                }
                
                _panStartPoint = currentPosition;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_isCreatingConnection)
            {
                // Try to complete the connection
                CompleteConnectionCreation(e.GetPosition(this));
            }
            else if (_draggedNode != null)
            {
                // End node dragging
                _draggedNode = null;
            }
            else if (_isPanning)
            {
                _isPanning = false;
            }

            ReleaseMouseCapture();

            // Remove temporary connection line if it exists
            if (_connectionLine != null && Children.Contains(_connectionLine))
            {
                Children.Remove(_connectionLine);
                _connectionLine = null;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            // Notify parent ZoomBorder to handle zoom
            var parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
            while (parent != null)
            {
                if (parent is ZoomBorder zoomBorder)
                {
                    // Forward the mouse wheel event to the ZoomBorder
                    var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent,
                        Source = this
                    };
                    zoomBorder.RaiseEvent(args);
                    break;
                }
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
        }

        private NodeViewModel FindAncestorViewModel(FrameworkElement element)
        {
            while (element != null)
            {
                if (element.DataContext is NodeViewModel nodeVm)
                    return nodeVm;
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return null;
        }

        private Point? GetConnectionPointAt(Point position, NodeViewModel node)
        {
            // Simplified logic - in a real implementation, you'd check actual connection points
            // For now, we'll just check if the click is near the edge of the node
            var nodeBounds = new Rect(node.X, node.Y, node.Width, node.Height);
            
            // Define connection points at center of each side
            var leftPoint = new Point(node.X, node.Y + node.Height / 2);
            var rightPoint = new Point(node.X + node.Width, node.Y + node.Height / 2);
            var topPoint = new Point(node.X + node.Width / 2, node.Y);
            var bottomPoint = new Point(node.X + node.Width / 2, node.Y + node.Height);
            
            // Check if position is close to any connection point (within 10 pixels)
            if (CalculateDistance(position, leftPoint) <= 10)
                return leftPoint;
            if (CalculateDistance(position, rightPoint) <= 10)
                return rightPoint;
            if (CalculateDistance(position, topPoint) <= 10)
                return topPoint;
            if (CalculateDistance(position, bottomPoint) <= 10)
                return bottomPoint;
            
            return null;
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void StartConnectionCreation(NodeViewModel sourceNode, Point startPoint)
        {
            _isCreatingConnection = true;
            _sourceNodeForConnection = sourceNode;
            
            // Create a temporary line to show the connection being created
            _connectionLine = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = startPoint.X,
                Y2 = startPoint.Y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                StrokeDashArray = new System.Windows.Media.DoubleCollection { 5, 5 }
            };
            
            Children.Add(_connectionLine);
        }

        private void CompleteConnectionCreation(Point endPoint)
        {
            // Find the target node at the end point
            var targetNode = FindNodeAt(endPoint);
            
            if (targetNode != null && targetNode != _sourceNodeForConnection)
            {
                // Create a new connection between the two nodes
                var connection = new ConnectionViewModel
                {
                    SourceNode = _sourceNodeForConnection,
                    TargetNode = targetNode,
                    SourcePoint = new Point(_sourceNodeForConnection.X + _sourceNodeForConnection.Width, 
                                          _sourceNodeForConnection.Y + _sourceNodeForConnection.Height / 2),
                    TargetPoint = new Point(targetNode.X, targetNode.Y + targetNode.Height / 2)
                };
                
                Connections.Add(connection);
            }
            
            _isCreatingConnection = false;
            _sourceNodeForConnection = null;
            
            // Remove the temporary line
            if (_connectionLine != null && Children.Contains(_connectionLine))
            {
                Children.Remove(_connectionLine);
                _connectionLine = null;
            }
        }

        private NodeViewModel FindNodeAt(Point position)
        {
            foreach (var node in Nodes)
            {
                var nodeRect = new Rect(node.X, node.Y, node.Width, node.Height);
                if (nodeRect.Contains(position))
                {
                    return node;
                }
            }
            return null;
        }

        public void UpdateConnections()
        {
            // Remove all connection visuals
            var connectionVisuals = Children.OfType<Line>().ToList();
            foreach (var line in connectionVisuals)
            {
                Children.Remove(line);
            }
            
            // Redraw all connections
            if (Connections != null)
            {
                foreach (var conn in Connections)
                {
                    var line = new Line
                    {
                        X1 = conn.SourceNode.X + conn.SourceNode.Width,
                        Y1 = conn.SourceNode.Y + conn.SourceNode.Height / 2,
                        X2 = conn.TargetNode.X,
                        Y2 = conn.TargetNode.Y + conn.TargetNode.Height / 2,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    
                    // Add arrowhead at the end of the line
                    var pathGeometry = new PathGeometry();
                    var pathFigure = new PathFigure(new Point(line.X2, line.Y2), new PathSegment[]
                    {
                        new LineSegment(new Point(line.X2 - 10, line.Y2 - 5), false),
                        new LineSegment(new Point(line.X2 - 10, line.Y2 + 5), false)
                    }, true);
                    pathGeometry.Figures.Add(pathFigure);
                    
                    var path = new Path
                    {
                        Data = pathGeometry,
                        Fill = Brushes.Black,
                        RenderTransform = new RotateTransform(
                            Math.Atan2(line.Y2 - line.Y1, line.X2 - line.X1) * 180 / Math.PI,
                            line.X2, line.Y2)
                    };
                    
                    Children.Add(line);
                    Children.Add(path);
                }
            }
        }

        private void UpdateCanvas()
        {
            // Clear existing children except connection lines (they'll be redrawn)
            var nonConnectionChildren = Children.OfType<UIElement>()
                .Where(child => !(child is Line) || child == _connectionLine)
                .ToList();
            
            foreach (var child in nonConnectionChildren)
            {
                Children.Remove(child);
            }
            
            // Redraw connections
            UpdateConnections();
        }
    }
}