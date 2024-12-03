using ResourceManagerApp.ViewModels;

namespace ResourceManagerApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }

    private void OnCreateFileClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as MainViewModel;
        viewModel?.CreateFile("NewFile.txt");
    }

    private void OnDeleteFileClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as MainViewModel;
        viewModel?.DeleteFile("NewFile.txt");
    }
}
