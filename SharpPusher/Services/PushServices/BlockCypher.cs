// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockCypher : Api
    {
        public override string ApiName
        {
            get { return "BlockCypher"; }
        }


        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = await PushTx(txHex, "tx", "https://api.blockcypher.com/v1/bcy/test/txs/push");
            if (resp.Errors.Any())
            {
                return resp;
            }

            JObject jResult = JObject.Parse(resp.Result);
            if (jResult["error"] != null)
            {
                resp.Errors.Add(jResult["error"].ToString());
            }
            else
            {
                resp.Result = "Successfully done. Tx ID: " + jResult["hash"].ToString();
            }

            return resp;
        }

    }
}
