using Flow_Wpf.Nodify.ViewModels;
using Flow_Wpf.Nodify.Services;

namespace Flow_Wpf.Nodify.Services
{
    /// <summary>
    /// Flow execution engine - interprets and runs flowcharts
    /// </summary>
    public class FlowExecutor
    {
        private readonly MainViewModel _viewModel;
        private NodeViewModel? _currentNode;
        private bool _isExecuting;
        private bool _isPaused;
        private readonly Dictionary<string, object> _variables = new();
        private int _loopCounter;

        // Events
        public event EventHandler<NodeExecutedEventArgs>? NodeExecuted;
        public event EventHandler<FlowCompletedEventArgs>? FlowCompleted;
        public event EventHandler<ExecutionErrorEventArgs>? ExecutionError;

        public bool IsExecuting => _isExecuting;
        public bool IsPaused => _isPaused;
        public NodeViewModel? CurrentNode => _currentNode;
        public IReadOnlyDictionary<string, object> Variables => _variables;

        public FlowExecutor(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        /// <summary>
        /// Start flow execution from beginning
        /// </summary>
        public void Execute()
        {
            _isExecuting = true;
            _isPaused = false;
            _variables.Clear();
            _loopCounter = 0;

            // Find start node
            var startNode = _viewModel.Nodes.FirstOrDefault(n => n.Type == NodeType.Start);
            if (startNode == null)
            {
                OnExecutionError("No start node found");
                return;
            }

            ExecuteNode(startNode);
        }

        /// <summary>
        /// Execute single node and move to next
        /// </summary>
        public void Step()
        {
            if (_currentNode != null && _isExecuting)
            {
                ExecuteNode(_currentNode);
            }
        }

        /// <summary>
        /// Stop execution
        /// </summary>
        public void Stop()
        {
            _isExecuting = false;
            _isPaused = false;
            _currentNode = null;
        }

        /// <summary>
        /// Pause execution
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resume execution
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
            if (_currentNode != null)
            {
                ExecuteNode(_currentNode);
            }
        }

        private void ExecuteNode(NodeViewModel node)
        {
            _currentNode = node;
            OnNodeExecuted(node);

            try
            {
                var nextNode = node.Type switch
                {
                    NodeType.Start => GetNextNode(node),
                    NodeType.Process => ExecuteProcessNode(node),
                    NodeType.Decision => ExecuteDecisionNode(node),
                    NodeType.Loop => ExecuteLoopNode(node),
                    NodeType.End => ExecuteEndNode(node),
                    _ => GetNextNode(node)
                };

                if (nextNode != null && _isExecuting && !_isPaused)
                {
                    // Small delay for visual feedback
                    Task.Delay(100).ContinueWith(_ => ExecuteNode(nextNode));
                }
                else if (node.Type == NodeType.End)
                {
                    OnFlowCompleted();
                }
            }
            catch (Exception ex)
            {
                OnExecutionError($"Error executing {node.Title}: {ex.Message}");
                Stop();
            }
        }

        private NodeViewModel? ExecuteProcessNode(NodeViewModel node)
        {
            // Execute process logic here
            // This can be extended with custom actions
            return GetNextNode(node);
        }

        private NodeViewModel? ExecuteDecisionNode(NodeViewModel node)
        {
            // Evaluate decision condition
            // Example: "SPEED < 200"
            var condition = node.Title;
            var result = EvaluateCondition(condition);

            // Find connection based on result
            var label = result ? "yes" : "no";
            var connection = _viewModel.Connections
                .FirstOrDefault(c => c.From == node && 
                                    (c.Label?.ToLower() == label || string.IsNullOrEmpty(c.Label)));

            return connection?.To;
        }

