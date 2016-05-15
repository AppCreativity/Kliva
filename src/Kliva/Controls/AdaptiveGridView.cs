﻿using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    /// <summary>
    /// An adaptive GridView that restricts item's aspect ratio to MinItemWidth and MinItemHeight values
    /// </summary>
    public class AdaptiveGridView : GridView
    {
        #region DependencyProperties

        /// <summary>
        /// Minimum height for item
        /// </summary>
        public double MinItemHeight
        {
            get { return (double)GetValue(MinItemHeightProperty); }
            set { SetValue(MinItemHeightProperty, value); }
        }

        public static readonly DependencyProperty MinItemHeightProperty = DependencyProperty.Register("MinItemHeight", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(1.0, (s, a) =>
        {
            if (!double.IsNaN((double)a.NewValue))
            {
                ((AdaptiveGridView)s).InvalidateMeasure();
            }
        }));

        /// <summary>
        /// Minimum width for item (must be greater than zero)
        /// </summary>
        public double MinItemWidth
        {
            get { return (double)GetValue(MinimumItemWidthProperty); }
            set { SetValue(MinimumItemWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumItemWidthProperty = DependencyProperty.Register("MinItemWidth", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(1.0, (s, a) =>
        {
            if (!Double.IsNaN((double)a.NewValue))
            {
                ((AdaptiveGridView)s).InvalidateMeasure();
            }
        }));

        #endregion

        public AdaptiveGridView()
        {
            if (ItemContainerStyle == null)
                ItemContainerStyle = new Style(typeof(GridViewItem));

            ItemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));

            Loaded += AdaptiveGridView_Loaded;
        }

        private void AdaptiveGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsPanelRoot != null)
            {
                InvalidateMeasure();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var panel = ItemsPanelRoot as ItemsWrapGrid;
            if (panel != null)
            {
                if (MinItemWidth == 0 || MinItemHeight == 0)
                {
                    throw new ArgumentException("You need to set MinItemHeight and MinItemWidth to a value greater than 0");
                }

                var availableWidth = finalSize.Width - (Padding.Right + Padding.Left);

                var numColumns = Math.Floor(availableWidth / MinItemWidth);
                numColumns = numColumns == 0 ? 1 : numColumns;

                //Not used yet (for horizontal scrolling scenarios)
                //var numRows = Math.Ceiling(this.Items.Count / numColumns);

                var itemWidth = availableWidth / numColumns;
                var aspectRatio = MinItemHeight / MinItemWidth;
                var itemHeight = itemWidth * aspectRatio;

                panel.ItemWidth = itemWidth;
                panel.ItemHeight = itemHeight;
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
