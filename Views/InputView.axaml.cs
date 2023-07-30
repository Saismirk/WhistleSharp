using Avalonia.Controls;
using WhistleSharp.ViewModels;

namespace WhistleSharp.Views;

public partial class InputView : UserControl {
    public InputView()
    {
        InitializeComponent();
        InputViewModel.OnUpdateInput += UpdateInput;
    }

    void UpdateInput(string input) => InputBox.Text = input;
}