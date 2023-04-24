using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using GeekDesk.CustomComponent.VirtualizingWrapPanel;

namespace GeekDesk.CustomComponent.VirtualizingWrapPanel
{
    public class VirtualizingWrapPanel : VirtualizingPanelBase
    {
        #region Deprecated properties

        [Obsolete("Use SpacingMode")]
        public static readonly DependencyProperty IsSpacingEnabledProperty = DependencyProperty.Register(nameof(IsSpacingEnabled), typeof(bool), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Obsolete("Use IsSpacingEnabled")]
        public bool SpacingEnabled { get => IsSpacingEnabled; set => IsSpacingEnabled = value; }

        /// <summary>
        ///  Gets or sets a value that specifies whether the items are distributed evenly across the width (horizontal orientation) 
        ///  or height (vertical orientation). The default value is true.
        /// </summary>
        [Obsolete("Use SpacingMode")]
        public bool IsSpacingEnabled { get => (bool)GetValue(IsSpacingEnabledProperty); set => SetValue(IsSpacingEnabledProperty, value); }

        [Obsolete("Use ItemSize")]
        public Size ChildrenSize { get => ItemSize; set => ItemSize = value; }

        #endregion

        public static readonly DependencyProperty SpacingModeProperty = DependencyProperty.Register(nameof(SpacingMode), typeof(SpacingMode), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(SpacingMode.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, (obj, args) => ((VirtualizingWrapPanel)obj).Orientation_Changed()));

        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register(nameof(ItemSize), typeof(Size), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty StretchItemsProperty = DependencyProperty.Register(nameof(StretchItems), typeof(bool), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the spacing mode used when arranging the items. The default value is <see cref="SpacingMode.Uniform"/>.
        /// </summary>
        public SpacingMode SpacingMode { get => (SpacingMode)GetValue(SpacingModeProperty); set => SetValue(SpacingModeProperty, value); }

        /// <summary>
        /// Gets or sets a value that specifies the orientation in which items are arranged. The default value is <see cref="Orientation.Vertical"/>.
        /// </summary>
        public Orientation Orientation { get => (Orientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

        /// <summary>
        /// Gets or sets a value that specifies the size of the items. The default value is <see cref="Size.Empty"/>. 
        /// If the value is <see cref="Size.Empty"/> the size of the items gots measured by the first realized item.
        /// </summary>
        public Size ItemSize { get => (Size)GetValue(ItemSizeProperty); set => SetValue(ItemSizeProperty, value); }

        /// <summary>
        /// Gets or sets a value that specifies if the items get stretched to fill up remaining space. The default value is false.
        /// </summary>
        /// <remarks>
        /// The MaxWidth and MaxHeight properties of the ItemContainerStyle can be used to limit the stretching. 
        /// In this case the use of the remaining space will be determined by the SpacingMode property. 
        /// </remarks>
        public bool StretchItems { get => (bool)GetValue(StretchItemsProperty); set => SetValue(StretchItemsProperty, value); }

        protected Size childSize;

        protected int rowCount;

        protected int itemsPerRowCount;

        private void Orientation_Changed()
        {
            MouseWheelScrollDirection = Orientation == Orientation.Vertical ? ScrollDirection.Vertical : ScrollDirection.Horizontal;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateChildSize(availableSize);
            return base.MeasureOverride(availableSize);
        }

        private void UpdateChildSize(Size availableSize)
        {
            if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem
                && VirtualizingPanel.GetIsVirtualizingWhenGrouping(ItemsControl))
            {
                if (Orientation == Orientation.Vertical)
                {
                    availableSize.Width = groupItem.Constraints.Viewport.Size.Width;
                    availableSize.Width = Math.Max(availableSize.Width - (Margin.Left + Margin.Right), 0);
                }
                else
                {
                    availableSize.Height = groupItem.Constraints.Viewport.Size.Height;
                    availableSize.Height = Math.Max(availableSize.Height - (Margin.Top + Margin.Bottom), 0);
                }
            }

            if (ItemSize != Size.Empty)
            {
                childSize = ItemSize;
            }
            else if (InternalChildren.Count != 0)
            {
                childSize = InternalChildren[0].DesiredSize;
            }
            else
            {
                childSize = CalculateChildSize(availableSize);
            }

            if (double.IsInfinity(GetWidth(availableSize)))
            {
                itemsPerRowCount = Items.Count;
            }
            else
            {
                itemsPerRowCount = Math.Max(1, (int)Math.Floor(GetWidth(availableSize) / GetWidth(childSize)));
            }

            rowCount = (int)Math.Ceiling((double)Items.Count / itemsPerRowCount);
        }

        private Size CalculateChildSize(Size availableSize)
        {
            if (Items.Count == 0)
            {
                return new Size(0, 0);
            }
            var startPosition = ItemContainerGenerator.GeneratorPositionFromIndex(0);
            using (ItemContainerGenerator.StartAt(startPosition, GeneratorDirection.Forward, true))
            {
                var child = (UIElement)ItemContainerGenerator.GenerateNext();
                AddInternalChild(child);
                ItemContainerGenerator.PrepareItemContainer(child);
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                return child.DesiredSize;
            }
        }

        protected override Size CalculateExtent(Size availableSize)
        {
            double extentWidth = IsSpacingEnabled && SpacingMode != SpacingMode.None && !double.IsInfinity(GetWidth(availableSize))
                ? GetWidth(availableSize)
                : GetWidth(childSize) * itemsPerRowCount;

            if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
            {
                if (Orientation == Orientation.Vertical)
                {
                    extentWidth = Math.Max(extentWidth - (Margin.Left + Margin.Right), 0);
                }
                else
                {
                    extentWidth = Math.Max(extentWidth - (Margin.Top + Margin.Bottom), 0);
                }
            }

            double extentHeight = GetHeight(childSize) * rowCount;
            return CreateSize(extentWidth, extentHeight);
        }

        protected void CalculateSpacing(Size finalSize, out double innerSpacing, out double outerSpacing)
        {
            Size childSize = CalculateChildArrangeSize(finalSize);

            double finalWidth = GetWidth(finalSize);

            double totalItemsWidth = Math.Min(GetWidth(childSize) * itemsPerRowCount, finalWidth);
            double unusedWidth = finalWidth - totalItemsWidth;

            SpacingMode spacingMode = IsSpacingEnabled ? SpacingMode : SpacingMode.None;

            switch (spacingMode)
            {
                case SpacingMode.Uniform:
                    innerSpacing = outerSpacing = unusedWidth / (itemsPerRowCount + 1);
                    break;

                case SpacingMode.BetweenItemsOnly:
                    innerSpacing = unusedWidth / Math.Max(itemsPerRowCount - 1, 1);
                    outerSpacing = 0;
                    break;

                case SpacingMode.StartAndEndOnly:
                    innerSpacing = 0;
                    outerSpacing = unusedWidth / 2;
                    break;

                case SpacingMode.None:
                default:
                    innerSpacing = 0;
                    outerSpacing = 0;
                    break;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offsetX = GetX(Offset);
            double offsetY = GetY(Offset);

            /* When the items owner is a group item offset is handled by the parent panel. */
            if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
            {
                offsetY = 0;
            }

            Size childSize = CalculateChildArrangeSize(finalSize);

            CalculateSpacing(finalSize, out double innerSpacing, out double outerSpacing);

            for (int childIndex = 0; childIndex < InternalChildren.Count; childIndex++)
            {
                UIElement child = InternalChildren[childIndex];

                int itemIndex = GetItemIndexFromChildIndex(childIndex);

                int columnIndex = itemIndex % itemsPerRowCount;
                int rowIndex = itemIndex / itemsPerRowCount;

                double x = outerSpacing + columnIndex * (GetWidth(childSize) + innerSpacing);
                double y = rowIndex * GetHeight(childSize);

                if (GetHeight(finalSize) == 0.0)
                {
                    /* When the parent panel is grouping and a cached group item is not 
                     * in the viewport it has no valid arrangement. That means that the 
                     * height/width is 0. Therefore the items should not be visible so 
                     * that they are not falsely displayed. */
                    child.Arrange(new Rect(0, 0, 0, 0));
                }
                else
                {
                    child.Arrange(CreateRect(x - offsetX, y - offsetY, childSize.Width, childSize.Height));
                }
            }

            return finalSize;
        }

        protected Size CalculateChildArrangeSize(Size finalSize)
        {
            if (StretchItems)
            {
                if (Orientation == Orientation.Vertical)
                {
                    double childMaxWidth = ReadItemContainerStyle(MaxWidthProperty, double.PositiveInfinity);
                    double maxPossibleChildWith = finalSize.Width / itemsPerRowCount;
                    double childWidth = Math.Min(maxPossibleChildWith, childMaxWidth);
                    return new Size(childWidth, childSize.Height);
                }
                else
                {
                    double childMaxHeight = ReadItemContainerStyle(MaxHeightProperty, double.PositiveInfinity);
                    double maxPossibleChildHeight = finalSize.Height / itemsPerRowCount;
                    double childHeight = Math.Min(maxPossibleChildHeight, childMaxHeight);
                    return new Size(childSize.Width, childHeight);
                }
            }
            else
            {
                return childSize;
            }
        }

        private T ReadItemContainerStyle<T>(DependencyProperty property, T fallbackValue) 
        {
            var value = ItemsControl.ItemContainerStyle?.Setters.OfType<Setter>()
                .FirstOrDefault(setter => setter.Property == property)?.Value;
            return (T)(value ?? fallbackValue);
        }

        protected override ItemRange UpdateItemRange()
        {
            if (!IsVirtualizing)
            {
                return new ItemRange(0, Items.Count - 1);
            }

            int startIndex;
            int endIndex;

            if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
            {
                if (!VirtualizingPanel.GetIsVirtualizingWhenGrouping(ItemsControl))
                {
                    return new ItemRange(0, Items.Count - 1);
                }

                var offset = new Point(Offset.X, groupItem.Constraints.Viewport.Location.Y);

                int offsetRowIndex;
                double offsetInPixel;

                int rowCountInViewport;

                if (ScrollUnit == ScrollUnit.Item)
                {
                    offsetRowIndex = GetY(offset) >= 1 ? (int)GetY(offset) - 1 : 0; // ignore header
                    offsetInPixel = offsetRowIndex * GetHeight(childSize);
                }
                else
                {
                    offsetInPixel = Math.Min(Math.Max(GetY(offset) - GetHeight(groupItem.HeaderDesiredSizes.PixelSize), 0), GetHeight(Extent));
                    offsetRowIndex = GetRowIndex(offsetInPixel);
                }

                double viewportHeight = Math.Min(GetHeight(Viewport), Math.Max(GetHeight(Extent) - offsetInPixel, 0));

                rowCountInViewport = (int)Math.Ceiling((offsetInPixel + viewportHeight) / GetHeight(childSize)) - (int)Math.Floor(offsetInPixel / GetHeight(childSize));

                startIndex = offsetRowIndex * itemsPerRowCount;
                endIndex = Math.Min(((offsetRowIndex + rowCountInViewport) * itemsPerRowCount) - 1, Items.Count - 1);

                if (CacheLengthUnit == VirtualizationCacheLengthUnit.Pixel)
                {
                    double cacheBeforeInPixel = Math.Min(CacheLength.CacheBeforeViewport, offsetInPixel);
                    double cacheAfterInPixel = Math.Min(CacheLength.CacheAfterViewport, GetHeight(Extent) - viewportHeight - offsetInPixel);
                    int rowCountInCacheBefore = (int)(cacheBeforeInPixel / GetHeight(childSize));
                    int rowCountInCacheAfter = ((int)Math.Ceiling((offsetInPixel + viewportHeight + cacheAfterInPixel) / GetHeight(childSize))) - (int)Math.Ceiling((offsetInPixel + viewportHeight) / GetHeight(childSize));
                    startIndex = Math.Max(startIndex - rowCountInCacheBefore * itemsPerRowCount, 0);
                    endIndex = Math.Min(endIndex + rowCountInCacheAfter * itemsPerRowCount, Items.Count - 1);
                }
                else if (CacheLengthUnit == VirtualizationCacheLengthUnit.Item)
                {
                    startIndex = Math.Max(startIndex - (int)CacheLength.CacheBeforeViewport, 0);
                    endIndex = Math.Min(endIndex + (int)CacheLength.CacheAfterViewport, Items.Count - 1);
                }
            }
            else
            {
                double viewportSartPos = GetY(Offset);
                double viewportEndPos = GetY(Offset) + GetHeight(Viewport);

                if (CacheLengthUnit == VirtualizationCacheLengthUnit.Pixel)
                {
                    viewportSartPos = Math.Max(viewportSartPos - CacheLength.CacheBeforeViewport, 0);
                    viewportEndPos = Math.Min(viewportEndPos + CacheLength.CacheAfterViewport, GetHeight(Extent));
                }

                int startRowIndex = GetRowIndex(viewportSartPos);
                startIndex = startRowIndex * itemsPerRowCount;

                int endRowIndex = GetRowIndex(viewportEndPos);
                endIndex = Math.Min(endRowIndex * itemsPerRowCount + (itemsPerRowCount - 1), Items.Count - 1);

                if (CacheLengthUnit == VirtualizationCacheLengthUnit.Page)
                {
                    int itemsPerPage = endIndex - startIndex + 1;
                    startIndex = Math.Max(startIndex - (int)CacheLength.CacheBeforeViewport * itemsPerPage, 0);
                    endIndex = Math.Min(endIndex + (int)CacheLength.CacheAfterViewport * itemsPerPage, Items.Count - 1);
                }
                else if (CacheLengthUnit == VirtualizationCacheLengthUnit.Item)
                {
                    startIndex = Math.Max(startIndex - (int)CacheLength.CacheBeforeViewport, 0);
                    endIndex = Math.Min(endIndex + (int)CacheLength.CacheAfterViewport, Items.Count - 1);
                }
            }

            return new ItemRange(startIndex, endIndex);
        }

        private int GetRowIndex(double location)
        {
            int calculatedRowIndex = (int)Math.Floor(location / GetHeight(childSize));
            int maxRowIndex = (int)Math.Ceiling((double)Items.Count / (double)itemsPerRowCount);
            return Math.Max(Math.Min(calculatedRowIndex, maxRowIndex), 0);
        }

        protected override void BringIndexIntoView(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"The argument {nameof(index)} must be >= 0 and < the number of items.");
            }

            if (itemsPerRowCount == 0)
            {
                throw new InvalidOperationException();
            }

            var offset = (index / itemsPerRowCount) * GetHeight(childSize);

            if (Orientation == Orientation.Horizontal)
            {
                SetHorizontalOffset(offset);
            }
            else
            {
                SetVerticalOffset(offset);
            }
        }

        protected override double GetLineUpScrollAmount()
        {
            return -Math.Min(childSize.Height * ScrollLineDeltaItem, Viewport.Height);
        }

        protected override double GetLineDownScrollAmount()
        {
            return Math.Min(childSize.Height * ScrollLineDeltaItem, Viewport.Height);
        }

        protected override double GetLineLeftScrollAmount()
        {
            return -Math.Min(childSize.Width * ScrollLineDeltaItem, Viewport.Width);
        }

        protected override double GetLineRightScrollAmount()
        {
            return Math.Min(childSize.Width * ScrollLineDeltaItem, Viewport.Width);
        }

        protected override double GetMouseWheelUpScrollAmount()
        {
            return -Math.Min(childSize.Height * MouseWheelDeltaItem, Viewport.Height);
        }

        protected override double GetMouseWheelDownScrollAmount()
        {
            return Math.Min(childSize.Height * MouseWheelDeltaItem, Viewport.Height);
        }

        protected override double GetMouseWheelLeftScrollAmount()
        {
            return -Math.Min(childSize.Width * MouseWheelDeltaItem, Viewport.Width);
        }

        protected override double GetMouseWheelRightScrollAmount()
        {
            return Math.Min(childSize.Width * MouseWheelDeltaItem, Viewport.Width);
        }

        protected override double GetPageUpScrollAmount()
        {
            return -Viewport.Height;
        }

        protected override double GetPageDownScrollAmount()
        {
            return Viewport.Height;
        }

        protected override double GetPageLeftScrollAmount()
        {
            return -Viewport.Width;
        }

        protected override double GetPageRightScrollAmount()
        {
            return Viewport.Width;
        }

        /* orientation aware helper methods */

        protected double GetX(Point point) => Orientation == Orientation.Vertical ? point.X : point.Y;
        protected double GetY(Point point) => Orientation == Orientation.Vertical ? point.Y : point.X;

        protected double GetWidth(Size size) => Orientation == Orientation.Vertical ? size.Width : size.Height;
        protected double GetHeight(Size size) => Orientation == Orientation.Vertical ? size.Height : size.Width;

        protected Size CreateSize(double width, double height) => Orientation == Orientation.Vertical ? new Size(width, height) : new Size(height, width);
        protected Rect CreateRect(double x, double y, double width, double height) => Orientation == Orientation.Vertical ? new Rect(x, y, width, height) : new Rect(y, x, width, height);
    }
}
