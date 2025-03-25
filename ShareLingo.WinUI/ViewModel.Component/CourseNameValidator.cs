using NetForge.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CONTENT = ShareLingo.WinUI.Resources.Strings.Content;

namespace ShareLingo.WinUI.ViewModel.Component
{
    public class CourseNameValidator : IAttachedTextValidator
    {
        #region Fields
        private char[] unrespectedChars;
        #endregion

        #region Constructors
        public CourseNameValidator(char[] unrespectedChars)
        {
            this.unrespectedChars = unrespectedChars;
        }
        #endregion

        public bool IsValid(string? prompt, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                message = CONTENT.courseNameValidator_nameCannotBeEmpty;
                return false;
            }
            else if (prompt.Any(x => unrespectedChars.Contains(x)))
            {
                message = CONTENT.courseNameValidator_nameCannotContainIncorrectSymbols;
                return false;
            }
            return true;
        }
    }
}
