using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace ImplicitAnimations
{
    public static class UIElementExtensionMethods
    {
        public static void EnableLayoutImplicitAnimations(this UIElement element)
        {
            Compositor compositor;
            var result = ElementCompositionPreview.GetElementVisual(element);
            compositor = result.Compositor;

            var elementImplicitAnimation = compositor.CreateImplicitAnimationMap();
            elementImplicitAnimation["Offset"] = createOffsetAnimation(compositor);
            elementImplicitAnimation["Opacity"] = createOpacityAnimation(compositor);
            result.ImplicitAnimations = elementImplicitAnimation;
        }

        private static KeyFrameAnimation createOffsetAnimation(Compositor compositor)
        {
            Vector3KeyFrameAnimation kf = compositor.CreateVector3KeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "FinalValue");
            kf.Duration = TimeSpan.FromSeconds(0.9);
            return kf;
        }

        private static KeyFrameAnimation createOpacityAnimation(Compositor compositor)
        {
            ScalarKeyFrameAnimation kf = compositor.CreateScalarKeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "FinalValue");
            kf.Duration = TimeSpan.FromSeconds(0.9);
            return kf;
        }
    }
}
