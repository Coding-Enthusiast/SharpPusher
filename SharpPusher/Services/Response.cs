using System.Collections.Generic;
using System.Text;

namespace SharpPusher.Services
{
    public class Response<T> : Response
    {
        public T Result { get; set; }
    }

    public class Response
    {
        public bool HasErrors { get; internal set; }

        internal List<string> ErrorList = new List<string>();

        /// <summary>
        /// Adds one error to the list of errors and changes the flag to indicate existance of errors.
        /// </summary>
        /// <param name="errorMessage">Error string to add.</param>
        internal void AddError(string errorMessage)
        {
            ErrorList.Add(errorMessage);
            HasErrors = true;
        }

        /// <summary>
        /// Adds multiple errors to the list of errors and changes the flag to indicate existance of errors.
        /// </summary>
        /// <param name="multiError">List of errors to add.</param>
        internal void AddError(List<string> multiError)
        {
            multiError.ForEach(x => ErrorList.Add(x));
            HasErrors = true;
        }

        /// <summary>
        /// Clears all errors and changes the flag to indicate no existance of errors.
        /// </summary>
        internal void ClearErrors()
        {
            ErrorList.Clear();
            HasErrors = false;
        }

        /// <summary>
        /// Returns a formated string of all the errors.
        /// </summary>
        /// <returns>Formatted string of all the errors</returns>
        public string GetErrors()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in ErrorList)
            {
                sb.AppendLine("- " + item);
            }
            return sb.ToString();
        }
    }
}
