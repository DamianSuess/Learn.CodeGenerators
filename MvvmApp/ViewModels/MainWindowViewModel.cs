using MvvmApp.Generators;
using Prism.Avalonia.Toolkit;

namespace MvvmApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
  [NotifyField]
  private string _firstName;

  [NotifyField]
  private string _lastName;

  public MainWindowViewModel()
  {
    Title = "Welcome to Prism.Avalonia!";
  }

#pragma warning disable CA1822 // Mark members as static
  public string Greeting => "Hello from, Prism.Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}
