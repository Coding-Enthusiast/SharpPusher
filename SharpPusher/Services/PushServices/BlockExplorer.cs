using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public class BlockExplorer : Api
    {
        public override string ApiName
        {
            get { return "BlockExplorer"; }
        }

        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = new Response<string>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://blockexplorer.com/api/tx/send";

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>( "rawtx", txHex)
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
                        resp.AddError(result);
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resp.AddError(errMsg);
                }
            }

            return resp;
        }

    }
}
