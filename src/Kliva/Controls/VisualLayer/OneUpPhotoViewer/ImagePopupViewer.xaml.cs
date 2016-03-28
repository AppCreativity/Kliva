using CompositionSampleGallery;
using Kliva.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Composition;
using SamplesCommon;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace Kliva.Controls.VisualLayer
{
    public sealed partial class ImagePopupViewer : UserControl
    {
        Compositor                  _compositor;
        CompositionEffectFactory    _lightEffectFactory;
        CompositionSurfaceBrush     _normalMapBrush;
        ExpressionAnimation         _lightPositionAnimation;
        ExpressionAnimation         _lightTargetAnimation;
        CompositionEffectBrush      _crossFadeBrush;
        CompositionSurfaceBrush     _previousSurfaceBrush;
        CompositionScopedBatch      _backgroundCrossFadeBatch;
        CompositionPropertySet      _lightProperties;
        ContinuityTransition        _transition;
        Photo                       _initialPhoto;
        static ImagePopupViewer     _viewerInstance;
        static Grid                 _hostGrid;
        Func<object, bool, Uri>     _imageUriGetterFunc;

        #region Initialization
        /// <summary>
        /// Private constructor as Show() is responsible for creating an instance
        /// </summary>
        private ImagePopupViewer(Func<object, bool, Uri> photoGetter, ContinuityTransition transition, Photo photo)
        {
            this.InitializeComponent();

            _imageUriGetterFunc = photoGetter;
            _transition = transition;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this.Loaded += ImagePopupViewer_Loaded;

            // Bring the selected item into view
            _initialPhoto = photo;

            // Initialize the lighting effects
            InitializeLighting();
            BindLightPositions();

            // Hide until the content is available
            this.Opacity = 0;
            BackgroundImage.ImageOpened += BackgroundImage_FirstOpened;

            // Disable the placeholder as we'll be using a transition
            PrimaryImage.PlaceholderDelay = TimeSpan.FromMilliseconds(-1);
            BackgroundImage.PlaceholderDelay = TimeSpan.FromMilliseconds(-1);
            BackgroundImage.LoadTimeEffectHandler = SampleImageColor;
            BackgroundImage.SharedSurface = true;

            // Create a crossfade brush to animate image transitions
            IGraphicsEffect graphicsEffect = new ArithmeticCompositeEffect()
            {
                Name = "CrossFade",
                Source1Amount = 0,
                Source2Amount = 1,
                MultiplyAmount = 0,
                Source1 = new CompositionEffectSourceParameter("ImageSource"),
                Source2 = new CompositionEffectSourceParameter("ImageSource2"),
            };

            CompositionEffectFactory factory = _compositor.CreateEffectFactory(graphicsEffect, new[] { "CrossFade.Source1Amount", "CrossFade.Source2Amount" });
            _crossFadeBrush = factory.CreateBrush();

        }

        private void ImagePopupViewer_Loaded(object sender, RoutedEventArgs e)
        {
            // Kick off the transition from originating thumbnail to final position
            _transition.Start(Window.Current.Content, PrimaryImage, null, null);

            // Update the sources
            BackgroundImage.Source = new Uri(_initialPhoto.ImageLarge);
            PrimaryImage.Source = new Uri(_initialPhoto.ImageLarge);

            // Ensure the source thumbnail is in view
            ImageList.ScrollIntoView(_initialPhoto);
        }
        #endregion

        #region BackgroundImage

        private void BackgroundImage_FirstOpened(object sender, RoutedEventArgs e)
        {
            // Image loaded, let's show the content
            this.Opacity = 1;

            // Show the content now that we should have something.
            ScalarKeyFrameAnimation fadeInAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeInAnimation.InsertKeyFrame(0, 0);
            fadeInAnimation.InsertKeyFrame(1, 1);
            fadeInAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            BackgroundImage.SpriteVisual.StartAnimation("Opacity", fadeInAnimation);
            ElementCompositionPreview.GetElementVisual(ImageList).StartAnimation("Opacity", fadeInAnimation);

            // Start a slow UV scale to create movement in the background image
            Vector2KeyFrameAnimation scaleAnimation = _compositor.CreateVector2KeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(0, new Vector2(1.1f, 1.1f));
            scaleAnimation.InsertKeyFrame(.5f, new Vector2(2.0f, 2.0f));
            scaleAnimation.InsertKeyFrame(1, new Vector2(1.1f, 1.1f));
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(40000);
            scaleAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            CompositionDrawingSurface surface = (CompositionDrawingSurface)BackgroundImage.SurfaceBrush.Surface;
            BackgroundImage.SurfaceBrush.CenterPoint = new Vector2((float)surface.Size.Width, (float)surface.Size.Height) * .5f;
            BackgroundImage.SurfaceBrush.StartAnimation("Scale", scaleAnimation);

            // Start the animation of the cross-fade brush so they're in sync
            _previousSurfaceBrush = _compositor.CreateSurfaceBrush();
            _previousSurfaceBrush.StartAnimation("Scale", scaleAnimation);

            BackgroundImage.ImageOpened -= BackgroundImage_FirstOpened;
        }

        private void BackgroundImage_ImageChanged(object sender, RoutedEventArgs e)
        {
            if (_backgroundCrossFadeBatch == null)
            {
                TimeSpan duration = TimeSpan.FromMilliseconds(1000);

                // Create the animations for cross-fading
                ScalarKeyFrameAnimation fadeInAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeInAnimation.InsertKeyFrame(0, 0);
                fadeInAnimation.InsertKeyFrame(1, 1);
                fadeInAnimation.Duration = duration;

                ScalarKeyFrameAnimation fadeOutAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeOutAnimation.InsertKeyFrame(0, 1);
                fadeOutAnimation.InsertKeyFrame(1, 0);
                fadeOutAnimation.Duration = duration;

                // Create a batch object so we can cleanup when the cross-fade completes.
                _backgroundCrossFadeBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

                // Set the sources
                _crossFadeBrush.SetSourceParameter("ImageSource", BackgroundImage.SurfaceBrush);
                _crossFadeBrush.SetSourceParameter("ImageSource2", _previousSurfaceBrush);

                // Animate the source amounts to fade between
                _crossFadeBrush.StartAnimation("CrossFade.Source1Amount", fadeInAnimation);
                _crossFadeBrush.StartAnimation("CrossFade.Source2Amount", fadeOutAnimation);

                // Update the image to use the cross fade brush
                BackgroundImage.Brush = _crossFadeBrush;

                _backgroundCrossFadeBatch.Completed += (fadesender, args) =>
                {
                    BackgroundImage.Brush = BackgroundImage.SurfaceBrush;

                    // Dispose the image
                    ((CompositionDrawingSurface)_previousSurfaceBrush.Surface).Dispose();
                    _previousSurfaceBrush.Surface = null;

                    // Clear out the batch
                    _backgroundCrossFadeBatch = null;
                };
                _backgroundCrossFadeBatch.End();
            }

            // Unhook the handler
            BackgroundImage.ImageOpened -= BackgroundImage_ImageChanged;
        }
        #endregion

        #region Public Properties

        public object ItemsSource
        {
            get { return ImageList.ItemsSource; }
            set { ImageList.ItemsSource = value; }
        }

        #endregion
        
        #region Color Helpers
        private Color ExtractPredominantColor(Color[] colors, Size size)
        {
            Dictionary<uint, int> dict = new Dictionary<uint, int>();
            uint maxColor = 0xff000000;

            // Take a small sampling of the decoded pixels, looking for the most common color
            int pixelSamples = Math.Min(2000, colors.Length);
            int skipPixels = colors.Length / pixelSamples;

            for (int pixel = colors.Length - 1; pixel >= 0; pixel -= skipPixels)
            {
                Color c = colors[pixel];

                // Quantize the colors to bucket the groupings better
                c.R -= (byte)(c.R % 10);
                c.G -= (byte)(c.G % 10);
                c.B -= (byte)(c.B % 10);

                // Determine the saturation and value for the color
                int max = Math.Max(c.R, Math.Max(c.G, c.B));
                int min = Math.Min(c.R, Math.Min(c.G, c.B));
                int saturation = (int)(((max == 0) ? 0 : (1f - (1f * min / max))) * 255);
                int value = (int)((max / 255f) * 255);

                if (c.A > 0)
                {
                    uint color = (uint)((255 << 24) | (c.R << 16) | (c.G << 8) | (c.B << 0));

                    // Weigh saturated, high value colors more heavily
                    int weight = saturation + value;

                    if (dict.ContainsKey(color))
                    {
                        dict[color] += weight;
                    }
                    else
                    {
                        dict.Add(color, weight);
                    }
                }
            }

            // Determine the predominant color
            int maxValue = 0;
            foreach (KeyValuePair<uint, int> pair in dict)
            {
                if (pair.Value > maxValue)
                {
                    maxColor = pair.Key;
                    maxValue = pair.Value;
                }
            }

            // Convert to the final color value
            return Color.FromArgb((byte)(maxColor >> 24), (byte)(maxColor >> 16),
                                   (byte)(maxColor >> 8), (byte)(maxColor >> 0));
        }

        private CompositionDrawingSurface SampleImageColor(CanvasBitmap bitmap, CompositionGraphicsDevice device, Size sizeTarget)
        {
            // Extract the color to tint the blur with
            Color predominantColor = ExtractPredominantColor(bitmap.GetPixelColors(), bitmap.Size);

            Size sizeSource = bitmap.Size;
            if (sizeTarget.IsEmpty)
            {
                sizeTarget = sizeSource;
            }

            // Create a heavily blurred version of the image
            GaussianBlurEffect blurEffect = new GaussianBlurEffect()
            {
                Source = bitmap,
                BlurAmount = 20.0f
            };

            CompositionDrawingSurface surface = device.CreateDrawingSurface(sizeTarget,
                                                            DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            using (var ds = CanvasComposition.CreateDrawingSession(surface))
            {
                ds.Clear(Color.FromArgb(255, 0, 0, 0));
                Rect destination = new Rect(0, 0, sizeTarget.Width, sizeTarget.Height);
                ds.DrawImage(blurEffect, destination, new Rect(0, 0, sizeSource.Width, sizeSource.Height));
                predominantColor.A = 100;
                ds.FillRectangle(destination, predominantColor);
            }

            return surface;
        }
        #endregion

        #region List Selection
        private void ImageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewItem item = (ListViewItem)ImageList.ContainerFromItem(e.ClickedItem);

            // If we near the edges of the list, scroll more into view
            GeneralTransform coordinate = item.TransformToVisual(ImageList);
            Point position = coordinate.TransformPoint(new Point(0, 0));

            if ((position.X + item.ActualWidth >= ImageList.ActualWidth) ||
                (position.X - item.ActualWidth <= 0))
            {
                double delta = position.X - item.ActualWidth <= 0 ? -item.ActualWidth : item.ActualWidth;
                delta *= 1.5;

                ScrollViewer scroller = ImageList.GetFirstDescendantOfType<ScrollViewer>();
                scroller.ChangeView(scroller.HorizontalOffset + delta, null, null);
            }
        }

        private void ImageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageList.SelectedItem != null)
            {
                ListViewItem item = (ListViewItem)ImageList.ContainerFromItem(ImageList.SelectedItem);
                Uri imageSource = _imageUriGetterFunc(item.Content, true);

                if (_backgroundCrossFadeBatch == null)
                {
                    // Save the previous image for a cross-fade
                    _previousSurfaceBrush.Surface = BackgroundImage.SurfaceBrush.Surface;
                    _previousSurfaceBrush.CenterPoint = BackgroundImage.SurfaceBrush.CenterPoint;
                    _previousSurfaceBrush.Stretch = BackgroundImage.SurfaceBrush.Stretch;

                    // Load the new background image
                    BackgroundImage.ImageOpened += BackgroundImage_ImageChanged;
                }

                // Update the images
                BackgroundImage.Source = imageSource;
                PrimaryImage.Source = imageSource;

                if (!_transition.Completed)
                {
                    _transition.Cancel();
                }

                // Kick off a continuity transition to animate from it's current position to it's new location
                CompositionImage image = VisualTreeHelperExtensions.GetFirstDescendantOfType<CompositionImage>(item);
                _transition.Initialize(this, image, null);
                _transition.Start(this, PrimaryImage, null, null);
            }
        }
        #endregion

        #region Dialog Functionality
        internal static void Show(Photo photo, object itemSource, Func<object, bool, Uri> photoGetter, Thickness margin, ContinuityTransition transition)
        {
            if (_viewerInstance != null)
            {
                throw new InvalidOperationException("Already displaying a photoviewer popup");
            }

            _hostGrid = Window.Current.Content.GetFirstDescendantOfType<Grid>();

            if (_hostGrid != null)
            {
                _viewerInstance = new ImagePopupViewer(photoGetter, transition, photo);

                // dialog needs to span all rows in the grid
                _viewerInstance.SetValue(Grid.RowSpanProperty, (_hostGrid.RowDefinitions.Count > 0 ? _hostGrid.RowDefinitions.Count : 1));
                _viewerInstance.SetValue(Grid.ColumnSpanProperty, (_hostGrid.ColumnDefinitions.Count > 0 ? _hostGrid.ColumnDefinitions.Count : 1));

                _hostGrid.Children.Add(_viewerInstance);

                _viewerInstance.ItemsSource = itemSource;

                // Create a full page desaturate effect to de-emphasize the background content
                DisplayInformation info = DisplayInformation.GetForCurrentView();
                Vector2 sizePageBounds = new Vector2((float)(Window.Current.Bounds.Width * info.RawPixelsPerViewPixel),
                                                     (float)(Window.Current.Bounds.Height * info.RawPixelsPerViewPixel));

                IGraphicsEffect graphicsEffect = new SaturationEffect()
                {
                    Saturation = 0.0f,
                    Source = new CompositionEffectSourceParameter("ImageSource")
                };

                Compositor compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
                CompositionEffectFactory effectFactory = compositor.CreateEffectFactory(graphicsEffect, null);
                CompositionEffectBrush brush = effectFactory.CreateBrush();
                brush.SetSourceParameter("ImageSource", compositor.CreateDestinationBrush());

                // Hook a new sprite under the host grid to completely cover the background content
                SpriteVisual desaturateVisual = compositor.CreateSpriteVisual();
                desaturateVisual.Size = sizePageBounds;
                desaturateVisual.Brush = brush;
                ElementCompositionPreview.SetElementChildVisual(_hostGrid.Children[0], desaturateVisual);

                // Fade the desaturation effect in
                ScalarKeyFrameAnimation fadeInAnimation = compositor.CreateScalarKeyFrameAnimation();
                fadeInAnimation.InsertKeyFrame(0, 0);
                fadeInAnimation.InsertKeyFrame(1, 1);
                fadeInAnimation.Duration = TimeSpan.FromMilliseconds(1000);
                desaturateVisual.StartAnimation("Opacity", fadeInAnimation);
            }
            else
            {
                throw new ArgumentException("can't find a top level grid");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CompositionScopedBatch batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += (batchSender, args) =>
            {
                ElementCompositionPreview.SetElementChildVisual(_hostGrid.Children[0], null);
                _hostGrid.Children.Remove(_viewerInstance);
                _viewerInstance = null;
            };

            // Closing the viewer, fade it out
            ScalarKeyFrameAnimation fadeOutAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeOutAnimation.InsertKeyFrame(0, 1);
            fadeOutAnimation.InsertKeyFrame(1, 0);
            fadeOutAnimation.Duration = TimeSpan.FromMilliseconds(800);
            ElementCompositionPreview.GetElementVisual(this).StartAnimation("Opacity", fadeOutAnimation);
            ElementCompositionPreview.GetElementChildVisual(_hostGrid.Children[0]).StartAnimation("Opacity", fadeOutAnimation);

            batch.End();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            #region PositionLights relative to grid size
            GridClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

            if (_lightProperties != null)
            {
                _lightProperties.InsertVector3("LightGlobalPosition", GetGlobalLightPosition());
            }
            #endregion

            Visual desaturateVisual = ElementCompositionPreview.GetElementChildVisual(_hostGrid.Children[0]);
            desaturateVisual.Size = GetPageBounds();

            StartDistanceAnimation();
        }

        private Vector3 GetGlobalLightPosition()
        {
            DisplayInformation info = DisplayInformation.GetForCurrentView();
            Vector2 sizePageBounds = new Vector2((float)(Window.Current.Bounds.Width * info.RawPixelsPerViewPixel),
                                                 (float)(Window.Current.Bounds.Height * info.RawPixelsPerViewPixel));

            var pageLeft = (float)CoreWindow.GetForCurrentThread().Bounds.Left * info.RawPixelsPerViewPixel;
            var pageTop = (float)CoreWindow.GetForCurrentThread().Bounds.Top * info.RawPixelsPerViewPixel;

            float lightDistance = sizePageBounds.X * .25f;
            var pos = new Vector3((float)pageLeft + (sizePageBounds.X * .5f), (float)pageTop + (sizePageBounds.Y * .85f), lightDistance);

            return pos;
        }

        private Vector2 GetPageBounds()
        {
            DisplayInformation info = DisplayInformation.GetForCurrentView();
            Vector2 sizePageBounds = new Vector2((float)(Window.Current.Bounds.Width * info.RawPixelsPerViewPixel),
                                                 (float)(Window.Current.Bounds.Height * info.RawPixelsPerViewPixel));
            return sizePageBounds;
        }

        #endregion

        #region Lighting Helpers
        private void BindLightPositions()
        {
            #region BindLightPositions
            _lightPositionAnimation = _compositor.CreateExpressionAnimation("propertySet.LightGlobalPosition + Vector3(0,0,propertySet.LightGlobalDistance)");
            _lightPositionAnimation.SetReferenceParameter("propertySet", _lightProperties);
            _lightTargetAnimation = _compositor.CreateExpressionAnimation("propertySet.LightGlobalPosition - Vector3(0,0,propertySet.LightGlobalDistance)");
            _lightTargetAnimation.SetReferenceParameter("propertySet", _lightProperties);
            #endregion
        }

        private void PositionImageLight(CompositionEffectBrush brush)
        {
            // Kick off the animations
            brush.StartAnimation("Light1.LightPosition", _lightPositionAnimation);
            brush.StartAnimation("Light1.LightTarget", _lightTargetAnimation);
        }
        #endregion

        #region List Helpers
        public Uri GetImageUriForListItem(object item)
        {
            return _imageUriGetterFunc(item, false);
        }
        #endregion

        private async void InitializeLighting()
        {
            // Create a lighting effect description

            var diffuseLightSource = new SpotDiffuseEffect()
            {
                Name = "Light1",
                DiffuseAmount = 1f,
                LimitingConeAngle = (float)Math.PI / 8f,
                LightColor = Colors.White,
                Source = new CompositionEffectSourceParameter("NormalMap"),
            };

            var combineImageWithLight = 
                    new ArithmeticCompositeEffect()
                    {
                        Source1Amount  = .6f,
                        Source2Amount  = .2f,
                        MultiplyAmount = .5f,

                        Source1 = new CompositionEffectSourceParameter("ImageSource"),
                        Source2 = diffuseLightSource
            };

            // Create the factory used to create brush for each sprite using lighting
            _lightEffectFactory = _compositor.CreateEffectFactory(combineImageWithLight,
                                new[] { "Light1.LightPosition", "Light1.LightTarget"});

            // Create the light position/target animations
            _lightProperties = _compositor.CreatePropertySet();
            _lightProperties.InsertVector3("LightGlobalPosition", GetGlobalLightPosition());
            _lightProperties.InsertScalar("LightGlobalDistance", 100f);

            // Create the shared normal map used for the lighting effect
            _normalMapBrush = _compositor.CreateSurfaceBrush();
            _normalMapBrush.Surface = await SurfaceLoader.LoadFromUri(new Uri("ms-appx:///Controls/VisualLayer/OneUpPhotoViewer/NormalMap.jpg"));
        }
        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            CompositionImage image = args.ItemContainer.ContentTemplateRoot.GetFirstDescendantOfType<CompositionImage>();
           
            // Set the URI source, and size to the large target image
            image.Source = GetImageUriForListItem(args.Item);

            // Setup the effect for each image
            SetLightingEffect(image);
        }

        private void SetLightingEffect(CompositionImage image)
        {
            // Create a brush from the lighting effect factory
            CompositionEffectBrush brush = _lightEffectFactory.CreateBrush();

            // Set the image sources
            brush.SetSourceParameter("ImageSource", image.SurfaceBrush);
            brush.SetSourceParameter("NormalMap", _normalMapBrush);

            // Update the image with the effect brush to use
            image.Brush = brush;

            PositionImageLight(brush);
        }

        private void StartDistanceAnimation()
        {
            // 3. animate distance of light from closer to farther and back

            var distanceAnimation = _compositor.CreateScalarKeyFrameAnimation();
            distanceAnimation.IterationCount = 1;
            distanceAnimation.InsertKeyFrame(0.0f, 80); // closer is smaller beam radius
            distanceAnimation.InsertKeyFrame(1.0f, 120); //further is bigger beam radius
            distanceAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            distanceAnimation.Direction = AnimationDirection.Alternate;
            distanceAnimation.Duration = TimeSpan.FromSeconds(2);
            _lightProperties.StartAnimation("LightGlobalDistance", distanceAnimation);

        }

      
    }
}
