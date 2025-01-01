// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpPusher.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockchainInfo : IApi
    {
        public string ApiName => "Blockchain.Info";

        public async Task<Response> PushTx(string txHex)
        {
            using HttpClient client = new();
            Response resp = new();

            try
            {
                client.BaseAddress = new Uri("https://blockchain.info");

                string json = JsonConvert.SerializeObject(txHex);
                string contentType = "application/x-www-form-urlencoded";
                HttpContent httpContent = new MultipartFormDataContent
                {
                    new StringContent(json, Encoding.UTF8, contentType)
                };

                HttpResponseMessage result = await client.PostAsync("pushtx", httpContent);
                string sResult = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    if (sResult != null && sResult.StartsWith("{\"error\":"))
                    {
                        JObject jObject = JObject.Parse(sResult);
                        resp.SetError(jObject["error"]?.ToString() ?? "");
                    }
                    else
                    {
                        resp.SetMessage(sResult ?? "");
                    }
                }
                else
                {
                    resp.SetError(sResult);
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
