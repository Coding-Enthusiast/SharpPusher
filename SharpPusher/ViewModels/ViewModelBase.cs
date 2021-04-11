// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using SharpPusher.MVVM;

namespace SharpPusher.ViewModels
{
    public class ViewModelBase : InpcBase
    {
        /// <summary>
        /// Used for changing the visibility of error message TextBox.
        /// </summary>
        [DependsOnProperty(nameof(Errors))]
        public bool IsErrorMsgVisible => !string.IsNullOrEmpty(Errors);


        private string _errors = string.Empty;
        /// <summary>
        /// String containing all the errors.
        /// </summary>
        public string Errors
        {
            get => _errors;
            set => SetField(ref _errors, value);
        }

        private string _status = string.Empty;
        /// <summary>
        /// Status, showing current action being performed.
        /// </summary>
        public string Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }
    }
}
