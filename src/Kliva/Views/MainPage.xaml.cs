using System.Linq;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Extensions;
using Kliva.Controls;

namespace Kliva.Views
{
    public sealed partial class MainPage : Page
    {
        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public MainPage()
        {
            InitializeComponent();
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
            style.Setters.Add(new Setter(MinWidthProperty, ListColumn.ActualWidth));

            //TODO: Glenn - Can't we just grab : {ThemeResource SplitViewCompactPaneThemeLength} - we can but than it could be different from the actual size during runtime
            //TODO: Convert.ToInt32((Application.Current.Resources["SplitViewCompactPaneThemeLength"] as double?).Value)
            SplitView splitView = ((KlivaApplicationFrame) Parent).GetVisualDescendents<SplitView>().FirstOrDefault();
            if(splitView.DisplayMode != SplitViewDisplayMode.Inline)
                style.Setters.Add(new Setter(MarginProperty, new Thickness(-5.5,-5,0,0))); //don't know exactly where this 'magic' margin of 5px comes from

            menuFlyout.MenuFlyoutPresenterStyle = style;
        }
    }
}
