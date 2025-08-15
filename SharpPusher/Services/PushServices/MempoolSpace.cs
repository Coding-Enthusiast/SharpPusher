// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Encoders;
using SharpPusher.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public class MempoolSpace : IApi
    {
        public string ApiName => "Mempool.space";

        public async Task<Response> PushTx(string txHex)
        {
            Response resp = new();
            using HttpClient client = new();

            try
            {
                string url = "https://mempool.space/api/tx";
                HttpResponseMessage httpResp = await client.PostAsync(url, new StringContent(txHex));
                if (!httpResp.IsSuccessStatusCode)
                {
                    resp.SetError("API response doesn't indicate success.");
                    return resp;
                }

                string t = await httpResp.Content.ReadAsStringAsync();
                if (t.Length == 64 && Base16.IsValid(t))
                {
                    resp.SetMessage($"Successfully done. Tx ID: {t}");
                }
                else
                {
                    resp.SetError(t);
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
