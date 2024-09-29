using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Lemon.Toolkit.Framework;
using Lemon.Toolkit.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace Lemon.Toolkit.Views;

[RequiresUnreferencedCode("")]
public partial class SingleFileView : UserControl, IView
{
    public SingleFileView()
    {
        InitializeComponent();
    }
    public void SetDataContext(IViewModel viewModel)
    {
        this.DataContext = viewModel;
    }
}