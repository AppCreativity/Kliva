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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace CompositionSampleGallery
{
    public sealed partial class ImagePopupViewer : UserControl
    {
        Compositor                  _compositor;
        //CompositionEffectFactory    _lightEffectFactory;
        //CompositionDrawingSurface   _normalMap;
        //Vector3KeyFrameAnimation    _lightPositionAnimation;
        //Vector3KeyFrameAnimation    _lightTargetAnimation;
        Vector3KeyFrameAnimation    _backgroundOffsetAnimation;
        ContinuityTransition        _transition;
        static ImagePopupViewer _viewerInstance;
        static Grid _hostGrid;
        Func<object, Uri> _imageUriGetterFunc;

        /// <summary>
        /// Private constructor as Show() is responsible for creating an instance
        /// </summary>
        private ImagePopupViewer(Func<object, Uri> photoGetter)
        {
            this.InitializeComponent();

            _imageUriGetterFunc = photoGetter;

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _transition = new ContinuityTransition();

            // Initialize the blur
            InitializeBlurPanel();

            // Initialize the lighting effects
            //InitializeLighting();
        }

        public object ItemsSource
        {
            get { return ImageList.ItemsSource; }
            set { ImageList.ItemsSource = value; }
        }

        public double ImageWidth
        {
            get { return PrimaryImage.Width; }
            set { PrimaryImage.Width = value; }
        }

        public double ImageHeight
        {
            get { return PrimaryImage.Height; }
            set { PrimaryImage.Height = value; }
        }

        private async void InitializeLighting()
        {
            // Create a lighting effect with diffuse + specular components
            IGraphicsEffect graphicsEffect = new CompositeEffect()
            {
                Mode = CanvasComposite.Add,
                Sources =
                {
                    new ArithmeticCompositeEffect()
                    {
                        Source1Amount  = .4f,
                        Source2Amount  = .2f,
                        MultiplyAmount = 1,

                        Source1 = new CompositionEffectSourceParameter("ImageSource"),
                        Source2 = new SpotDiffuseEffect()
                        {
                            Name = "Light1",
                            DiffuseAmount = 1f,
                            LimitingConeAngle = (float)Math.PI / 8f,
                            LightTarget = new Vector3(1000, 1000, 100),
                            LightPosition = new Vector3(0f, 0f, 100),
                            LightColor = Colors.White,
                            Source = new CompositionEffectSourceParameter("NormalMap"),
                        },
                    }//,
                    //new SpotSpecularEffect()
                    //{
                    //    Name = "Light2",
                    //    SpecularAmount = 1f,
                    //    SpecularExponent = 10000f,
                    //    LimitingConeAngle = (float)Math.PI / 8f,
                    //    LightColor = Colors.White,
                    //    Source = new CompositionEffectSourceParameter("NormalMap"),
                    //}
                }
            };

            // Create the factory used to create brush for each sprite using lighting
            //_lightEffectFactory = _compositor.CreateEffectFactory(graphicsEffect,
            //                    new[] { "Light1.LightPosition", "Light1.LightTarget" }); //,
            //                            //"Light2.LightPosition", "Light2.LightTarget"});

            //// Bug - lights are currently in screen space which is not intended
            //DisplayInformation info = DisplayInformation.GetForCurrentView();
            //Vector2 sizeLightBounds = new Vector2((float)(Window.Current.Bounds.Width * info.RawPixelsPerViewPixel),
            //                                      (float)(Window.Current.Bounds.Height * info.RawPixelsPerViewPixel));

            //// Create the light position/target animations
            //const float lightDistance = 1200;
            //Vector3 centerPosition = new Vector3(sizeLightBounds.X * .5f, sizeLightBounds.Y * .8f, lightDistance);
            //Vector3 rightPosition = new Vector3(sizeLightBounds.X * .55f, sizeLightBounds.Y * .9f, lightDistance);
            //Vector3 leftPosition = new Vector3(sizeLightBounds.X * .45f, sizeLightBounds.Y * .9f, lightDistance);

            //_lightPositionAnimation = _compositor.CreateVector3KeyFrameAnimation();
            //_lightPositionAnimation.InsertKeyFrame(0f, centerPosition);
            //_lightPositionAnimation.InsertKeyFrame(.33f, rightPosition);
            //_lightPositionAnimation.InsertKeyFrame(.66f, leftPosition);
            //_lightPositionAnimation.InsertKeyFrame(1f, centerPosition);
            //_lightPositionAnimation.Duration = TimeSpan.FromMilliseconds(20000);
            //_lightPositionAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            //centerPosition.Z -= lightDistance;
            //rightPosition.Z  -= lightDistance;
            //leftPosition.Z   -= lightDistance;

            //_lightTargetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            //_lightTargetAnimation.InsertKeyFrame(0f, centerPosition);
            //_lightTargetAnimation.InsertKeyFrame(.33f, rightPosition);
            //_lightTargetAnimation.InsertKeyFrame(.66f, leftPosition);
            //_lightTargetAnimation.InsertKeyFrame(1f, centerPosition);
            //_lightTargetAnimation.Duration = TimeSpan.FromMilliseconds(20000);
            //_lightTargetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            //// Create the shared normal map used for the lighting effect
            //_normalMap = await SurfaceLoader.LoadFromUri(new Uri("ms-appx:///Samples/SDK Insider/PhotoViewer/FlatNormals.jpg"));
        }

        private void InitializeBlurPanel()
        {
            //
            // Create the tinted blur effect for use by the popup panel
            //

            IGraphicsEffect graphicsEffect = new ArithmeticCompositeEffect()
            {
                Source1Amount = .5f,
                Source2Amount = .5f,
                MultiplyAmount = 0,
                Source1 = new GaussianBlurEffect()
                {
                    Source = new CompositionEffectSourceParameter("DestinationSource"),
                    BlurAmount = 40.0f,
                    Name = "Blur",
                    BorderMode = EffectBorderMode.Hard,
                    Optimization = EffectOptimization.Balanced
                },
                Source2 = new ColorSourceEffect()
                {
                    Name = "Tint",
                    Color = Color.FromArgb(255, 255, 255, 255)
                },
            };

            CompositionEffectFactory effectFactory = _compositor.CreateEffectFactory(graphicsEffect, new[] { "Tint.Color" });
            CompositionEffectBrush brush = effectFactory.CreateBrush();
            brush.SetSourceParameter("DestinationSource", _compositor.CreateDestinationBrush());

            // Create the blur visual with the blur brush and attach it to the tree
            SpriteVisual blurVisual = _compositor.CreateSpriteVisual();
            blurVisual.Size = new Vector2((float)BlurPanel.ActualWidth, (float)BlurPanel.ActualHeight);
            blurVisual.Brush = brush;
            ElementCompositionPreview.SetElementChildVisual(BlurPanel, blurVisual);

            // Use a load time effect handler to sample the predominant color from the image
            PrimaryImage.LoadTimeEffectHandler = SampleImageColor;
        }

     

        private void ExtractPredominantColor(Color[] colors, Size size)
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
            Color final = Color.FromArgb((byte)(maxColor >> 24), (byte)(maxColor >> 16),
                                         (byte)(maxColor >> 8),  (byte)(maxColor >> 0));

            // Animate the blur's tint color to the new value
            SpriteVisual blurVisual = (SpriteVisual)ElementCompositionPreview.GetElementChildVisual(BlurPanel);
            if (blurVisual != null && blurVisual.Brush != null)
            {
                ColorKeyFrameAnimation colorAnimation = _compositor.CreateColorKeyFrameAnimation();
                colorAnimation.InsertKeyFrame(1f, final);
                colorAnimation.InterpolationColorSpace = CompositionColorSpace.Rgb;
                colorAnimation.Duration = TimeSpan.FromMilliseconds(600);
                blurVisual.Brush.StartAnimation("Tint.Color", colorAnimation);
            }
        }

        private CompositionDrawingSurface SampleImageColor(CanvasBitmap bitmap, CompositionGraphicsDevice device, Size sizeTarget)
        {
            // Extract the color to tint the blur with
            ExtractPredominantColor(bitmap.GetPixelColors(), bitmap.Size);

            // Load the bitmap normally
            Size sizeSource = bitmap.Size;
            if (sizeTarget.IsEmpty)
            {
                sizeTarget = sizeSource;
            }

            CompositionDrawingSurface surface = device.CreateDrawingSurface(sizeTarget,
                                                            DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            using (var ds = CanvasComposition.CreateDrawingSession(surface))
            {
                ds.Clear(Color.FromArgb(0, 0, 0, 0));
                ds.DrawImage(bitmap, new Rect(0, 0, sizeTarget.Width, sizeTarget.Height), new Rect(0, 0, sizeSource.Width, sizeSource.Height));
            }

            return surface;
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            CompositionImage image = args.ItemContainer.ContentTemplateRoot.GetFirstDescendantOfType<CompositionImage>();
            Uri imageSource = _imageUriGetterFunc(args.Item);

            // Set the URI source, and size to the large target image
            image.Source = imageSource;
            
            // Setup the effect for each image
            SetLightingEffect(image);

            // If the AlbumImage is not yet set, update it with the first item
            if (PrimaryImage.Source == null && PrimaryImage.Brush == null)
            {
                // Set the background image with the same effect as the tiles
                SetLightingEffect(BackgroundImage);

                UpdateAlbumImage(imageSource, image.SurfaceBrush);
            }
        }

        private void SetLightingEffect(CompositionImage image)
        {
            // Create a brush from the lighting effect factory
            //CompositionEffectBrush brush = _lightEffectFactory.CreateBrush();

            //// Set the image sources
            //brush.SetSourceParameter("ImageSource", image.SurfaceBrush);
            //brush.SetSourceParameter("NormalMap", _compositor.CreateSurfaceBrush(_normalMap));

            //// Update the image with the effect brush to use
            //image.Brush = brush;

            // Kick off the animations
            //brush.StartAnimation("Light1.LightPosition", _lightPositionAnimation);
            //brush.StartAnimation("Light1.LightTarget", _lightTargetAnimation);
            //brush.StartAnimation("Light2.LightPosition", _lightPositionAnimation);
            //brush.StartAnimation("Light2.LightTarget", _lightTargetAnimation);
        }


        private void ImageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewItem item = (ListViewItem)ImageList.ContainerFromItem(e.ClickedItem);
            CompositionImage image = VisualTreeHelperExtensions.GetFirstDescendantOfType<CompositionImage>(item);
            Uri imageSource = _imageUriGetterFunc(item.Content);

            // Update the images with the new selection
            UpdateAlbumImage(imageSource, image.SurfaceBrush);

            if (!_transition.Completed)
            {
                _transition.Cancel();
            }

            // Kick off a continuity transition to animate from it's current position to it's new location
            _transition.Initialize(this, image, null);
            _transition.Start(this, PrimaryImage, null, null);
        }

        private void UpdateAlbumImage(Uri uri, CompositionBrush brush)
        {
            Size sizeBitmap = new Size(PrimaryImage.Width, PrimaryImage.Height);
            Uri uriSized = uri;
            PrimaryImage.Source = uriSized;

            if (brush != null)
            {
                // If we have a brush ready to go, just update the effect to use it
                CompositionEffectBrush effectBrush = BackgroundImage.Brush as CompositionEffectBrush;

                if(brush != null && effectBrush!=null)
                {
                    effectBrush.SetSourceParameter("ImageSource", brush);
                }
            }
            else
            {
                // No brush ready yet, update the source which will update the surface when available
                BackgroundImage.Source = uriSized;
            }
        }

        private void BlurPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Visual blurVisual = ElementCompositionPreview.GetElementChildVisual(BlurPanel);

            Vector2 newSize = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            if (blurVisual != null)
            {
                blurVisual.Size = newSize;
            }

            // Update the center point since the size has changed
            BackgroundImage.SpriteVisual.CenterPoint = new Vector3(newSize.X, newSize.Y, 0) * .5f;


            // Start a slow animation of the background image which is under the blur
            float max = Math.Max(newSize.X, newSize.Y);
            Vector2 sizeBackground = new Vector2(max, max);

            // Apply a scale from the center
            SpriteVisual backgroundSprite = BackgroundImage.SpriteVisual;
            backgroundSprite.Scale = new Vector3(2, 2, 0);
            backgroundSprite.CenterPoint = new Vector3(sizeBackground.X, sizeBackground.Y, 0) * .5f;

            // Linearly animate the offset side to side, slowly
            LinearEasingFunction linear = _compositor.CreateLinearEasingFunction();
            _backgroundOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            _backgroundOffsetAnimation.InsertKeyFrame(0, new Vector3(0, 0, 0), linear);
            _backgroundOffsetAnimation.InsertKeyFrame(.25f, new Vector3(-sizeBackground.X * .25f, 0, 0), linear);
            _backgroundOffsetAnimation.InsertKeyFrame(.75f, new Vector3(sizeBackground.X * .25f, 0, 0), linear);
            _backgroundOffsetAnimation.InsertKeyFrame(1, new Vector3(0, 0, 0), linear);
            _backgroundOffsetAnimation.Duration = TimeSpan.FromMilliseconds(80000);
            _backgroundOffsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            backgroundSprite.StartAnimation("Offset", _backgroundOffsetAnimation);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _hostGrid.Children.Remove(_viewerInstance);
            _viewerInstance = null;
            //TODO: IDisposible for viewer
        }

        internal static void Show(object itemSource, Func<object, Uri> photoGetter, Thickness margin)
        {
            if (_viewerInstance!=null)
            {
                throw new InvalidOperationException("Already displaying a photoviewer popup");
            }

            _hostGrid = Window.Current.Content.GetFirstDescendantOfType<Grid>();

            if (_hostGrid != null)
            {
                _viewerInstance = new ImagePopupViewer(photoGetter);

                _viewerInstance.Margin = margin;
                
                // dialog needs to span all rows in the grid
                _viewerInstance.SetValue(Grid.RowSpanProperty, (_hostGrid.RowDefinitions.Count>0?_hostGrid.RowDefinitions.Count:1));
                _viewerInstance.SetValue(Grid.ColumnSpanProperty, (_hostGrid.ColumnDefinitions.Count > 0 ? _hostGrid.ColumnDefinitions.Count : 1));

                _hostGrid.Children.Add(_viewerInstance);
                
              _viewerInstance.ItemsSource = itemSource;
            }
            else
            {
                throw new ArgumentException("can't find a top level grid");
            }
        }
    }
}
