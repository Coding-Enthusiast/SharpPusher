using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockBook : Api
    {
        public override string ApiName => "BlockBook";

        public override string ToString() {
            return "BlockBook";
        }

        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = new Response<string>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://blockbook.groestlcoin.org/api/sendtx";

                    Response<string> respnu = await PushTx(txHex, "hex", url);

                    var res = respnu.Result;

                    string json = JsonConvert.SerializeObject(txHex);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("hex", txHex)
                    });

                    HttpResponseMessage httpResp = await client.PostAsync(url, content);

                    string result = await httpResp.Content.ReadAsStringAsync();
                    if (httpResp.IsSuccessStatusCode)
                    {
                        JObject jResult = JObject.Parse(result);
                        resp.Result = "Successfully done. Tx ID: " + jResult["txid"].ToString();
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
