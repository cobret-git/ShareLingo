using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLingo.Core.ViewModel.Component
{
    public enum DialogButtons { Ok, OkCancel, YesNo, YesNoCancel }
    public enum DialogResult { None, Ok, Yes, No, Cancel }
    public interface IDialogViewModel
    {
        string Title { get; }
        string Message { get; }
        DialogButtons Buttons { get; }
        DialogResult Result { get; }
    }
}
