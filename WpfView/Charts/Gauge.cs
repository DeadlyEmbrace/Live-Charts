﻿//The MIT License(MIT)

//copyright(c) 2016 Alberto Rodriguez

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LiveCharts.Wpf.Points;

// ReSharper disable once CheckNamespace
namespace LiveCharts.Wpf
{
    /// <summary>
    /// The gauge chart is useful to display progress or completion.
    /// </summary>
    public class Gauge : UserControl
    {
        public Gauge()
        {
            Canvas = new Canvas {ClipToBounds = true};
            Content = Canvas;

            PieBack = new PieSlice();
            Pie = new PieSlice();
            TitleTextBlock = new TextBlock();
            MeasureTextBlock = new TextBlock();
            LeftLabel = new TextBlock();
            RightLabel = new TextBlock();

            Canvas.Children.Add(PieBack);
            Canvas.Children.Add(Pie);
            Canvas.Children.Add(TitleTextBlock);
            Canvas.Children.Add(MeasureTextBlock);
            Canvas.Children.Add(RightLabel);
            Canvas.Children.Add(LeftLabel);

            Panel.SetZIndex(PieBack, 0);
            Panel.SetZIndex(Pie, 1);

            Canvas.SetBinding(WidthProperty,
                new Binding {Path = new PropertyPath(WidthProperty), Source = this});
            Canvas.SetBinding(HeightProperty,
                new Binding {Path = new PropertyPath(HeightProperty), Source = this});

            PieBack.SetBinding(Shape.FillProperty,
                new Binding {Path = new PropertyPath(GaugeBackgroundProperty), Source = this});
            PieBack.SetBinding(Shape.StrokeThicknessProperty,
                new Binding {Path = new PropertyPath(StrokeThicknessProperty), Source = this});
            PieBack.SetBinding(Shape.StrokeProperty,
                new Binding {Path = new PropertyPath(StrokeProperty), Source = this});

            Pie.SetBinding(Shape.StrokeThicknessProperty,
                new Binding { Path = new PropertyPath(StrokeThicknessProperty), Source = this });
            Pie.Stroke = Brushes.Transparent;

            TitleTextBlock.SetBinding(TextBlock.TextProperty,
                new Binding { Path = new PropertyPath(TitleProperty), Source = this });

            SetValue(GaugeBackgroundProperty, new SolidColorBrush(Color.FromRgb(21, 101, 191)) {Opacity = .1});
            SetValue(StrokeThicknessProperty, 0d);
            SetValue(StrokeProperty, new SolidColorBrush(Color.FromRgb(222, 222, 222)));

            SetValue(ToColorProperty, Color.FromRgb(100, 180, 245));
            SetValue(FromColorProperty, Color.FromRgb(21, 101, 191));

            SetValue(MinHeightProperty, 50d);
            SetValue(MinWidthProperty, 80d);

            SetValue(AnimationsSpeedProperty, TimeSpan.FromMilliseconds(800));

            MeasureTextBlock.FontWeight = FontWeights.Bold;

            IsNew = true;

            SizeChanged += (sender, args) =>
            {
                IsChartInitialized = true;
                Update();
            };
        }

        #region Properties

        private Canvas Canvas { get; set; }
        private PieSlice PieBack { get; set; }
        private PieSlice Pie { get; set; }
        private TextBlock TitleTextBlock { get; set; }
        private TextBlock MeasureTextBlock { get; set; }
        private TextBlock LeftLabel { get; set; }
        private TextBlock RightLabel { get; set; }
        private bool IsNew { get; set; }
        private bool IsChartInitialized { get; set; }

