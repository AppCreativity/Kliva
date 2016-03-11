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

namespace SamplesCommon
{
    public class BackDrop : Control
    {
        Compositor m_compositor;
        SpriteVisual m_blurVisual;
        CompositionEffectBrush m_blurBrush;
        bool m_setUpExpressions;

        public BackDrop()
        {
            var myBackingVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            m_compositor = myBackingVisual.Compositor;
            this.SizeChanged += BackDrop_SizeChanged;

            m_blurBrush = BuildBlurBrush();
            m_blurBrush.SetSourceParameter("source", m_compositor.CreateDestinationBrush());

            m_blurVisual = m_compositor.CreateSpriteVisual();
            m_blurVisual.Brush = m_blurBrush;

            BlurAmount = 9;
            TintColor = Colors.Transparent;

            BackDrop_SizeChanged(this, null);

            ElementCompositionPreview.SetElementChildVisual(this as UIElement, m_blurVisual);

            this.Unloaded += BackDrop_Unloaded;
        }

        public const string BlurAmountProperty = "BlurAmount";
        public const string TintColorProperty = "TintColor";

        public double BlurAmount
        {
            get
            {
                float value = 0;
                m_blurVisual.Properties.TryGetScalar(BlurAmountProperty, out value);
                return value;
            }
            set
            {
                if (!m_setUpExpressions)
                {
                    m_blurBrush.Properties.InsertScalar("Blur.BlurAmount", (float)value);
                }
                m_blurVisual.Properties.InsertScalar(BlurAmountProperty, (float)value);
            }
        }

        public Color TintColor
        {
            get
            {
                Color value;
                m_blurVisual.Properties.TryGetColor("TintColor", out value);
                return value;
            }
            set
            {
                if (!m_setUpExpressions)
                {
                    m_blurBrush.Properties.InsertColor("Color.Color", value);
                }
                m_blurVisual.Properties.InsertColor(TintColorProperty, value);
            }
        }

        public CompositionPropertySet Properties
        {
            get
            {
                if (!m_setUpExpressions)
                {
                    SetUpPropertySetExpressions();
                }
                return m_blurVisual.Properties;
            }
        }

        private void BackDrop_Unloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= BackDrop_SizeChanged;
            m_blurVisual = null;
        }


        private void BackDrop_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (m_blurVisual != null)
            {
                m_blurVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }

        private void SetUpPropertySetExpressions()
        {
            m_setUpExpressions = true;

            var exprAnim = m_compositor.CreateExpressionAnimation();
            exprAnim.Expression = $"sourceProperties.{BlurAmountProperty}";
            exprAnim.SetReferenceParameter("sourceProperties", m_blurVisual.Properties);

            m_blurBrush.Properties.StartAnimation("Blur.BlurAmount", exprAnim);

            exprAnim.Expression = $"sourceProperties.{TintColorProperty}";

            m_blurBrush.Properties.StartAnimation("Color.Color", exprAnim);
        }


        private CompositionEffectBrush BuildBlurBrush()
        {
            GaussianBlurEffect blurEffect = new GaussianBlurEffect() {
                Name = "Blur",
                BlurAmount = 0.0f,
                BorderMode = EffectBorderMode.Hard, Optimization = EffectOptimization.Balanced };

            blurEffect.Source = new CompositionEffectSourceParameter("source");

            BlendEffect effect = new BlendEffect
            {
                Foreground = new ColorSourceEffect { Name = "Color", Color = Colors.Transparent },
                Background = blurEffect,
                Mode = BlendEffectMode.Multiply
            };

            var factory = m_compositor.CreateEffectFactory(
                effect,
                new [] { "Blur.BlurAmount", "Color.Color" }
                );

            return factory.CreateBrush();
        }
    }
}
