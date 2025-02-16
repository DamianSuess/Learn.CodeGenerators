using System.ComponentModel;

namespace MvvmApp.ViewModels;

// Tests that two partial classes can double-inherit using an interface
// But cannot inherit from two classes such as, ViewModelBase & BindableBase
// even though ViewModelBase inherits from BindableBase.
public partial class ClassA : INotifyPropertyChanged
{
}
