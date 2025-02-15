using Prism.Avalonia.Toolkit;

namespace MvvmApp.ViewModels;

public partial class MainWindowViewModel //: ViewModelBase
{
  [NotifyField]
  private string _firstName;

  [NotifyField]
  private string _lastName;

  private string _title = string.Empty;

  /// <summary>Gets or sets the title of the view.</summary>
  public string Title { get => _title; set => SetProperty(ref _title, value); }

  public MainWindowViewModel()
  {
    Title = "Welcome to Prism.Avalonia!";
  }

  // Can't find this attribute due to library's access
  ////[Notifiable]
  ////public string SomeProperty { get; set; }

#pragma warning disable CA1822 // Mark members as static
  public string Greeting => "Hello from, Prism.Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}
