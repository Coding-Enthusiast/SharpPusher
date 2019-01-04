using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockBook_Testnet : Api
    {
        public override string ToString() {
            return "BlockBook Testnet";
        }

        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = new Response<string>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://blockbook-test.groestlcoin.org/api/sendtx/" + txHex;

                    HttpResponseMessage httpResp = await client.GetAsync(url);

                    string result = await httpResp.Content.ReadAsStringAsync();
                    if (httpResp.IsSuccessStatusCode)
                    {
                        JObject jResult = JObject.Parse(result);
                        resp.Result = "Successfully done. Tx ID: " + jResult["txid"];
                    }
                    else
                    {
                        resp.Errors.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resp.Errors.Add(errMsg);
                }
            }

            return resp;
        }
    }
}
