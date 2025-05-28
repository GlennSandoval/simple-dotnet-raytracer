using Avalonia.Controls;
using raytracer.ViewModels;

namespace raytracer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            if (DataContext is MainWindowViewModel context)
            {
                context.Refresh += () =>
            {
                MainImage.InvalidateVisual();
            };
            }
        };
    }
}