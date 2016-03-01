using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
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
//            this.Background = new SolidColorBrush(Colors.Red);
            var myBackingVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            _compositor = myBackingVisual.Compositor;
            this.SizeChanged += BlurPanelProto_SizeChanged;
            
            var brush = BuildBlurBrush();
            brush.SetSourceParameter("dest", _compositor.CreateDestinationBrush());

            m_blurVisual = _compositor.CreateSpriteVisual();
            //m_blurVisual.Brush = _compositor.CreateColorBrush(Color.FromArgb(128,255,0,0));
            m_blurVisual.Brush = brush;

            m_blurVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(this as UIElement, m_blurVisual);
        }


        private void BlurPanelProto_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (m_blurVisual != null)
            {
                m_blurVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }

        private CompositionEffectBrush BuildBlurBrush()
        {
            GaussianBlurEffect se = new GaussianBlurEffect() { BlurAmount = 25.0f, Name = "Blur", BorderMode = EffectBorderMode.Hard, Optimization = EffectOptimization.Balanced };

            se.Source = new CompositionEffectSourceParameter("dest");

            var factory = _compositor.CreateEffectFactory(se);

            return factory.CreateBrush();
        }
    }
}
