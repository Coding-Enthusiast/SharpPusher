using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Resources;

namespace SharpPusher.Services.PushServices {
    public sealed class Chainz_Test : Api {
        public override string ToString() => "Chainz Testnet";

        public override async Task<Response<string>> PushTx(string txHex) {
            Response<string> resp = new Response<string>();

            try {
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri("https://chainz.cryptoid.info/grs-test/");
                    string contentType = "text/plain";

                    HttpContent httpContent = new StringContent(txHex, Encoding.UTF8, contentType);

                    HttpResponseMessage result = await client.PostAsync($"api.dws?q=pushtx&key={ChainzKey}", httpContent);
                    string sResult = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode) {
                        if (sResult != null && sResult.StartsWith("{\"error\":")) {
                            JObject jObject = JObject.Parse(sResult);
                            resp.Errors.Add(jObject["error"].ToString());
                        }
                        else {
                            resp.Result = sResult;
                        }
                    }
                    else {
                        resp.Errors.Add(sResult);
                    }
                }
            }
            catch (Exception ex) {
                string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                resp.Errors.Add(errMsg);
            }
            return resp;
        }
    }
}