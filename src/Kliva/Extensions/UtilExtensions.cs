﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Foundation = Windows.Foundation;

namespace Kliva.Extensions
{
    public static class UtilExtensions
    {
        public static float ToFloat(this double value) => (float)value;

        public static int ToInt(this float value) => (int)value;

        public static List<FrameworkElement> Children(this DependencyObject parent)
        {
            var list = new List<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement)
                {
                    list.Add(child as FrameworkElement);
                }

                list.AddRange(Children(child));
            }

            return list;
        }

        public static T GetChildByName<T>(this DependencyObject parent, string name)
        {
            var childControls = Children(parent);
            var controls = childControls.OfType<FrameworkElement>();

            if (controls == null)
            {
                return default(T);
            }

            var control = controls
                .Where(x => x.Name.Equals(name))
                .Cast<T>()
                .First();

            return control;
        }

        public static void ScrollToElement(this ScrollViewer scrollViewer, UIElement element,
            bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null)
        {
            var transform = element.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
            }
        }

        //public static void Seek(this ScalarKeyFrameAnimation animation, float from, float to, double duration)
        //{
        //    CompositionScopedBatch batch = null;
        //    batch.
        //    batch = animation.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        //    batch.Completed += (s, e) => completed();

        //    visual.StartAnimation($"Offset.{orientation.ToString()}", compositor.CreateScalarKeyFrameAnimation(to, duration, easing));

        //    batch.End();
        //}

        public static CompositionPropertySet ScrollProperties(this ScrollViewer scrollViewer) => 
            ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

        public static Visual Visual(this UIElement element) => 
            ElementCompositionPreview.GetElementVisual(element);

        public static void SetChildVisual(this UIElement element, Visual childVisual) => 
            ElementCompositionPreview.SetElementChildVisual(element, childVisual);

        public static ContainerVisual ContainerVisual(this UIElement element)
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }

        public static FlickDirection FlickDirection(this ManipulationCompletedRoutedEventArgs e)
        {
            if (!e.IsInertial)
            {
                return global::Kliva.FlickDirection.None;
            }

            var x = e.Cumulative.Translation.X;
            var y = e.Cumulative.Translation.Y;

            if (Math.Abs(x) > Math.Abs(y))
            {
                return x > 0 ? global::Kliva.FlickDirection.Right : global::Kliva.FlickDirection.Left;
            }

            return y > 0 ? global::Kliva.FlickDirection.Down : global::Kliva.FlickDirection.Up;
        }

        public static void FillAnimation(this ManipulationCompletedRoutedEventArgs e, double fullDimension,
            Action forward, Action backward,
            AnimationAxis orientation = AnimationAxis.Y, double ratio = 0.5)
        {
            var translation = e.Cumulative.Translation;
            var distance = orientation == AnimationAxis.X ? translation.X : translation.Y;

            if (distance >= fullDimension * ratio)
            {
                forward();
            }
            else
            {
                backward();
            }
        }

        public static float MovementRatioOnYAxis(this Point translation, double height)
        {
            var ratio = translation.Y / height;
            ratio = ratio < 0 ? 0 : ratio;

            return ratio.ToFloat();
        }

        public static Point RelativePosition(this UIElement element, UIElement other) => 
            element.TransformToVisual(other).TransformPoint(new Point(0, 0));

        public static float OffsetX(this UIElement element, UIElement other)
        {
            var position = element.RelativePosition(other);
            return position.X.ToFloat();
        }

        public static float OffsetY(this UIElement element, UIElement other)
        {
            var position = element.RelativePosition(other);
            return position.Y.ToFloat();
        }

        public static int Create(this Random random, int min, int max,
            Func<int, bool> regenerateIfMet = null, int regenrationMaxCount = 5)
        {
            var value = random.Next(min, max);

            if (regenerateIfMet != null)
            {
                var i = 0;
                while (i < regenrationMaxCount && regenerateIfMet(value))
                {
                    value = random.Next(min, max);
                    i++;
                }

                return value;
            }

            return value;
        }

        public static bool IsFullyVisibile(this FrameworkElement element, FrameworkElement parent)
        {
            if (element == null || parent == null)
                return false;

            if (element.Visibility != Visibility.Visible)
                return false;

            var elementBounds = element.TransformToVisual(parent).TransformBounds(new Foundation.Rect(0, 0, element.ActualWidth, element.ActualHeight));
            var containerBounds = new Foundation.Rect(0, 0, parent.ActualWidth, parent.ActualHeight);

            var originalElementWidth = elementBounds.Width;
            var originalElementHeight = elementBounds.Height;

            elementBounds.Intersect(containerBounds);

            var newElementWidth = elementBounds.Width;
            var newElementHeight = elementBounds.Height;

            return originalElementWidth.Equals(newElementWidth) && originalElementHeight.Equals(newElementHeight);
        }

        public static void ScrollToElement(this ScrollViewer scrollViewer, FrameworkElement element,
            bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null, bool bringToTopOrLeft = true)
        {
            if (!bringToTopOrLeft && element.IsFullyVisibile(scrollViewer))
                return;

            var contentArea = (FrameworkElement)scrollViewer.Content;
            var position = element.RelativePosition(contentArea);

            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
            }
        }

        public static Task ChangeViewAsync(this ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, float? zoomFactor, bool disableAniamtion)
        {
            var taskSource = new TaskCompletionSource<bool>();
            EventHandler<ScrollViewerViewChangedEventArgs> onViewChanged = null;

            onViewChanged = (sender, e) =>
            {
                if (e.IsIntermediate)
                    return;

                scrollViewer.ViewChanged -= onViewChanged;
                taskSource.SetResult(true);
            };

            scrollViewer.ViewChanged += onViewChanged;
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, zoomFactor, disableAniamtion);

            return taskSource.Task;
        }

        public static async Task ScrollToElementAsync(this ScrollViewer scrollViewer, FrameworkElement element,
            bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null, bool bringToTopOrLeft = true)
        {
            if (!bringToTopOrLeft && element.IsFullyVisibile(scrollViewer))
                return;

            var contentArea = (FrameworkElement)scrollViewer.Content;
            var position = element.RelativePosition(contentArea);

            if (isVerticalScrolling)
            {
                await scrollViewer.ChangeViewAsync(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                await scrollViewer.ChangeViewAsync(position.X, null, zoomFactor, !smoothScrolling);
            }
        }

        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static BitmapImage ToBitmapImage(this string uri) => new BitmapImage(new Uri(uri));
    }
}
