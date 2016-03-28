﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************

using Microsoft.Graphics.Canvas;
using SamplesCommon;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace Kliva.Controls.VisualLayer
{
    public sealed class CompositionImage : Control
    {
        private bool                        _unloaded;
        private Compositor                  _compositor;
        private SpriteVisual                _sprite;
        private Uri                         _uri;
        private CompositionDrawingSurface   _surface;
        private CompositionSurfaceBrush     _surfaceBrush;
        private CompositionBrush            _brush;
        private LoadTimeEffectHandler       _loadEffectDelegate;
        private CompositionStretch          _stretchMode;
        private DispatcherTimer             _timer;
        public event RoutedEventHandler     ImageOpened;
        public event RoutedEventHandler     ImageFailed;
        private TimeSpan                    _placeholderDelay;
        private bool                        _sharedSurface;

        static private CompositionBrush     _loadingBrush;
        static private ScalarKeyFrameAnimation
                                            _fadeOutAnimation;
        static private Vector2KeyFrameAnimation
                                            _scaleAnimation;
        static bool                         _staticsInitialized;

        public CompositionImage()
        {
            this.DefaultStyleKey = typeof(CompositionImage);
            this.Background = new SolidColorBrush(Colors.Transparent);
            this._stretchMode = CompositionStretch.Uniform;
            this.Unloaded += CompImage_Unloaded;
            this.SizeChanged += CompImage_SizeChanged;

            _placeholderDelay = TimeSpan.FromMilliseconds(50);

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _sprite = _compositor.CreateSpriteVisual();
            _sprite.Size = new Vector2((float)ActualWidth, (float)ActualHeight);
            _surfaceBrush = _compositor.CreateSurfaceBrush(null);

            // Intialize the statics as needed
            if (!_staticsInitialized)
            {
                _loadingBrush = _compositor.CreateColorBrush(Colors.DarkGray);

                TimeSpan duration = TimeSpan.FromMilliseconds(1000);
                _fadeOutAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _fadeOutAnimation.InsertKeyFrame(0, 1);
                _fadeOutAnimation.InsertKeyFrame(1, 0);
                _fadeOutAnimation.Duration = duration;

                _scaleAnimation = _compositor.CreateVector2KeyFrameAnimation();
                _scaleAnimation.InsertKeyFrame(0, new Vector2(1.25f, 1.25f));
                _scaleAnimation.InsertKeyFrame(1, new Vector2(1, 1));
                _scaleAnimation.Duration = duration;

                _staticsInitialized = true;
            }

            // Initialize the surface loader if needed
            if (!SurfaceLoader.IsInitialized)
            {
                SurfaceLoader.Initialize(ElementCompositionPreview.GetElementVisual(this).Compositor);
            }

            // If the surface is not yet loaded, do so now
            if (!IsContentLoaded)
            {
                LoadSurface();
            }

            // If we have content, update the brush
            if (IsContentLoaded)
            {
                UpdateBrush();
            }

            ElementCompositionPreview.SetElementChildVisual(this, _sprite);

            _unloaded = false;

        }

        private void ReleaseSurface()
        {
            if (_surface != null)
            {
                // If no one has asked to share, dispose it to free the memory
                if (!_sharedSurface)
                {
                    _surface.Dispose();
                }
                _surface = null;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size(0, 0);

            // We override measure to implement similar semantics to the normal XAML Image UIElement
            if (_surface != null)
            {
                Size scaling = new Size(1, 1);
                Size imageSize = _surface.Size;

                // If we're not stretching or have infinite space, request the full surface size
                if (!(Double.IsInfinity(availableSize.Width) && Double.IsInfinity(availableSize.Height)) &&
                    _stretchMode != CompositionStretch.None)
                {
                    // Calculate the amount of horizontal and vertical scaling to fit into available space
                    scaling = new Size(availableSize.Width / imageSize.Width, availableSize.Height / imageSize.Height);


                    //
                    // If we've got infinite space in either dimension, scale by the same amount as the constrained
                    // dimension.
                    //

                    if (Double.IsInfinity(availableSize.Width))
                    {
                        scaling.Width = scaling.Height;
                    }
                    else if (Double.IsInfinity(availableSize.Height))
                    {
                        scaling.Height = scaling.Width;
                    }
                    else
                    {
                        //
                        // We're fitting into a space confined by both width and height, do appropriate scaling
                        // based on the stretch mode.
                        //

                        switch(_stretchMode)
                        {
                            case CompositionStretch.Uniform:
                                scaling.Width = scaling.Height = Math.Min(scaling.Width, scaling.Height);
                                break;
                            case CompositionStretch.UniformToFill:
                                scaling.Width = scaling.Height = Math.Max(scaling.Width, scaling.Height);
                                break;
                            case CompositionStretch.Fill:
                            default:
                                break;
                        }
                    }
                }

                // Apply the scale to get the final desired size
                desiredSize.Width = imageSize.Width * scaling.Width;
                desiredSize.Height = imageSize.Height * scaling.Height;
            }
            else
            {
                // We don't have any content, so default to zero unless a specific size was requested
                if (!Double.IsNaN(Width))
                {
                    desiredSize.Width = Width;
                }
                if (!Double.IsNaN(Height))
                {
                    desiredSize.Height = Height;
                }
            }

            return desiredSize;
        }

        private void CompImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_sprite != null)
            {
                // Calculate the new size
                Vector2 size = new Vector2((float)ActualWidth, (float)ActualHeight);

                // Update the sprite
                _sprite.Size = size;

                // Update the loading sprite if set
                Visual loadingSprite = _sprite.Children.FirstOrDefault();
                if (loadingSprite != null)
                {
                    loadingSprite.Size = size;
                }
            }
        }
        
        private void CompImage_Unloaded(object sender, RoutedEventArgs e)
        {
            _unloaded = true;

            ReleaseSurface();

            if (_sprite != null)
            {
                _sprite.Dispose();
                _sprite = null;
            }
        }

        private void UpdateBrush()
        {
            // Create the surface brush if needed
            if (_surfaceBrush == null)
            {
                _surfaceBrush = _compositor.CreateSurfaceBrush(_surface);
            }
            else
            {
                // Already created, just update the surface
                _surfaceBrush.Surface = _surface;
            }
            _surfaceBrush.Stretch = _stretchMode;

            // If the active brush is not set, use the surface brush
            if (_brush == null)
            {
                _brush = _surfaceBrush;
            }
            else if (_brush is CompositionEffectBrush)
            {
                //
                // If there is an EffectBrush set, it must supply ImageSource reference parameter for setitng
                // the Image content.
                //

                ((CompositionEffectBrush)_brush).SetSourceParameter("ImageSource", _surfaceBrush);
            }

            // Update the sprite to use the brush
            _sprite.Brush = _brush;
        }

        public Stretch Stretch
        {
            get
            {
                Stretch stretch;

                switch (_stretchMode)
                {
                    case CompositionStretch.Fill:
                        stretch = Stretch.Fill;
                        break;
                    case CompositionStretch.Uniform:
                        stretch = Stretch.Uniform;
                        break;
                    case CompositionStretch.UniformToFill:
                        stretch = Stretch.UniformToFill;
                        break;
                    default:
                        stretch = Stretch.None;
                        break;
                }

                return stretch;
            }

            set
            {
                CompositionStretch stretch;
                switch (value)
                {
                    case Stretch.Fill:
                        stretch = CompositionStretch.Fill;
                        break;
                    case Stretch.Uniform:
                        stretch = CompositionStretch.Uniform;
                        break;
                    case Stretch.UniformToFill:
                        stretch = CompositionStretch.UniformToFill;
                        break;
                    default:
                        stretch = CompositionStretch.None;
                        break;
                }

                if (stretch != _stretchMode)
                {
                    _stretchMode = stretch;

                    if (_surfaceBrush != null)
                    {
                        _surfaceBrush.Stretch = stretch;
                    }
                }
            }
        }

        public Uri Source
        {
            get { return _uri; }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    LoadSurface();
                }
            }
        }

        public bool IsContentLoaded
        {
            get { return _surface != null; }
        }

        public bool SharedSurface
        {
            get { return _sharedSurface; }
            set { _sharedSurface = value; }
        }

        public LoadTimeEffectHandler LoadTimeEffectHandler
        {
            get { return _loadEffectDelegate; }
            set
            {
                _loadEffectDelegate = value;
            }
        }

        private async void LoadSurface()
        {
            // If we're clearing out the content, return
            if (_uri == null)
            {
                ReleaseSurface();
                return;
            }

            try
            {
                // Start a timer to enable the placeholder image if requested
                if (_surface == null && _placeholderDelay >= TimeSpan.Zero)
                {
                    _timer = new DispatcherTimer();
                    _timer.Interval = _placeholderDelay;
                    _timer.Tick += Timer_Tick;
                    _timer.Start();
                }

                // Load the image asynchronously
                CompositionDrawingSurface surface = await SurfaceLoader.LoadFromUri(_uri, Size.Empty, _loadEffectDelegate);

                if (_surface != null)
                {
                    ReleaseSurface();
                }

                _surface = surface;

                // The surface has changed, so we need to re-measure with the new surface dimensions
                InvalidateMeasure();

                // Async operations may take a while.  If we've unloaded, return now.
                if (_unloaded)
                {
                    ReleaseSurface();
                    return;
                }

                // Update the brush
                UpdateBrush();

                // Success, fire the Opened event
                if (ImageOpened != null)
                {
                    ImageOpened(this, null);
                }

                //
                // If we created the loading placeholder, now that the image has loaded 
                // cross-fade it out.
                //

                if (_sprite.Children.Count > 0)
                {
                    Debug.Assert(_timer == null);
                    StartCrossFade();
                }
                else if (_timer != null)
                {
                    // We didn't end up loading the placeholder, so just stop the timer
                    _timer.Stop();
                    _timer = null;
                }
            }
            catch(FileNotFoundException)
            {
                if (ImageFailed != null)
                {
                    ImageFailed(this, null);
                }
            }           
        }

        private void Timer_Tick(object sender, object e)
        {
            if (_timer != null)
            {
                //Debug.Assert(_sprite.Children.Count == 0, "Should not be any children");

                // Create a second sprite to show while the image is still loading
                SpriteVisual loadingSprite = _compositor.CreateSpriteVisual();
                loadingSprite = _compositor.CreateSpriteVisual();
                loadingSprite.Size = new Vector2((float)ActualWidth, (float)ActualHeight);
                loadingSprite.Brush = _loadingBrush;
                _sprite.Children.InsertAtTop(loadingSprite);

                // Stop and null out the time, no more need for it.
                _timer.Stop();
                _timer = null;
            }
        }

        private void StartCrossFade()
        {
            Debug.Assert(_sprite.Children.Count > 0, "Unexpected number of children");

            // Start a batch so we can cleanup the loading sprite
            CompositionScopedBatch batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += EndCrossFade;

            // Animate the opacity of the loading sprite to fade it out and the texture scale just for effect
            Visual loadingVisual = _sprite.Children.LastOrDefault();
            loadingVisual.StartAnimation("Opacity", _fadeOutAnimation);

#if SDKVERSION_INSIDER
            _surfaceBrush.StartAnimation("Scale", _scaleAnimation);
            _surfaceBrush.CenterPoint = new Vector2((float)_surface.Size.Width *.5f, (float)_surface.Size.Height * .5f);
#endif
            // End the batch after those animations complete
            batch.End();
        }

        private void EndCrossFade(object sender, CompositionBatchCompletedEventArgs args)
        {
            // If the sprite is still valid, remove the loading sprite from the children collection
            if (_sprite != null && _sprite.Children.Count > 0)
            {
                _sprite.Children.RemoveAll();
            }
        }

        public CompositionBrush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                UpdateBrush();
            }
        }

        public CompositionSurfaceBrush SurfaceBrush
        {
            get { return _surfaceBrush; }
        }

        public SpriteVisual SpriteVisual
        {
            get { return _sprite; }
        }

        public TimeSpan PlaceholderDelay
        {
            get { return _placeholderDelay; }
            set { _placeholderDelay = value; }
        }
    }
}
