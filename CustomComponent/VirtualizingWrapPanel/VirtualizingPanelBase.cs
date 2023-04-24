using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace GeekDesk.CustomComponent.VirtualizingWrapPanel
{
    public abstract class VirtualizingPanelBase : VirtualizingPanel, IScrollInfo
    {
        public static readonly DependencyProperty ScrollLineDeltaProperty = DependencyProperty.Register(nameof(ScrollLineDelta), typeof(double), typeof(VirtualizingPanelBase), new FrameworkPropertyMetadata(16.0));
        public static readonly DependencyProperty MouseWheelDeltaProperty = DependencyProperty.Register(nameof(MouseWheelDelta), typeof(double), typeof(VirtualizingPanelBase), new FrameworkPropertyMetadata(48.0));
        public static readonly DependencyProperty ScrollLineDeltaItemProperty = DependencyProperty.Register(nameof(ScrollLineDeltaItem), typeof(int), typeof(VirtualizingPanelBase), new FrameworkPropertyMetadata(1));
        public static readonly DependencyProperty MouseWheelDeltaItemProperty = DependencyProperty.Register(nameof(MouseWheelDeltaItem), typeof(int), typeof(VirtualizingPanelBase), new FrameworkPropertyMetadata(3));
        private ScrollViewer scrollOwner;

        public ScrollViewer GetScrollOwner()
        {
            return scrollOwner;
        }

        public void SetScrollOwner(ScrollViewer value)
        {
            scrollOwner = value;
        }

        public bool CanVerticallyScroll { get; set; }
        public bool CanHorizontallyScroll { get; set; }

        protected override bool CanHierarchicallyScrollAndVirtualizeCore => true;

        /// <summary>
        /// Scroll line delta for pixel based scrolling. The default value is 16 dp.
        /// </summary>
        public double ScrollLineDelta { get => (double)GetValue(ScrollLineDeltaProperty); set => SetValue(ScrollLineDeltaProperty, value); }

        /// <summary>
        /// Mouse wheel delta for pixel based scrolling. The default value is 48 dp.
        /// </summary>        
        public double MouseWheelDelta { get => (double)GetValue(MouseWheelDeltaProperty); set => SetValue(MouseWheelDeltaProperty, value); }

        /// <summary>
        /// Scroll line delta for item based scrolling. The default value is 1 item.
        /// </summary>
        public double ScrollLineDeltaItem { get => (int)GetValue(ScrollLineDeltaItemProperty); set => SetValue(ScrollLineDeltaItemProperty, value); }

        /// <summary>
        /// Mouse wheel delta for item based scrolling. The default value is 3 items.
        /// </summary> 
        public int MouseWheelDeltaItem { get => (int)GetValue(MouseWheelDeltaItemProperty); set => SetValue(MouseWheelDeltaItemProperty, value); }

        protected ScrollUnit ScrollUnit => GetScrollUnit(ItemsControl);

        /// <summary>
        /// The direction in which the panel scrolls when user turns the mouse wheel.
        /// </summary>
        protected ScrollDirection MouseWheelScrollDirection { get; set; } = ScrollDirection.Vertical;


        protected bool IsVirtualizing => GetIsVirtualizing(ItemsControl);

        protected VirtualizationMode VirtualizationMode => GetVirtualizationMode(ItemsControl);

        /// <summary>
        /// Returns true if the panel is in VirtualizationMode.Recycling, otherwise false.
        /// </summary>
        protected bool IsRecycling => VirtualizationMode == VirtualizationMode.Recycling;

        /// <summary>
        /// The cache length before and after the viewport. 
        /// </summary>
        protected VirtualizationCacheLength CacheLength { get; private set; }

        /// <summary>
        /// The Unit of the cache length. Can be Pixel, Item or Page. 
        /// When the ItemsOwner is a group item it can only be pixel or item.
        /// </summary>
        protected VirtualizationCacheLengthUnit CacheLengthUnit { get; private set; }


        /// <summary>
        /// The ItemsControl (e.g. ListView).
        /// </summary>
        protected ItemsControl ItemsControl => ItemsControl.GetItemsOwner(this);

        /// <summary>
        /// The ItemsControl (e.g. ListView) or if the ItemsControl is grouping a GroupItem.
        /// </summary>
        protected DependencyObject ItemsOwner
        {
            get
            {
                if (ItemsOwner1 is null)
                {
                    /* Use reflection to access internal method because the public 
                     * GetItemsOwner method does always return the itmes control instead 
                     * of the real items owner for example the group item when grouping */
                    MethodInfo getItemsOwnerInternalMethod = typeof(ItemsControl).GetMethod(
                        "GetItemsOwnerInternal",
                        BindingFlags.Static | BindingFlags.NonPublic,
                        null,
                        new Type[] { typeof(DependencyObject) },
                        null
                    );
                    ItemsOwner1 = (DependencyObject)getItemsOwnerInternalMethod.Invoke(null, new object[] { this });
                }
                return ItemsOwner1;
            }
        }
        private DependencyObject _itemsOwner;

        protected ReadOnlyCollection<object> Items => ((ItemContainerGenerator)ItemContainerGenerator).Items;

        protected new IRecyclingItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (_itemContainerGenerator is null)
                {
                    /* Because of a bug in the framework the ItemContainerGenerator 
                     * is null until InternalChildren accessed at least one time. */
                    var children = InternalChildren;
                    _itemContainerGenerator = (IRecyclingItemContainerGenerator)base.ItemContainerGenerator;
                }
                return _itemContainerGenerator;
            }
        }
        private IRecyclingItemContainerGenerator _itemContainerGenerator;

        public double ExtentWidth => Extent.Width;
        public double ExtentHeight => Extent.Height;
        protected Size Extent { get; private set; } = new Size(0, 0);

        public double HorizontalOffset => Offset.X;
        public double VerticalOffset => Offset.Y;
        protected Size Viewport { get; private set; } = new Size(0, 0);

        public double ViewportWidth => Viewport.Width;
        public double ViewportHeight => Viewport.Height;
        protected Point Offset { get; private set; } = new Point(0, 0);

        /// <summary>
        /// The range of items that a realized in viewport or cache.
        /// </summary>
        protected ItemRange ItemRange { get; set; }
        public ScrollViewer ScrollOwner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DependencyObject ItemsOwner1 { get => _itemsOwner; set => _itemsOwner = value; }

        private Visibility previousVerticalScrollBarVisibility = Visibility.Collapsed;
        private Visibility previousHorizontalScrollBarVisibility = Visibility.Collapsed;

        protected virtual void UpdateScrollInfo(Size availableSize, Size extent)
        {
            bool invalidateScrollInfo = false;

            if (extent != Extent)
            {
                Extent = extent;
                invalidateScrollInfo = true;

            }
            if (availableSize != Viewport)
            {
                Viewport = availableSize;
                invalidateScrollInfo = true;
            }

            if (ViewportHeight != 0 && VerticalOffset != 0 && VerticalOffset + ViewportHeight + 1 >= ExtentHeight)
            {
                Offset = new Point(Offset.X, extent.Height - availableSize.Height);
                invalidateScrollInfo = true;
            }
            if (ViewportWidth != 0 && HorizontalOffset != 0 && HorizontalOffset + ViewportWidth + 1 >= ExtentWidth)
            {
                Offset = new Point(extent.Width - availableSize.Width, Offset.Y);
                invalidateScrollInfo = true;
            }

            if (invalidateScrollInfo)
            {
                GetScrollOwner()?.InvalidateScrollInfo();
            }
        }

        public virtual Rect MakeVisible(Visual visual, Rect rectangle)
        {
            Point pos = visual.TransformToAncestor(this).Transform(Offset);

            double scrollAmountX = 0;
            double scrollAmountY = 0;

            if (pos.X < Offset.X)
            {
                scrollAmountX = -(Offset.X - pos.X);
            }
            else if ((pos.X + rectangle.Width) > (Offset.X + Viewport.Width))
            {
                double notVisibleX = (pos.X + rectangle.Width) - (Offset.X + Viewport.Width);
                double maxScrollX = pos.X - Offset.X; // keep left of the visual visible
                scrollAmountX = Math.Min(notVisibleX, maxScrollX);
            }

            if (pos.Y < Offset.Y)
            {
                scrollAmountY = -(Offset.Y - pos.Y);
            }
            else if ((pos.Y + rectangle.Height) > (Offset.Y + Viewport.Height))
            {
                double notVisibleY = (pos.Y + rectangle.Height) - (Offset.Y + Viewport.Height);
                double maxScrollY = pos.Y - Offset.Y; // keep top of the visual visible
                scrollAmountY = Math.Min(notVisibleY, maxScrollY);
            }

            SetHorizontalOffset(Offset.X + scrollAmountX);
            SetVerticalOffset(Offset.Y + scrollAmountY);

            double visibleRectWidth = Math.Min(rectangle.Width, Viewport.Width);
            double visibleRectHeight = Math.Min(rectangle.Height, Viewport.Height);

            return new Rect(scrollAmountX, scrollAmountY, visibleRectWidth, visibleRectHeight);
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.OldPosition.Index, args.ItemUICount);
                    break;
            }
        }

        protected int GetItemIndexFromChildIndex(int childIndex)
        {
            var generatorPosition = GetGeneratorPositionFromChildIndex(childIndex);
            return ItemContainerGenerator.IndexFromGeneratorPosition(generatorPosition);
        }

        protected virtual GeneratorPosition GetGeneratorPositionFromChildIndex(int childIndex)
        {
            return new GeneratorPosition(childIndex, 0);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            /* Sometimes when scrolling the scrollbar gets hidden without any reason. In this case the "IsMeasureValid" 
             * property of the ScrollOwner is false. To prevent a infinite circle the mesasure call is ignored. */
            if (GetScrollOwner() != null)
            {
                bool verticalScrollBarGotHidden = GetScrollOwner().VerticalScrollBarVisibility == ScrollBarVisibility.Auto
                    && GetScrollOwner().ComputedVerticalScrollBarVisibility != Visibility.Visible
                    && GetScrollOwner().ComputedVerticalScrollBarVisibility != previousVerticalScrollBarVisibility;

                bool horizontalScrollBarGotHidden = GetScrollOwner().HorizontalScrollBarVisibility == ScrollBarVisibility.Auto
                   && GetScrollOwner().ComputedHorizontalScrollBarVisibility != Visibility.Visible
                   && GetScrollOwner().ComputedHorizontalScrollBarVisibility != previousHorizontalScrollBarVisibility;

                previousVerticalScrollBarVisibility = GetScrollOwner().ComputedVerticalScrollBarVisibility;
                previousHorizontalScrollBarVisibility = GetScrollOwner().ComputedHorizontalScrollBarVisibility;

                if (!GetScrollOwner().IsMeasureValid && verticalScrollBarGotHidden || horizontalScrollBarGotHidden)
                {
                    return availableSize;
                }
            }

            var groupItem = ItemsOwner as IHierarchicalVirtualizationAndScrollInfo;

            Size extent;
            Size desiredSize;

            if (groupItem != null)
            {
                /* If the ItemsOwner is a group item the availableSize is ifinity. 
                 * Therfore the vieport size provided by the group item is used. */
                var viewportSize = groupItem.Constraints.Viewport.Size;
                var headerSize = groupItem.HeaderDesiredSizes.PixelSize;
                double availableWidth = Math.Max(viewportSize.Width - 5, 0); // left margin of 5 dp
                double availableHeight = Math.Max(viewportSize.Height - headerSize.Height, 0);
                availableSize = new Size(availableWidth, availableHeight);

                extent = CalculateExtent(availableSize);

                desiredSize = new Size(extent.Width, extent.Height);

                Extent = extent;
                Offset = groupItem.Constraints.Viewport.Location;
                Viewport = groupItem.Constraints.Viewport.Size;
                CacheLength = groupItem.Constraints.CacheLength;
                CacheLengthUnit = groupItem.Constraints.CacheLengthUnit; // can be Item or Pixel
            }
            else
            {
                extent = CalculateExtent(availableSize);
                double desiredWidth = Math.Min(availableSize.Width, extent.Width);
                double desiredHeight = Math.Min(availableSize.Height, extent.Height);
                desiredSize = new Size(desiredWidth, desiredHeight);

                UpdateScrollInfo(desiredSize, extent);
                CacheLength = GetCacheLength(ItemsOwner);
                CacheLengthUnit = GetCacheLengthUnit(ItemsOwner); // can be Page, Item or Pixel
            }

            ItemRange = UpdateItemRange();

            RealizeItems();
            VirtualizeItems();

            return desiredSize;
        }

        /// <summary>
        /// Realizes visible and cached items.
        /// </summary>
        protected virtual void RealizeItems()
        {
            var startPosition = ItemContainerGenerator.GeneratorPositionFromIndex(ItemRange.StartIndex);

            int childIndex = startPosition.Offset == 0 ? startPosition.Index : startPosition.Index + 1;

            using (ItemContainerGenerator.StartAt(startPosition, GeneratorDirection.Forward, true))
            {
                for (int i = ItemRange.StartIndex; i <= ItemRange.EndIndex; i++, childIndex++)
                {
                    UIElement child = (UIElement)ItemContainerGenerator.GenerateNext(out bool isNewlyRealized);
                    if (isNewlyRealized || /*recycled*/!InternalChildren.Contains(child))
                    {
                        if (childIndex >= InternalChildren.Count)
                        {
                            AddInternalChild(child);
                        }
                        else
                        {
                            InsertInternalChild(childIndex, child);
                        }
                        ItemContainerGenerator.PrepareItemContainer(child);

                        child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }

                    if (child is IHierarchicalVirtualizationAndScrollInfo groupItem)
                    {
                        groupItem.Constraints = new HierarchicalVirtualizationConstraints(
                            new VirtualizationCacheLength(0),
                            VirtualizationCacheLengthUnit.Item,
                            new Rect(0, 0, ViewportWidth, ViewportHeight));
                        child.Measure(new Size(ViewportWidth, ViewportHeight));
                    }
                }
            }
        }

        /// <summary>
        /// Virtualizes (cleanups) no longer visible or cached items.
        /// </summary>
        protected virtual void VirtualizeItems()
        {
            for (int childIndex = InternalChildren.Count - 1; childIndex >= 0; childIndex--)
            {
                var generatorPosition = GetGeneratorPositionFromChildIndex(childIndex);

                int itemIndex = ItemContainerGenerator.IndexFromGeneratorPosition(generatorPosition);

                if (itemIndex != -1 && !ItemRange.Contains(itemIndex))
                {
                    if (VirtualizationMode == VirtualizationMode.Recycling)
                    {
                        ItemContainerGenerator.Recycle(generatorPosition, 1);
                    }
                    else
                    {
                        ItemContainerGenerator.Remove(generatorPosition, 1);
                    }
                    RemoveInternalChildRange(childIndex, 1);
                }
            }
        }

        /// <summary>
        /// Calculates the extent that would be needed to show all items.
        /// </summary>
        protected abstract Size CalculateExtent(Size availableSize);

        /// <summary>
        /// Calculates the item range that is visible in the viewport or cached.
        /// </summary>
        protected abstract ItemRange UpdateItemRange();

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || Viewport.Height >= Extent.Height)
            {
                offset = 0;
            }
            else if (offset + Viewport.Height >= Extent.Height)
            {
                offset = Extent.Height - Viewport.Height;
            }
            Offset = new Point(Offset.X, offset);
            GetScrollOwner()?.InvalidateScrollInfo();
            InvalidateMeasure();
        }

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || Viewport.Width >= Extent.Width)
            {
                offset = 0;
            }
            else if (offset + Viewport.Width >= Extent.Width)
            {
                offset = Extent.Width - Viewport.Width;
            }
            Offset = new Point(offset, Offset.Y);
            GetScrollOwner()?.InvalidateScrollInfo();
            InvalidateMeasure();
        }

        protected void ScrollVertical(double amount)
        {
            SetVerticalOffset(VerticalOffset + amount);
        }

        protected void ScrollHorizontal(double amount)
        {
            SetHorizontalOffset(HorizontalOffset + amount);
        }

        public void LineUp() => ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? -ScrollLineDelta : GetLineUpScrollAmount());
        public void LineDown() => ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? ScrollLineDelta : GetLineDownScrollAmount());
        public void LineLeft() => ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? -ScrollLineDelta : GetLineLeftScrollAmount());
        public void LineRight() => ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? ScrollLineDelta : GetLineRightScrollAmount());

        public void MouseWheelUp()
        {
            if (MouseWheelScrollDirection == ScrollDirection.Vertical)
            {
                ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? -MouseWheelDelta : GetMouseWheelUpScrollAmount());
            }
            else
            {
                MouseWheelLeft();
            }
        }

        public void MouseWheelDown()
        {
            if (MouseWheelScrollDirection == ScrollDirection.Vertical)
            {
                ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? MouseWheelDelta : GetMouseWheelDownScrollAmount());
            }
            else
            {
                MouseWheelRight();
            }
        }

        public void MouseWheelLeft() => ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? -MouseWheelDelta : GetMouseWheelLeftScrollAmount());
        public void MouseWheelRight() => ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? MouseWheelDelta : GetMouseWheelRightScrollAmount());

        public void PageUp() => ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? -ViewportHeight : GetPageUpScrollAmount());
        public void PageDown() => ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? ViewportHeight : GetPageDownScrollAmount());
        public void PageLeft() => ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? -ViewportHeight : GetPageLeftScrollAmount());
        public void PageRight() => ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? ViewportHeight : GetPageRightScrollAmount());

        protected abstract double GetLineUpScrollAmount();
        protected abstract double GetLineDownScrollAmount();
        protected abstract double GetLineLeftScrollAmount();
        protected abstract double GetLineRightScrollAmount();

        protected abstract double GetMouseWheelUpScrollAmount();
        protected abstract double GetMouseWheelDownScrollAmount();
        protected abstract double GetMouseWheelLeftScrollAmount();
        protected abstract double GetMouseWheelRightScrollAmount();

        protected abstract double GetPageUpScrollAmount();
        protected abstract double GetPageDownScrollAmount();
        protected abstract double GetPageLeftScrollAmount();
        protected abstract double GetPageRightScrollAmount();
    }
}
