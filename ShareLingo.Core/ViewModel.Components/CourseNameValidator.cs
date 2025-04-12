using NetForge.Core;
using System;
using System.Linq;
using CONTENT = ShareLingo.Core.Resources.Content;

namespace ShareLingo.Core.ViewModel.Component
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
