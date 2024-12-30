// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace SharpPusher.Services
{
    public class Response<T>
    {
        private readonly ErrorCollection errors = new ErrorCollection();

        public ErrorCollection Errors
        {
            get { return errors; }
        }

        public T Result { get; set; }
    }
}
