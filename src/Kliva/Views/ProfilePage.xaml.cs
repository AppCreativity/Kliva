using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Kliva.ViewModels;
using Windows.UI.Composition;
using ImplicitAnimations;
using System;

namespace Kliva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private ProfileViewModel ViewModel => DataContext as ProfileViewModel;

        public ProfilePage()
        {
            this.InitializeComponent();
        }

        private void OnFriendsContainerChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // get the Visual for the item container
            var visual = args.ItemContainer.GetVisual(); // this is the GridViewItem

            // get the instance of the compositor
            Compositor compositor = visual.Compositor;

            // create the animation we want to run implicitly when the property changes
            var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(450);

            // create the implicit map to the properties we care about
            var implicitMap = compositor.CreateImplicitAnimationMap();
            implicitMap.Add("Offset", offsetAnimation);

            // set the implicit map to the visual
            visual.ImplicitAnimations = implicitMap;
        }
    }
}
