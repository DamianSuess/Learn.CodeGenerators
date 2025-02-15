using Avalonia;
using Avalonia.Markup.Xaml;
using MvvmGenerator.Views;
using Prism.DryIoc;
using Prism.Ioc;

namespace MvvmGenerator;

public partial class App : PrismApplication
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);

    // Required when overriding Initialize
    base.Initialize();
  }

  protected override AvaloniaObject CreateShell()
  {
    return Container.Resolve<MainWindow>();
  }

  protected override void RegisterTypes(IContainerRegistry containerRegistry)
  {
    // Register you Services, Views, Dialogs, etc.
  }
}
