// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using SharpPusher.Models;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockCypher : Api
    {
        public override string ApiName => "BlockCypher";


        public override async Task<Response> PushTx(string txHex)
        {
            Response resp = await PushTx(txHex, "tx", "https://api.blockcypher.com/v1/bcy/test/txs/push");
            if (!resp.IsSuccess)
            {
                return resp;
            }

            JObject jResult = JObject.Parse(resp.Message);
            if (jResult["error"] != null)
            {
                resp.SetError(jResult["error"].ToString());
            }
            else
            {
                resp.SetMessage($"Successfully done. Tx ID: {jResult["hash"]}");
            }

            return resp;
        }

    }
}
