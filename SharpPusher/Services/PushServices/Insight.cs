using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices {
    public sealed class Insight : Api {
        public override string ToString() {
            return "Groestlsight";
        }

        public override async Task<Response<ResultWrapper>> PushTx(string txHex) {
            Response<ResultWrapper> resp = new Response<ResultWrapper>();
            var resultWrapper = new ResultWrapper();
            resultWrapper.TxnId = txHex;
            resultWrapper.Network = MainWindowViewModel.Networks.Mainnet;
            resultWrapper.Provider = ToString();

            using (HttpClient client = new HttpClient()) {
                try {
                    string url = "https://groestlsight.groestlcoin.org/api/tx/send";

                    var content = new FormUrlEncodedContent(new[] {
                                                                      new KeyValuePair<string, string>("rawtx", txHex)
                                                                  });

                    HttpResponseMessage httpResp = await client.PostAsync(url, content);

                    string result = await httpResp.Content.ReadAsStringAsync();
                    if (httpResp.IsSuccessStatusCode) {
                        JObject jResult = JObject.Parse(result);
                        resultWrapper.Output = "Successfully done. Tx ID: " + jResult["txid"];
                        resultWrapper.Result = Result.Success;
                    }
                    else {
                        resp.Errors.Add(result);
                        resultWrapper.Output = result;
                        resultWrapper.Result = Result.Failed;
                    }
                }
                catch (Exception ex) {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resultWrapper.Output = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resultWrapper.Result = Result.Failed;
                    resp.Errors.Add(errMsg);
                }
            }
            resp.Result = resultWrapper;
            return resp;
        }
    }
}