        private NodeViewModel? ExecuteLoopNode(NodeViewModel node)
        {
            // Parse loop: "FOR i=1000 TO 3500"
            // This is a simplified implementation
            _loopCounter++;

            if (_loopCounter <= 1) // Just one iteration for demo
            {
                return GetNextNode(node);
            }
            else
            {
                _loopCounter = 0;
                return GetNextNode(node);
            }
        }

        private NodeViewModel? ExecuteEndNode(NodeViewModel node)
        {
            _isExecuting = false;
            return null;
        }

        private NodeViewModel? GetNextNode(NodeViewModel currentNode)
        {
            var connection = _viewModel.Connections.FirstOrDefault(c => c.From == currentNode);
            return connection?.To;
        }

        private bool EvaluateCondition(string condition)
        {
            // Simple condition evaluator
            // Supports: SPEED < 200, repeat == true, etc.
            // This is a basic implementation - can be extended

            if (string.IsNullOrEmpty(condition))
                return true;

            // Try to evaluate as comparison
            var operators = new[] { "<=", ">=", "!=", "==", "<", ">", "=" };
            foreach (var op in operators)
            {
                if (condition.Contains(op))
                {
                    var parts = condition.Split(new[] { op }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        var left = parts[0].Trim();
                        var right = parts[1].Trim();

                        // Get variable value or use as-is
                        var leftValue = _variables.TryGetValue(left, out var v) ? v : left;
                        var rightValue = _variables.TryGetValue(right, out var v2) ? v2 : right;

                        // Compare
                        return CompareValues(leftValue, rightValue, op);
                    }
                }
            }

            // Default: check if variable is truthy
            if (_variables.TryGetValue(condition, out var value))
            {
                return Convert.ToBoolean(value);
            }

            return true;
        }

        private bool CompareValues(object? left, object? right, string op)
        {
            if (left is int lInt && right is int rInt)
            {
                return op switch
                {
                    "<" => lInt < rInt,
                    "<=" => lInt <= rInt,
                    ">" => lInt > rInt,
                    ">=" => lInt >= rInt,
                    "==" or "=" => lInt == rInt,
                    "!=" => lInt != rInt,
                    _ => false
                };
            }

            // String comparison
            var lStr = left?.ToString() ?? "";
            var rStr = right?.ToString() ?? "";
            var cmp = string.Compare(lStr, rStr, StringComparison.Ordinal);

            return op switch
            {
                "<" => cmp < 0,
                "<=" => cmp <= 0,
                ">" => cmp > 0,
                ">=" => cmp >= 0,
                "==" or "=" => cmp == 0,
                "!=" => cmp != 0,
                _ => false
            };
        }

        protected virtual void OnNodeExecuted(NodeViewModel node)
        {
            NodeExecuted?.Invoke(this, new NodeExecutedEventArgs { Node = node });
        }

        protected virtual void OnFlowCompleted()
        {
            FlowCompleted?.Invoke(this, new FlowCompletedEventArgs { Variables = _variables });
        }

        protected virtual void OnExecutionError(string message)
        {
            ExecutionError?.Invoke(this, new ExecutionErrorEventArgs { Message = message });
        }

        /// <summary>
        /// Set a variable value
        /// </summary>
        public void SetVariable(string name, object value)
        {
            _variables[name] = value;
        }

        /// <summary>
        /// Get a variable value
        /// </summary>
        public object? GetVariable(string name)
        {
            return _variables.TryGetValue(name, out var value) ? value : null;
        }
    }

    /// <summary>
    /// Event args for node execution
    /// </summary>
    public class NodeExecutedEventArgs : EventArgs
    {
        public NodeViewModel? Node { get; set; }
    }

    /// <summary>
    /// Event args for flow completion
    /// </summary>
    public class FlowCompletedEventArgs : EventArgs
    {
        public IReadOnlyDictionary<string, object>? Variables { get; set; }
    }

    /// <summary>
    /// Event args for execution errors
    /// </summary>
    public class ExecutionErrorEventArgs : EventArgs
    {
        public string Message { get; set; } = "";
    }
}
