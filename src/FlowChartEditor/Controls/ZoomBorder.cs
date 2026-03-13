using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FlowChartEditor.Controls
{
    /// <summary>
    /// 缩放边框控件，支持鼠标滚轮缩放和平移
    /// </summary>
    public class ZoomBorder : Decorator
    {
        #region Fields

        private Point _origin;
        private Point _start;
        private TransformGroup _transformGroup;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        #endregion

        #region Constructor

        static ZoomBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomBorder),
                new FrameworkPropertyMetadata(typeof(ZoomBorder)));
        }

        public ZoomBorder()
        {
            InitializeTransforms();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 最小缩放比例
        /// </summary>
        public double MinZoom { get; set; } = 0.1;

        /// <summary>
        /// 最大缩放比例
        /// </summary>
        public double MaxZoom { get; set; } = 10.0;

        /// <summary>
        /// 当前缩放比例
        /// </summary>
        public double Zoom
        {
            get => _scaleTransform.ScaleX;
            set => ApplyZoom(value, _origin);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 初始化变换组
        /// </summary>
        private void InitializeTransforms()
        {
            _scaleTransform = new ScaleTransform(1, 1);
            _translateTransform = new TranslateTransform(0, 0);
            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_translateTransform);
            _transformGroup.Children.Add(_scaleTransform);
            
            RenderTransform = _transformGroup;
            RenderTransformOrigin = new Point(0, 0);
        }

        /// <summary>
        /// 应用缩放
        /// </summary>
        private void ApplyZoom(double zoom, Point relativeTo)
        {
            if (Child == null) return;

            zoom = Math.Max(MinZoom, Math.Min(MaxZoom, zoom));

            Point p = relativeTo;
            Point scaledCenter = new Point(
                Child.RenderTransformOrigin.X * Child.ActualWidth,
                Child.RenderTransformOrigin.Y * Child.ActualHeight
            );

            double scale = zoom / _scaleTransform.ScaleX;

            _scaleTransform.ScaleX = zoom;
            _scaleTransform.ScaleY = zoom;

            double newDeltaX = p.X - (p.X - _translateTransform.X) * scale;
            double newDeltaY = p.Y - (p.Y - _translateTransform.Y) * scale;

            _translateTransform.X += newDeltaX;
            _translateTransform.Y += newDeltaY;
        }

        /// <summary>
        /// 平移画布
        /// </summary>
        private void Pan(Point delta)
        {
            _translateTransform.X += delta.X;
            _translateTransform.Y += delta.Y;
        }

        #endregion

        #region Event Handlers

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (Child != null)
            {
                // 获取鼠标相对于子元素的位置
                Point mousePos = e.GetPosition(Child);
                
                // 计算新的缩放比例
                double zoom = _scaleTransform.ScaleX;
                double delta = e.Delta > 0 ? 1.1 : 0.9;
                zoom *= delta;

                // 应用缩放
                ApplyZoom(zoom, mousePos);
                
                e.Handled = true;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            // 如果按下的是空格键或中键，开始平移
            if (Keyboard.IsKeyDown(Key.Space) || e.ChangedButton == MouseButton.Middle)
            {
                _start = e.GetPosition(this);
                _origin = new Point(_translateTransform.X, _translateTransform.Y);
                
                if (e.ChangedButton == MouseButton.Middle)
                {
                    CaptureMouse();
                    _isPanning = true;
                }
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            // 处理平移
            if (_isPanning || (Keyboard.IsKeyDown(Key.Space) && e.LeftButton == MouseButtonState.Pressed))
            {
                if (IsMouseCaptured)
                {
                    Point p = e.GetPosition(this);
                    Point delta = new Point(p.X - _start.X, p.Y - _start.Y);
                    
                    _translateTransform.X = _origin.X + delta.X;
                    _translateTransform.Y = _origin.Y + delta.Y;
                    
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (_isPanning)
            {
                _isPanning = false;
                ReleaseMouseCapture();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 重置缩放和平移到初始状态
        /// </summary>
        public void Reset()
        {
            _scaleTransform.ScaleX = 1;
            _scaleTransform.ScaleY = 1;
            _translateTransform.X = 0;
            _translateTransform.Y = 0;
        }

        /// <summary>
        /// 缩放到适应内容
        /// </summary>
        public void ZoomToFit()
        {
            if (Child == null) return;

            // 计算适应内容的缩放比例
            double scaleX = ActualWidth / Child.ActualWidth;
            double scaleY = ActualHeight / Child.ActualHeight;
            double scale = Math.Min(scaleX, scaleY);

            ApplyZoom(scale, new Point(ActualWidth / 2, ActualHeight / 2));
        }

        #endregion

        #region Private Fields

        private bool _isPanning;

        #endregion
    }
}
