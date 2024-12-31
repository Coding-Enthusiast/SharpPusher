// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace SharpPusher.Models
{
    public class Response
    {
        public bool IsSuccess { get; private set; } = true;
        public string Message { get; private set; } = string.Empty;

        private void SetMsg(string msg, bool success)
        {
            IsSuccess = success;
            Message = msg;
        }
        public void SetMessage(string msg) => SetMsg(msg, true);
        public void SetError(string msg) => SetMsg(msg, false);
    }
}
