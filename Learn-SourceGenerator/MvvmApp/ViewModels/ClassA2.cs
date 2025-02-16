using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace MvvmApp.ViewModels;

// Tests that two partial classes can double-inherit
// But not different classes such as, ViewModelBase & BindableBase
public partial class ClassA : BindableBase
{
}
