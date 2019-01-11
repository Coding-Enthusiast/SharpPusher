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
    public sealed class Chainz : Api {
        public override string ToString() => "Chainz";

        public override async Task<Response<ResultWrapper>> PushTx(string txHex) {
            Response<ResultWrapper> resp = new Response<ResultWrapper>();
            var resultWrapper = new ResultWrapper();
            resultWrapper.TxnId = txHex;
            resultWrapper.Network = MainWindowViewModel.Networks.Mainnet;
            resultWrapper.Provider = ToString();

            try {
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri("https://chainz.cryptoid.info/grs/");
                    string contentType = "text/plain";

                    HttpContent httpContent = new StringContent(txHex, Encoding.UTF8, contentType);

                    HttpResponseMessage result = await client.PostAsync($"api.dws?q=pushtx&key={ChainzKey}", httpContent);
                    string sResult = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode) {
                        if (sResult != null && sResult.StartsWith("{\"error\":")) {
                            JObject jObject = JObject.Parse(sResult);
                            resultWrapper.Result = Enum.GetName(typeof(Result), Result.Failed);
                            resultWrapper.Output = jObject["error"].ToString();
                            resp.Errors.Add(jObject["error"].ToString());
                        }
                        else {
                            resultWrapper.Result = Enum.GetName(typeof(Result), Result.Success);
                            resultWrapper.Output = sResult;
                        }
                    }
                    else {
                        resultWrapper.Result = Enum.GetName(typeof(Result), Result.Failed);
                        resultWrapper.Output = sResult;
                        resp.Errors.Add(sResult);
                    }
                }
            }
            catch (Exception ex) {
                string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                resultWrapper.Output = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                resultWrapper.Result = Enum.GetName(typeof(Result), Result.Failed);
                resp.Errors.Add(errMsg);
            }
            resp.Result = resultWrapper;
            return resp;
        }
    }
}