using Microsoft.Maui.Controls;
using Microsoft.AspNetCore.Components.WebView.Maui;
using PersonalFinanceTrackerMB.Components;

namespace PersonalFinanceTrackerMB;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Programmatically add Routes.razor as the root component
        blazorWebView.RootComponents.Add(new RootComponent
        {
            Selector = "#app",
            ComponentType = typeof(Routes)
        });
    }
}
