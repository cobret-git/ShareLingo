using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLingo.WinUI.ViewModel
{
    public enum DialogButtons { Ok, OkCancel, YesNo, YesNoCancel }
    public enum DialogResult { None, Ok, Yes, No, Cancel }
    public interface IDialogViewModel
    {
        string Message { get; }
        DialogButtons Buttons { get; }
        DialogResult Result { get; }
    }
}
