using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ShareLingo.Core.ViewModel;

namespace ShareLingo.AvaloniaMvvmApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.DataContext = App.Current.Services.GetService<MainViewModel>();
        InitializeComponent();
    }
}