// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using SharpPusher.Models;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockCypher : IApi
    {
        public string ApiName => "BlockCypher";


        public async Task<Response> PushTx(string txHex)
        {
            Response resp = new();
            using HttpClient client = new();

            try
            {
                JObject tx = new()
                {
                    {"tx", txHex}
                };

                string url = "https://api.blockcypher.com/v1/bcy/test/txs/push";
                HttpResponseMessage httpResp = await client.PostAsync(url, new StringContent(tx.ToString()));
                if (!httpResp.IsSuccessStatusCode)
                {
                    resp.SetError("API response doesn't indicate success.");
                    return resp;
                }

                string t = await httpResp.Content.ReadAsStringAsync();
                JObject jResult = JObject.Parse(t);
                if (jResult["error"] != null)
                {
                    resp.SetError(jResult["error"]?.ToString() ?? "");
                }
                else
                {
                    resp.SetMessage($"Successfully done. Tx ID: {jResult["hash"]}");
                }
            }
            catch (Exception ex)
            {
                string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                resp.SetError(errMsg);
            }

            return resp;
        }

    }
}