        public static readonly DependencyProperty Uses360ModeProperty = DependencyProperty.Register(
            "Uses360Mode", typeof (bool), typeof (Gauge), new PropertyMetadata(default(bool), UpdateCallback));
        /// <summary>
        /// Gets or sets whether the gauge uses 360 mode, 360 mode will plot a full circle instead of a semi circle
        /// </summary>
        public bool Uses360Mode
        {
            get { return (bool) GetValue(Uses360ModeProperty); }
            set { SetValue(Uses360ModeProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            "From", typeof(double), typeof(Gauge), new PropertyMetadata(0d, UpdateCallback));
        /// <summary>
        /// Gets or sets the value where the gauge starts
        /// </summary>
        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To", typeof(double), typeof(Gauge), new PropertyMetadata(1d, UpdateCallback));
        /// <summary>
        /// Gets or sets the value where the gauge ends
        /// </summary>
        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof (double), typeof (Gauge), new PropertyMetadata(default(double), UpdateCallback));
        /// <summary>
        /// Gets or sets the current value of the gauge
        /// </summary>
        public double Value
        {
            get { return (double) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Gauge), new PropertyMetadata(default(string)));
        /// <summary>
        /// Gets or sets the gauge title
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            "InnerRadius", typeof (double?), typeof (Gauge), new PropertyMetadata(null, UpdateCallback));
        /// <summary>
        /// Gets o sets inner radius
        /// </summary>
        public double? InnerRadius
        {
            get { return (double?) GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(Gauge), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets stroke, the stroke is the brush used to draw the gauge border.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }


        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(Gauge), new PropertyMetadata(default(double)));
        /// <summary>
        /// Gets or sets stroke brush thickness
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ToColorProperty = DependencyProperty.Register(
            "ToColor", typeof(Color), typeof(Gauge), new PropertyMetadata(default(Color), UpdateCallback));
        /// <summary>
        /// Gets or sets the color when the current value equals to min value, any value between min and max will use an interpolated color.
        /// </summary>
        public Color ToColor
        {
            get { return (Color)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        public static readonly DependencyProperty FromColorProperty = DependencyProperty.Register(
            "FromColor", typeof(Color), typeof(Gauge), new PropertyMetadata(default(Color), UpdateCallback));
        /// <summary>
        /// Gets or sets the color when the current value equals to max value, any value between min and max will use an interpolated color.
        /// </summary>
        public Color FromColor
        {
            get { return (Color)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        public static readonly DependencyProperty GaugeBackgroundProperty = DependencyProperty.Register(
            "GaugeBackground", typeof (Brush), typeof (Gauge), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets the gauge background
        /// </summary>
        public Brush GaugeBackground
        {
            get { return (Brush) GetValue(GaugeBackgroundProperty); }
            set { SetValue(GaugeBackgroundProperty, value); }
        }

        public static readonly DependencyProperty AnimationsSpeedProperty = DependencyProperty.Register(
            "AnimationsSpeed", typeof (TimeSpan), typeof (Gauge), new PropertyMetadata(default(TimeSpan)));
        /// <summary>
        /// G3ts or sets the gauge animations speed
        /// </summary>
        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan) GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public static readonly DependencyProperty LabelFormatterProperty = DependencyProperty.Register(
            "LabelFormatter", typeof (Func<double, string>), typeof (Gauge), new PropertyMetadata(default(Func<double, string>)));
        /// <summary>
        /// Gets or sets the label formatter, a label formatter takes a double value, and return a string, e.g. val => val.ToString("C");
        /// </summary>
        public Func<double, string> LabelFormatter
        {
            get { return (Func<double, string>) GetValue(LabelFormatterProperty); }
            set { SetValue(LabelFormatterProperty, value); }
        }

        public static readonly DependencyProperty HighFontSizeProperty = DependencyProperty.Register(
            "HighFontSize", typeof (double?), typeof (Gauge), new PropertyMetadata(null));
        /// <summary>
        /// Gets o sets the label size, if this value is null then it will be automatically calculated, default is null.
        /// </summary>
        public double? HighFontSize
        {
            get { return (double?) GetValue(HighFontSizeProperty); }
            set { SetValue(HighFontSizeProperty, value); }
        }

        #endregion

        private static void UpdateCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var gauge = (Gauge)dependencyObject;

            gauge.Update();
        }

        private void Update()
        {
            if (!IsChartInitialized) return;

            Func<double, string> defFormatter = x => x.ToString(CultureInfo.InvariantCulture);

            var completed = Value/(To - From);

            var t = 0d;

            completed = completed > 1 ? 1 : (completed < 0 ? 0 : completed);
            var angle = Uses360Mode ? 360 : 180;

            if (!string.IsNullOrWhiteSpace(Title))
            {
                TitleTextBlock.UpdateLayout();
                t = TitleTextBlock.ActualHeight;
                Canvas.SetLeft(TitleTextBlock, ActualWidth/2 - TitleTextBlock.ActualWidth/2);
            }

            if (!Uses360Mode)
            {
                LeftLabel.Text = (LabelFormatter ?? defFormatter)(From);
                RightLabel.Text = (LabelFormatter ?? defFormatter)(To);

                LeftLabel.UpdateLayout();
                RightLabel.UpdateLayout();

                LeftLabel.Visibility = Visibility.Visible;
                RightLabel.Visibility = Visibility.Visible;

                t = LeftLabel.ActualHeight;
            }
            else
            {
                LeftLabel.Visibility = Visibility.Hidden;
                RightLabel.Visibility = Visibility.Hidden;
            }

            double r, top;

            if (Uses360Mode)
            {
                r = ActualWidth > ActualHeight ? ActualHeight: ActualWidth;
                r = r/2 - 2*t ;
                top = ActualHeight/2;
            }
            else
            {
                r = ActualWidth;
                
                if (ActualWidth > ActualHeight*2)
                {
                    r = ActualHeight*2;
                }

                r = r/2 - 2*t;

                top = ActualHeight/2 + r/2;
            }

            PieBack.Radius = r;
            PieBack.InnerRadius = InnerRadius ?? r*.6;
            PieBack.RotationAngle = 270;
            PieBack.WedgeAngle = angle;

            Pie.Radius = PieBack.Radius;
            Pie.InnerRadius = PieBack.InnerRadius;
            Pie.RotationAngle = PieBack.RotationAngle;

            Canvas.SetLeft(PieBack, ActualWidth/2);
            Canvas.SetTop(PieBack, top);
            Canvas.SetLeft(Pie, ActualWidth/2);
            Canvas.SetTop(Pie, top);

            Canvas.SetTop(LeftLabel, top);
            Canvas.SetTop(RightLabel, top);
            Canvas.SetRight(LeftLabel, ActualWidth/2 + (r + PieBack.InnerRadius)/2 - LeftLabel.ActualWidth/2);
            Canvas.SetRight(RightLabel, ActualWidth/2 - (r + PieBack.InnerRadius)/2 - RightLabel.ActualWidth/2);

            MeasureTextBlock.FontSize = HighFontSize ?? Pie.InnerRadius*.4;
            MeasureTextBlock.Text = (LabelFormatter ?? defFormatter)(Value);
            MeasureTextBlock.UpdateLayout();
            Canvas.SetTop(MeasureTextBlock, top - MeasureTextBlock.ActualHeight*(Uses360Mode ? .5 : 1));
            Canvas.SetLeft(MeasureTextBlock, ActualWidth/2 - MeasureTextBlock.ActualWidth/2);

            var interpolatedColor = new Color
            {
                R = LinearInterpolation(FromColor.R, ToColor.R),
                G = LinearInterpolation(FromColor.G, ToColor.G),
                B = LinearInterpolation(FromColor.B, ToColor.B),
                A = 255
            };

            if (IsNew)
            {
                Pie.Fill = new SolidColorBrush(FromColor);
                Pie.WedgeAngle = 0;
            }

            Pie.BeginAnimation(PieSlice.WedgeAngleProperty, new DoubleAnimation(completed*angle, AnimationsSpeed));
            ((SolidColorBrush)Pie.Fill).BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(interpolatedColor, AnimationsSpeed));

            IsNew = false;
        }

        private byte LinearInterpolation(double from, double to)
        {
            var p1 = new Point(From, from);
            var p2 = new Point(To, to);

            var deltaX = p2.X - p1.X;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var m = (p2.Y - p1.Y) / (deltaX == 0 ? double.MinValue : deltaX);

            return (byte) (m * (Value - p1.X) + p1.Y);
        }
    }
}
