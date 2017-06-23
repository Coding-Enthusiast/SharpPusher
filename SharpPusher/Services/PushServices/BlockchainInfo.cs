using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class BlockchainInfo : Api
    {
        public override string ApiName
        {
            get { return "Blockchain.Info"; }
        }

        public async override Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = new Response<string>();

            using (HttpClient client = new HttpClient())
            {
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
                            resp.Errors.Add(jObject["error"].ToString());
                        }
                        else
                        {
                            resp.Result = sResult;
                        }
                    }
                    else
                    {
                        resp.Errors.Add(sResult);
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
