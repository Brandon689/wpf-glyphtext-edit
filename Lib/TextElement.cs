using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lib
{
    public class TextElement : FrameworkElement
    {
        private bool _enabled;
        bool VERTICAL = true;
        internal Text? _text;
        private GlyphRun[]? _glyphRuns = null;
        public bool newline = false;
        public TextElement()
        {
            Loaded += TextElement_Loaded;
        }
        public void Resize(Size size)
        {
            RenderSize = size;
        }
        private void TextElement_Loaded(object sender, RoutedEventArgs e)
        {
            double lineheightF = 0.7;
            double lineHeightVF = 1 + lineheightF;
            string sample = @"はありません。誤解を恐れることなく、昔ながらの便宜的表現を用いるな
ら、今のわたくしは幽霊とでも言うべき存在なのです」
部屋に深い沈黙が降りた。子易さんは口元に微かな笑みを浮かべ、膝の
上で両手をごしごしと擦り合わせながら、ストーブの火を見つめていた。
この人は冗談を言っているのかもしれない、ただ私をからかっているの
かもしれない──そういう可能性が私の頭をよぎった。通常の場合であれ
ば、それは十分あり得る可能性だ。人によっては真顔で冗談を言うし、人
をからかいもする。しかしどう考えても、子易さんはそのような冗談を口
にして喜ぶタイプの人ではなかった。それになんといっても、彼は実際に
影を持っていないのだ。当たり前の話だが、冗談でちょっと影を消すとい
うわけにはいかない。";
            _text = new(FontSize, 1.22 * FontSize, 1.66 * FontSize);
            foreach (var item in sample.Split("\r\n"))
            {
                _text.Append(item, newline);
            }
            if (VERTICAL)
            {
                _glyphRuns = _text.CalculateV(newline).ToArray();
            }
            else
            {
                _glyphRuns = _text.Calculate(newline).ToArray();
            }
            InvalidateVisual();
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }

        #region DP

        public bool MouseHighlightText
        {
            get { return (bool)GetValue(MouseHighlightTextProperty); }
            set { SetValue(MouseHighlightTextProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Brush HighlightBackground
        {
            get { return (Brush)GetValue(HighlightBackgroundProperty); }
            set { SetValue(HighlightBackgroundProperty, value); }
        }

        public SolidColorBrush HighlightColor
        {
            get { return (SolidColorBrush)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(TextElement), new PropertyMetadata(0d));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("BackgroundProperty", typeof(Brush), typeof(TextElement), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.Register("HighlightBackgroundProperty", typeof(Brush), typeof(TextElement), new PropertyMetadata(Brushes.Black));


        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("ForegroundProperty", typeof(Brush), typeof(TextElement), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeightProperty", typeof(FontWeight), typeof(TextElement), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty HighlightColorProperty =
             DependencyProperty.Register("HighlightColorProperty", typeof(SolidColorBrush), typeof(TextElement), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(51, 153, 255))));

        public static readonly DependencyProperty MouseHighlightTextProperty =
            DependencyProperty.Register("MouseHighlightTextProperty", typeof(bool), typeof(TextElement), new PropertyMetadata(true));

        #endregion

        #region override

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_glyphRuns == null) return;

            _text._renderSize = RenderSize;
            //Resize(RenderSize);
            if (VERTICAL)
            {
                _glyphRuns = _text.CalculateV(newline).ToArray();
            }
            else
            {
                _glyphRuns = _text.Calculate(newline).ToArray();
            }
            drawingContext.DrawRectangle(Background, null, new Rect(0,0, this.ActualWidth, this.ActualHeight));


            //var d = _text.HighlightedTextBoundingBoxes();
            //if (d.Count() > 0)
            //{
            //    PathGeometry g = new();
            //    for (int i = 1; i < d.Count; i++)
            //    {
            //        RectangleGeometry r = new(d[i]);
            //        RectangleGeometry r2 = new(d[i - 1]);
            //        var p = Geometry.Combine(r, r2, GeometryCombineMode.Union, null);
            //        g = Geometry.Combine(p, g, GeometryCombineMode.Union, null);
            //    }
            //}

            foreach (var item in _text.HighlightedTextBoundingBoxes())
            {
                drawingContext.DrawRectangle(HighlightBackground, null, item);
                drawingContext.DrawLine(new Pen(Brushes.DarkCyan, 1), item.BottomLeft, item.BottomRight);
            }

            for (int i = 0; i < _glyphRuns.Length; i++)
            {
                var brush = Foreground;
                if (MouseHighlightText && i == _text.HoveredChunk)
                {
                    brush = HighlightColor;
                }
                drawingContext.DrawGlyphRun(brush, _glyphRuns[i]);
            }

            //foreach (var item in _text._glyphs)
            //{
            //    drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 1), item.BoundingBox);
            //}

            GeometryGroup fc = new();
            for (int i = 0; i < _glyphRuns.Length; i++)
            {
                var s = _glyphRuns[i].BuildGeometry();
                fc.Children.Add(s);
                //drawingContext.DrawGeometry(fiveColorLGB, null, s);
                
            }
            //drawingContext.DrawGeometry(null, new Pen(fiveColorLGB, 2), fc);
            //drawingContext.DrawGeometry(fiveColorLGB, null, fc);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _text.MouseDown(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_text.MouseMove(e.GetPosition(this)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _text.MouseUp(e.GetPosition(this));
            InvalidateVisual();
        }

        #endregion
    }
}
