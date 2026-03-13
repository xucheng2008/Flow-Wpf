using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Flow_Wpf.Nodify.ViewModels;

namespace Flow_Wpf.Nodify.Converters
{
    public class NodeTypeToTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeType nodeType)
            {
                var app = Application.Current;
                return nodeType switch
                {
                    NodeType.Start => app?.FindResource("StartNodeTemplate"),
                    NodeType.Decision => app?.FindResource("DecisionNodeTemplate"),
                    NodeType.Process => app?.FindResource("ProcessNodeTemplate"),
                    NodeType.Loop => app?.FindResource("LoopNodeTemplate"),
                    NodeType.End => app?.FindResource("EndNodeTemplate"),
                    _ => app?.FindResource("ProcessNodeTemplate")
                };
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
