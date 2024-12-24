using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLingo.Core.ViewModel.Component
{
    public class CourseNameValidator : IPromptValidator
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

        public bool IsValid(string? prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt)) return false;
            else if (prompt.Any(x => unrespectedChars.Contains(x))) return false;
            return true;
        }
    }
}
