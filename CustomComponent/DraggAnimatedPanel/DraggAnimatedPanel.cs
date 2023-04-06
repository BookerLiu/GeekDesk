/*Developed by (doiTTeam)=>doiTTeam.mail = devdoiTTeam@gmail.com*/
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DraggAnimatedPanel
{
    /// <summary>
    /// Description of DraggAnimatedPanel.
    /// </summary>
    public partial class DraggAnimatedPanel : WrapPanel
    {
        #region private vars
        Size _calculatedSize;
        bool _isNotFirstArrange = false;
        int columns, rows;
        #endregion
        static DraggAnimatedPanel()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DraggAnimatedPanel), new FrameworkPropertyMetadata(typeof(DraggAnimatedPanel)));
        }

        public DraggAnimatedPanel() : base()
        {
            this.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), false);
            this.MouseLeftButtonUp += OnMouseUp;
            this.LostMouseCapture += OnLostMouseCapture;
        }

        UIElement GetChildThatHasMouseOver()
        {
            return GetParent(Mouse.DirectlyOver as DependencyObject, (ve) => Children.Contains(ve as UIElement)) as UIElement;
        }

        Point GetItemVisualPoint(UIElement element)
        {
            TransformGroup group = (TransformGroup)element.RenderTransform;
            TranslateTransform trans = (TranslateTransform)group.Children[0];

            return new Point(trans.X, trans.Y);
        }

        int GetIndexFromPoint(double x, double y)
        {
            int columnIndex = (int)Math.Truncate(x / itemContainterWidth);
            int rowIndex = (int)Math.Truncate(y / itemContainterHeight);
            return columns * rowIndex + columnIndex;
        }
        int GetIndexFromPoint(Point p)
        {
            return GetIndexFromPoint(p.X, p.Y);
        }

        #region dependency properties		
        public static readonly DependencyProperty ItemsWidthProperty =
          DependencyProperty.Register(
                  "ItemsWidth",
                  typeof(double),
                  typeof(DraggAnimatedPanel),
                  new FrameworkPropertyMetadata(150d));

        public static readonly DependencyProperty ItemsHeightProperty =
          DependencyProperty.Register(
                  "ItemsHeight",
                  typeof(double),
                  typeof(DraggAnimatedPanel),
                  new FrameworkPropertyMetadata(60d));

        public static readonly DependencyProperty ItemSeparationProperty =
         DependencyProperty.Register(
                 "ItemSeparation",
                 typeof(Thickness),
                 typeof(DraggAnimatedPanel),
                 new FrameworkPropertyMetadata());

        // Using a DependencyProperty as the backing store for AnimationMilliseconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationMillisecondsProperty =
            DependencyProperty.Register("AnimationMilliseconds", typeof(int), typeof(DraggAnimatedPanel), new FrameworkPropertyMetadata(200));

        public static readonly DependencyProperty SwapCommandProperty =
          DependencyProperty.Register(
                  "SwapCommand",
                  typeof(ICommand),
                  typeof(DraggAnimatedPanel),
                  new FrameworkPropertyMetadata(null));

        #endregion

        #region properties
        public double ItemsWidth
        {
            get { return (double)GetValue(ItemsWidthProperty); }
            set { SetValue(ItemsWidthProperty, value); }
        }
        public double ItemsHeight
        {
            get { return (double)GetValue(ItemsHeightProperty); }
            set { SetValue(ItemsHeightProperty, value); }
        }
        public Thickness ItemSeparation
        {
            get { return (Thickness)this.GetValue(ItemSeparationProperty); }
            set { this.SetValue(ItemSeparationProperty, value); }
        }
        public int AnimationMilliseconds
        {
            get { return (int)GetValue(AnimationMillisecondsProperty); }
            set { SetValue(AnimationMillisecondsProperty, value); }
        }
        private double itemContainterHeight
        {
            get { return ItemSeparation.Top + ItemsHeight + ItemSeparation.Bottom; }
        }
        private double itemContainterWidth
        {
            get { return ItemSeparation.Left + ItemsWidth + ItemSeparation.Right; }
        }
        public ICommand SwapCommand
        {
            get { return (ICommand)GetValue(SwapCommandProperty); }
            set { SetValue(SwapCommandProperty, value); }
        }
        #endregion

        #region transformation things		
        private void AnimateAll()
        {
            //Apply exactly the same algorithm, but instide of Arrange a call AnimateTo method
            double colPosition = 0;
            double rowPosition = 0;
            foreach (UIElement child in Children)
            {
                if (child != _draggedElement)
                    AnimateTo(child, colPosition + ItemSeparation.Left, rowPosition + ItemSeparation.Top, _isNotFirstArrange ? AnimationMilliseconds : 0);
                //drag will locate dragged element
                colPosition += itemContainterWidth;
                if (colPosition + 1 > _calculatedSize.Width)
                {
                    colPosition = 0;
                    rowPosition += itemContainterHeight;
                }
            }
        }

        private void AnimateTo(UIElement child, double x, double y, int duration)
        {
            TransformGroup group = (TransformGroup)child.RenderTransform;
            TranslateTransform trans = (TranslateTransform)group.Children.First((groupElement) => groupElement is TranslateTransform);

            trans.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(x, duration));
            trans.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(y, duration));
        }

        private DoubleAnimation MakeAnimation(double to, int duration)
        {
            DoubleAnimation anim = new DoubleAnimation(to, TimeSpan.FromMilliseconds(duration));
            anim.AccelerationRatio = 0.2;
            anim.DecelerationRatio = 0.7;
            return anim;
        }
        #endregion

        #region measure
        protected override Size MeasureOverride(Size availableSize)
        {
            Size itemContainerSize = new Size(itemContainterWidth, itemContainterHeight);
            int count = 0;  //for not call it again
            foreach (UIElement child in Children)
            {
                child.Measure(itemContainerSize);
                count++;
            }
            if (availableSize.Width < itemContainterWidth)
                _calculatedSize = new Size(itemContainterWidth, count * itemContainterHeight);  //the size of nX1
            else
            {
                columns = (int)Math.Truncate(availableSize.Width / itemContainterWidth);
                rows = count / columns;
                if (count % columns != 0)
                    rows++;
                _calculatedSize = new Size(columns * itemContainterWidth, rows * itemContainterHeight);
            }
            return _calculatedSize;
        }
        #endregion

        #region arrange
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size _finalItemSize = new Size(ItemsWidth, ItemsHeight);
            //if is animated then arrange elements to 0,0, and then put them on its location using the transform
            foreach (UIElement child in InternalChildren)
            {
                // If this is the first time we've seen this child, add our transforms
                if (child.RenderTransform as TransformGroup == null)
                {
                    child.RenderTransformOrigin = new Point(0.5, 0.5);
                    TransformGroup group = new TransformGroup();
                    child.RenderTransform = group;
                    group.Children.Add(new TranslateTransform());
                }
                //locate all children in 0,0 point//TODO: use infinity and then scale each element to items size
                child.Arrange(new Rect(new Point(0, 0), _finalItemSize));       //when use transformations change to childs.DesireSize
            }
            AnimateAll();

            if (!_isNotFirstArrange)
                _isNotFirstArrange = true;

            return _calculatedSize;
        }
        #endregion

        #region Static
        //this can be an extension method
        public static DependencyObject GetParent(DependencyObject o, Func<DependencyObject, bool> matchFunction)
        {
            DependencyObject t = o;
            do
            {
                t = VisualTreeHelper.GetParent(t);
            } while (t != null && !matchFunction.Invoke(t));
            return t;
        }
        #endregion

        //TODO: Add IsEditing property
        //TODO: Add Scale transform to items for fill items area
    }
}
