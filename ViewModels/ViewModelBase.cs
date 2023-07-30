using System;
using ReactiveUI;
#nullable disable
namespace WhistleSharp.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public static event Action OnPropertyChanged;
    protected void OnPropertyChangedInvoke() => OnPropertyChanged?.Invoke();
}