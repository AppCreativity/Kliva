using Kliva;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace App1
{
    class BlurPanelProto : Control
    {
        Compositor m_compositor;
        SpriteVisual m_blurVisual;
        SpriteVisual m_shadowVisual;
        CompositionEffectBrush m_blurBrush;
        ContainerVisual m_container;

        public CompositionPropertySet VisualProperties
        {
            get
            {
                return m_blurVisual.Properties;
            }
        }

        public Compositor Compositor
        {
            get
            {
                return m_compositor;
            }

            private set
            {
                m_compositor = value;
            }
        }

        public BlurPanelProto()
        {
            var myBackingVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            m_compositor = myBackingVisual.Compositor;
            m_blurBrush = BuildColoredBlurMixerBrush();
            m_blurBrush.SetSourceParameter("source", m_compositor.CreateDestinationBrush());

            m_blurVisual = m_compositor.CreateSpriteVisual();
            m_blurVisual.Brush = m_blurBrush;

            m_blurVisual.Properties.InsertScalar("BlurValue", 10.0f);
            m_blurVisual.Properties.InsertScalar("FadeValue", 1.0f);

            SetupPropertySetExpression();

            m_container = m_compositor.CreateContainerVisual();
            m_container.Children.InsertAtTop(m_blurVisual);

            CreateDropshadow();

            ElementCompositionPreview.SetElementChildVisual(this as UIElement, m_container);

            this.Loading += OnLoading;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            this.SizeChanged += OnSizeChanged;
            OnSizeChanged(this, null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= OnSizeChanged;
        }

        private void CreateDropshadow()
        {
            m_shadowVisual = m_compositor.CreateSpriteVisual();

            var theshadow = m_compositor.CreateDropShadow();
            theshadow.BlurRadius = 32.0f;
            m_shadowVisual.Shadow = theshadow;

            m_container.Children.InsertAtBottom(m_shadowVisual);
        }

        private void SetupPropertySetExpression()
        {
            var blurAnimator = m_compositor.CreateExpressionAnimation();
            blurAnimator.SetReferenceParameter("bluramount", m_blurVisual);
            blurAnimator.Expression = "bluramount.BlurValue";
            m_blurBrush.StartAnimation("Blur.BlurAmount", blurAnimator);

            var fadeInAnimator = m_compositor.CreateExpressionAnimation();
            fadeInAnimator.SetReferenceParameter("fadeInAmount", m_blurVisual);
            fadeInAnimator.Expression = "fadeInAmount.FadeValue";
            m_blurBrush.StartAnimation("mixer.Source1Amount", fadeInAnimator);

            var fadeOutAnimator = m_compositor.CreateExpressionAnimation();
            fadeOutAnimator.SetReferenceParameter("fadeOutAmount", m_blurVisual);
            fadeOutAnimator.Expression = "1-fadeOutAmount.FadeValue";
            m_blurBrush.StartAnimation("mixer.Source2Amount", fadeOutAnimator);
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (m_blurVisual != null)
            {
                var sharedSize = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
                m_blurVisual.Size = sharedSize;
            }

            if (m_shadowVisual != null)
            {
                m_shadowVisual.Size = new Vector2((float)this.ActualWidth, 4.0f);
                m_shadowVisual.Offset = new Vector3(0.0f, ((float)this.ActualHeight - 1), 0.0f);
            }
        }

        private CompositionEffectBrush BuildBlurBrush()
        {
            var gaussianBlur = new GaussianBlurEffect
            {
                Name = "Blur",
                Source = new CompositionEffectSourceParameter("source"),
                BlurAmount = 15.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Balanced
            };

            var factory = m_compositor.CreateEffectFactory(gaussianBlur);

            return factory.CreateBrush();
        }

        private CompositionEffectBrush BuildColoredBlurBrush()
        {
            var gaussianBlur = new GaussianBlurEffect
            {
                Name = "Blur",
                Source = new CompositionEffectSourceParameter("source"),
                BlurAmount = 15.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Balanced
            };

            var colorEffect = new ColorSourceEffect
            {
                Name = "ColorSource2",
                Color = (Color)App.Current.Resources["KlivaMainColor"]
            };

            var blendEffect = new BlendEffect
            {
                Mode = BlendEffectMode.Multiply,

                Background = gaussianBlur,
                Foreground = colorEffect
            };

            var factory = m_compositor.CreateEffectFactory(blendEffect);

            var brush = factory.CreateBrush();

            return brush;
        }

        private CompositionEffectBrush BuildColoredBlurMixerBrush()
        {
            var arithmeticComposit = new ArithmeticCompositeEffect
            {
                Name="Mixer",
                Source1Amount = 0.0f,
                Source2Amount = 0.0f,
                MultiplyAmount = 0,
                Source2 = new ColorSourceEffect
                {
                    Name = "ColorSource",
                    Color = (Color)App.Current.Resources["KlivaMainColor"]
                },
                Source1 = new BlendEffect
                {
                    Mode = BlendEffectMode.Multiply,

                    Foreground = new ColorSourceEffect
                    {
                        Name = "ColorSource2",
                        Color = (Color)App.Current.Resources["KlivaMainColor"]
                    },
                    Background = new GaussianBlurEffect 
                    {
                        Name = "Blur",
                        Source = new CompositionEffectSourceParameter("source"),
                        BlurAmount = 0.0f, //15
                        BorderMode = EffectBorderMode.Hard,
                        Optimization = EffectOptimization.Balanced
                    }
                }
            };

            var factory = m_compositor.CreateEffectFactory(arithmeticComposit, new string[] { "Blur.BlurAmount", "Mixer.Source1Amount", "Mixer.Source2Amount" });

            var brush = factory.CreateBrush();

            brush.Properties.InsertScalar("Blur.BlurAmount", 0.0f);
            brush.Properties.InsertScalar("Mixer.Source1Amount", 1.0f);
            brush.Properties.InsertScalar("Mixer.Source2Amount", 0.0f);

            return brush;
        }
    }
}
