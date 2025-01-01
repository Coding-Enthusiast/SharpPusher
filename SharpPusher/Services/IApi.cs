// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using SharpPusher.Models;
using System.Threading.Tasks;

namespace SharpPusher.Services
{
    public interface IApi
    {
        string ApiName { get; }
        Task<Response> PushTx(string txHex);
    }
}
