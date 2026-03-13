using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlowChartEditor.Converters
{
    /// <summary>
    /// 将布尔值转换为可见性
    /// true = Visible, false = Collapsed
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 布尔值转可见性
        /// </summary>
        /// <param name="value">布尔值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数（可选：反转）</param>
        /// <param name="culture">文化信息</param>
        /// <returns>Visibility 枚举值</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return Visibility.Collapsed;

            // 如果参数为"invert"，则反转结果
            bool invert = parameter?.ToString()?.ToLower() == "invert";

            if (invert)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 可见性转布尔值
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility visibility)
                return false;

            bool invert = parameter?.ToString()?.ToLower() == "invert";

            if (invert)
            {
                return visibility == Visibility.Collapsed;
            }

            return visibility == Visibility.Visible;
        }
    }

    /// <summary>
    /// 将布尔值转换为可见性（true=Collapsed, false=Visible）
    /// 用于相反的逻辑场景
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return Visibility.Visible;

            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility visibility)
                return false;

            return visibility != Visibility.Visible;
        }
    }
}
