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
        Compositor _compositor;
        SpriteVisual m_blurVisual;
        SpriteVisual m_shadowVisual;
        ContainerVisual m_container;

        public CompositionPropertySet Params
        {
            get
            {
                return m_blurVisual.Properties;
            }
        }

        public BlurPanelProto()
        {
            this.Loaded += BlurPanelProto_Loaded;
            this.Unloaded += BlurPanelProto_Unloaded;
            //this.Background = new Brush(Colors.from)
        }

        private void BlurPanelProto_Unloaded(object sender, RoutedEventArgs e)
        {
            //TODO: cleanup
        }

        private void BlurPanelProto_Loaded(object sender, RoutedEventArgs e)
        {
            var sharedSize = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            //            this.Background = new SolidColorBrush(Colors.Red);
            var myBackingVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            _compositor = myBackingVisual.Compositor;
            this.SizeChanged += BlurPanelProto_SizeChanged;

            var blurBrush = BuildColoredBlurBrush();
            blurBrush.SetSourceParameter("dest", _compositor.CreateDestinationBrush());

            m_blurVisual = _compositor.CreateSpriteVisual();
            //m_blurVisual.Brush = _compositor.CreateColorBrush(Color.FromArgb(128,255,0,0));
            m_blurVisual.Brush = blurBrush;

            m_blurVisual.Properties.InsertScalar("BlurValue", 10.0f);
            m_blurVisual.Properties.InsertScalar("FadeValue", 1.0f);

            var blurAnimator = _compositor.CreateExpressionAnimation();
            blurAnimator.SetReferenceParameter("bluramount", m_blurVisual);
            blurAnimator.Expression = "bluramount.BlurValue";
            blurBrush.StartAnimation("GB.BlurAmount", blurAnimator);

            var fadeInAnimator = _compositor.CreateExpressionAnimation();
            fadeInAnimator.SetReferenceParameter("fadeInAmount", m_blurVisual);
            fadeInAnimator.Expression = "fadeInAmount.FadeValue";
            blurBrush.StartAnimation("mixer.Source1Amount", fadeInAnimator);

            var fadeOutAnimator = _compositor.CreateExpressionAnimation();
            fadeOutAnimator.SetReferenceParameter("fadeOutAmount", m_blurVisual);
            fadeOutAnimator.Expression = "1-fadeOutAmount.FadeValue";
            blurBrush.StartAnimation("mixer.Source2Amount", fadeOutAnimator);

            m_blurVisual.Size = sharedSize;

            m_container = _compositor.CreateContainerVisual();
            m_container.Children.InsertAtTop(m_blurVisual);
            m_container.Size = sharedSize;

            m_shadowVisual = _compositor.CreateSpriteVisual();

            var theshadow = _compositor.CreateDropShadow();
            theshadow.BlurRadius = 32.0f;
            //theshadow.Color = Color.FromArgb(128, 0, 0, 0);
            //theshadow.Offset = new Vector3(0.0f, 5.0f, 0.0f);
            m_shadowVisual.Shadow = theshadow;

            m_shadowVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth + 100.0f, (float)4.0);
            m_shadowVisual.Offset = new Vector3(0.0f, ((float)this.ActualHeight - 1), 0.0f);
            m_container.Children.InsertAtBottom(m_shadowVisual);

            ElementCompositionPreview.SetElementChildVisual(this as UIElement, m_container);
        }


        private void BlurPanelProto_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (m_blurVisual != null)
            {
                var sharedSize = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
                m_blurVisual.Size = sharedSize;
                m_shadowVisual.Size = new Vector2((float)this.ActualWidth, 4.0f);
                m_shadowVisual.Offset = new Vector3(0.0f, ((float)this.ActualHeight - 1), 0.0f);
                m_container.Size = sharedSize;
                //TODO: other sizes
            }
        }

        private CompositionEffectBrush BuildBlurBrush()
        {
            GaussianBlurEffect se = new GaussianBlurEffect() { BlurAmount = 15.0f, Name = "Blur", BorderMode = EffectBorderMode.Hard, Optimization = EffectOptimization.Balanced };

            se.Source = new CompositionEffectSourceParameter("dest");

            var factory = _compositor.CreateEffectFactory(se);

            return factory.CreateBrush();
        }


        private CompositionEffectBrush BuildColoredBlurBrush()
        {
            //var blurEffect = new BlendEffect
            //{
            //    Mode = BlendEffectMode.Multiply,

            //    Foreground = new ColorSourceEffect
            //    {
            //        Name = "ColorSource",
            //        Color = (Color)App.Current.Resources["KlivaMainColor"]
            //    },
            //    Background = new GaussianBlurEffect /* newly supported by Composition */
            //    {
            //        Name = "GB",
            //        Source = new CompositionEffectSourceParameter("dest"),
            //        BlurAmount = 0.0f, //15
            //        BorderMode = EffectBorderMode.Hard,
            //        Optimization = EffectOptimization.Balanced
            //    }
            //};

            var blurEffect = new ArithmeticCompositeEffect
            {
                Name="mixer",
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
                    Background = new GaussianBlurEffect /* newly supported by Composition */
                    {
                        Name = "GB",
                        Source = new CompositionEffectSourceParameter("dest"),
                        BlurAmount = 0.0f, //15
                        BorderMode = EffectBorderMode.Hard,
                        Optimization = EffectOptimization.Balanced
                    }
                }
            };


            //GaussianBlurEffect se = new GaussianBlurEffect() { BlurAmount = 15.0f, Name = "Blur", BorderMode = EffectBorderMode.Hard, Optimization = EffectOptimization.Balanced };

            //se.Source = new CompositionEffectSourceParameter("dest");

            var factory = _compositor.CreateEffectFactory(blurEffect, new string[] { "GB.BlurAmount", "mixer.Source1Amount", "mixer.Source2Amount" });

            var brush = factory.CreateBrush();

            brush.Properties.InsertScalar("GB.BlurAmount", 0.0f);
            brush.Properties.InsertScalar("mixer.Source1Amount", 1.0f);
            brush.Properties.InsertScalar("mixer.Source2Amount", 0.0f);

            return brush;
        }
    }
}
