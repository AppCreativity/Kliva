using System.Linq;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Extensions;
using Kliva.Controls;
using Windows.UI;
using Kliva.Extensions;
using System.Numerics;

namespace Kliva.Views
{
    public sealed partial class MainPage : Page
    {
        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public MainPage()
        {
            InitializeComponent();
            InitializeFluentDesign();
        }

        private void InitializeFluentDesign()
        {
            var compositor = this.Visual().Compositor;

            ActivityFeedGrid.EnableImplicitAnimation(VisualPropertyType.Offset, 200);
            ActivityDetail.EnableImplicitAnimation(VisualPropertyType.Offset, 200);
            //RightGrid.EnableImplicitAnimation(VisualPropertyType.Offset, 200);

            // Enable implicit Visible/Collapsed animations.
            ActivityFeedGrid.EnableFluidVisibilityAnimation(showFromScale: 0.6f, hideToScale: 0.8f, showDuration: 300, hideDuration: 250);
            ActivityDetail.EnableFluidVisibilityAnimation(showFromScale: 0.6f, hideToScale: 0.8f, showDelay: 200, showDuration: 300, hideDuration: 250);
            //RightGrid.EnableFluidVisibilityAnimation(showFromScale: 0.6f, hideToScale: 0.8f, showDelay: 200, showDuration: 300, hideDuration: 250);
        }

        private void UpdateVisualState(VisualState currentState)
        {
            ViewModel.CurrentState = currentState;
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // We are only guaranteed to have the updated VisualState in Loaded
            UpdateVisualState(VisualStateGroup.CurrentState);
        }

        /// <summary>
        /// Set menu flyout width equal to the container it's in, in our case the ListColumn ( the first column of the grid )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Not sure why, but we can't do this in XAML?
        /// MenuFlyout.MenuFlyoutPresenterStyle
        ///     Style TargetType = "MenuFlyoutPresenter"
        ///         Setter Property="MinWidth" Value="{Binding ElementName=ListColumn, Path=ActualWidth}"
        ///     Style
        /// MenuFlyout.MenuFlyoutPresenterStyle
        /// </remarks>
        private void OnFilterFlyoutOpened(object sender, object e)
        {
            MenuFlyout menuFlyout = sender as MenuFlyout;
            Style style = new Style { TargetType = typeof(MenuFlyoutPresenter) };
            style.Setters.Add(new Setter(MinWidthProperty, LeftPanel.ActualWidth));

            //TODO: Glenn - Can't we just grab : {ThemeResource SplitViewCompactPaneThemeLength} - we can but than it could be different from the actual size during runtime
            //TODO: Convert.ToInt32((Application.Current.Resources["SplitViewCompactPaneThemeLength"] as double?).Value)
            SplitView splitView = ((KlivaApplicationFrame) Parent).GetVisualDescendents<SplitView>().FirstOrDefault();
            if(splitView.DisplayMode != SplitViewDisplayMode.Inline)
                style.Setters.Add(new Setter(MarginProperty, new Thickness(-5.5,-5,0,0))); //don't know exactly where this 'magic' margin of 5px comes from

            menuFlyout.MenuFlyoutPresenterStyle = style;
        }
    }
}
