using Prism.Avalonia.Toolkit;
using Prism.Commands;

namespace MvvmApp.ViewModels;

public partial class MainWindowViewModel //: ViewModelBase
{
  [NotifyField]
  private string _firstName;

  private string _fullName;

  ////[Notifies(nameof(FullName))]
  [NotifyField]
  private string _lastName;

  [NotifyField]
  private string _title;

  public MainWindowViewModel()
  {
    Title = "Welcome to Prism.Avalonia.Toolkit - Code Generator!";
  }

  public string FullName => $"{FirstName} {LastName}";

  public DelegateCommand CmdPopulate => new(() =>
  {
    FirstName = "Incremental";
    LastName = "GeneratorSample";

    RaisePropertyChanged(nameof(FullName));
  });

  // Can't find this attribute due to library's access
  ////[Notifiable]
  ////public string SomeProperty { get; set; }

#pragma warning disable CA1822 // Mark members as static
  public string Greeting => "Hello from, Prism.Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}
