using Avalonia.Controls;
using Lemon.Hosting.Modularization.Abstracts;

namespace Lemon.Toolkit.Views;

public partial class CompareView : UserControl,IView
{
    public CompareView()
    {
        InitializeComponent();
    }

    public void SetDataContext(IViewModel viewModel)
    {
        this.DataContext = viewModel;
    }
}