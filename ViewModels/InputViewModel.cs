using System;
using ReactiveUI;
#nullable disable
namespace WhistleSharp.ViewModels;

public class InputViewModel : ViewModelBase
{
    public static event Action<string> OnUpdateInput;

    static string             _input = "";
    public string Input {
        get => _input;
        set {
            this.RaiseAndSetIfChanged(ref _input, value);
            OnPropertyChangedInvoke();
        }
    }

    public InputViewModel()
    {
        _input = "1q.5+q'12 3q55 1q.5+q'13 2q.8q'12 321781 252q7q''12q' 3q.2q'435w4q'321q 5+q'1235";
    }

    public void UpdateInput(string input) {
        Input = input;
        OnUpdateInput?.Invoke(input);
    }
}