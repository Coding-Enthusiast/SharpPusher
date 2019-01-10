using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices {
    public sealed class BlockBook_Testnet : Api {
        public override string ToString() {
            return "BlockBook Testnet";
        }

        public override async Task<Response<ResultWrapper>> PushTx(string txHex) {
            Response<ResultWrapper> resp = new Response<ResultWrapper>();
            var resultWrapper = new ResultWrapper();
            resultWrapper.TxnId = txHex;
            resultWrapper.Network = MainWindowViewModel.Networks.Testnet;
            resultWrapper.Provider = ToString();

            using (HttpClient client = new HttpClient()) {
                try {
                    string url = "https://blockbook-test.groestlcoin.org/api/sendtx/" + txHex;

                    HttpResponseMessage httpResp = await client.GetAsync(url);

                    string result = await httpResp.Content.ReadAsStringAsync();
                    if (httpResp.IsSuccessStatusCode) {
                        JObject jResult = JObject.Parse(result);
                        resultWrapper.Output = "Successfully done. Tx ID: " + jResult["txid"];
                        resultWrapper.Result = "Success";
                    }
                    else {
                        resultWrapper.Result = "Fail";
                        resultWrapper.Output = result;
                        resp.Errors.Add(result);
                    }
                }
                catch (Exception ex) {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resultWrapper.Output = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resultWrapper.Result = "Failed";
                    resp.Errors.Add(errMsg);
                }
            }
            resp.Result = resultWrapper;
            return resp;
        }
    }
}