using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace VirtualJoystick
{
    public partial class VirtualJoystick : UserControl
    {

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(VirtualJoystick), null);

        /// <summary>
        /// Current distanse (from 0 to 100)
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(VirtualJoystick), null);

        /// <summary>
        /// Delta angle to raise event StickMove
        /// </summary>
        public static readonly DependencyProperty AngleStepProperty = DependencyProperty.Register("AngleStep", typeof(double), typeof(VirtualJoystick), new PropertyMetadata(1.0));

        /// <summary>
        /// Delta distance to raise event StickMove
        /// </summary>
        public static readonly DependencyProperty DistanceStepProperty = DependencyProperty.Register("DistanceStep", typeof(double), typeof(VirtualJoystick), new PropertyMetadata(1.0));

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public double Angle

        {
            get { return Convert.ToDouble(GetValue(AngleProperty)); }
            private set
            {
                if (value > 315 || value < 45 && value > 0)
                {
                    TopArrow.Fill = new SolidColorBrush(Colors.DodgerBlue);
                    LeftArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    RightArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    BottomArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                }
                else if (value > 45 && value < 135 && value > 0)
                {
                    RightArrow.Fill = new SolidColorBrush(Colors.DodgerBlue);
                    TopArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    LeftArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    BottomArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));

                }
                else if (value > 135 && value < 225 && value > 0)
                {
                    LeftArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    TopArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    RightArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    BottomArrow.Fill = new SolidColorBrush(Colors.DodgerBlue);
                }
                else if (value > 225 && value < 315 && value > 0)
                {
                    TopArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    RightArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    BottomArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    LeftArrow.Fill = new SolidColorBrush(Colors.DodgerBlue);
                }
                else if (value == 0)
                {
                    TopArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    LeftArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    RightArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                    BottomArrow.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
                }
                SetValue(AngleProperty, value);
            }
        }

        /// <summary>
        /// Current distanse (from 0 to 100)
        /// </summary>
        public double Distance
        {
            get { return Convert.ToDouble(GetValue(DistanceProperty)); }
            private set { SetValue(DistanceProperty, value); }
        }

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public double AngleStep
        {
            get { return Convert.ToDouble(GetValue(AngleStepProperty)); }
            set
            {
                if (value < 1) value = 1; else if (value > 90) value = 90;
                SetValue(AngleStepProperty, Math.Round(value));
            }
        }

        /// <summary>
        /// Current angle (in degrees)
        /// </summary>
        public double DistanceStep
        {
            get { return Convert.ToDouble(GetValue(DistanceStepProperty)); }
            set
            {
                if (value < 1) value = 1; else if (value > 50) value = 50;
                SetValue(DistanceStepProperty, value);
            }
        }

        /// <summary>
        ///  Event raised when stick is moved
        /// </summary>
        public event EventHandler StickMove;
        private Pointer pointer;
        private Point _startPos;
        private Boolean pointerOutOfArea = true;
        private double _prevAngle, _prevDistance;

        public VirtualJoystick()
        {
            InitializeComponent();
            centerKnob.Completed += new EventHandler<object>(centerKnob_Completed);
            Knob.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(Knob_PointerDown), true);
            Knob.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(Knob_PointerMoved), true);
            Knob.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(Knob_PointerUp), true);
            Knob.AddHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(Knob_PointerLost), true);
            Knob.AddHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(Knob_PointerUp), true);
            Knob.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(Knob_PointerUp), true);
            Knob.AddHandler(UIElement.PointerCanceledEvent, new PointerEventHandler(Knob_PointerUp), true);
            Knob.LostFocus += Knob_LostFocus;

        }

        private void Knob_LostFocus(object sender, RoutedEventArgs e)
        {
            Knob.ReleasePointerCapture(pointer);
            centerKnob.Begin();
        }
        private void Knob_PointerLost(object sender, PointerRoutedEventArgs e)
        {
            pointerOutOfArea = true;
        }
        private void Knob_PointerDown(object sender, PointerRoutedEventArgs e)
        {
            _startPos = e.GetCurrentPoint(Base).Position;
            _prevAngle = _prevDistance = 0;
            Knob.CapturePointer(e.Pointer);
            pointerOutOfArea = false;
            pointer = e.Pointer;

        }

        private void Knob_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (pointer != null && !pointerOutOfArea)
            {
                Point newPos = e.GetCurrentPoint(Base).Position;

                Point p = new Point(newPos.X - _startPos.X, newPos.Y - _startPos.Y);

                double angle = Math.Atan2(p.Y, p.X) * 180 / Math.PI;
                if (angle > 0) angle += 90;
                else
                {
                    angle = 270 + (180 + angle);
                    if (angle >= 360) angle -= 360;
                }

                double distance = Math.Round(Math.Sqrt(p.X * p.X + p.Y * p.Y) / 135 * 100);
                if (distance <= 100)
                {
                    Angle = angle;
                    Distance = distance;

                    knobPosition.X = p.X;
                    knobPosition.Y = p.Y;

                    if (StickMove != null && (Math.Abs(_prevAngle - angle) > AngleStep || Math.Abs(_prevDistance - distance) > DistanceStep))
                    {
                        StickMove(this, new EventArgs());
                        _prevAngle = Angle;
                        _prevDistance = Distance;
                    }
                }
            }
        }

        private void Knob_PointerUp(object sender, PointerRoutedEventArgs e)
        {
            if (pointer != null)
            {
                Knob.ReleasePointerCapture(pointer);
            }
            centerKnob.Begin();
        }

        private void centerKnob_Completed(object sender, object e)
        {
            Angle = Distance = _prevAngle = _prevDistance = 0;
            if (StickMove != null) StickMove(this, new EventArgs());
        }

    }
}
