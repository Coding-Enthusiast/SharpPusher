using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class Blockchair : Api
    {
        public Blockchair(Chain chain)
        {
            this.chain = chain;
        }

        public enum Chain
        {
            BTC,
            BCH,
            DOGE,
            LTC,
            XMR,
            TBTC,
            BSV,
            ZEC,
            XRP,
            tETH,
            ETH,
            EOS,
            XTZ,
            XIN,
            ADA,
            XLM,
            GRS,
            DASH,
            ABC
        }

        private readonly Chain chain;

        public override string ApiName => "Blockchair";

        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = new Response<string>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string chainName = "";
                    if(chain == Chain.BTC)
                    {
                        chainName = "bitcoin";
                    }else if(chain == Chain.TBTC)
                    {
                        chainName = "bitcoin/testnet";
                    }else if(chain == Chain.BCH)
                    {
                        chainName = "bitcoin-cash";
                    }else if(chain == Chain.DOGE)
                    {
                        chainName = "dogecoin";
                    }else if(chain == Chain.LTC)
                    {
                        chainName = "litecoin";
                    }else if(chain == Chain.XMR)
                    {
                        chainName = "monero";
                    }else if(chain == Chain.ADA)
                    {
                        chainName = "cardano";
                    }else if(chain == Chain.BSV)
                    {
                        chainName = "bitcoin-sv";
                    }else if(chain == Chain.EOS)
                    {
                        chainName = "eos";
                    }else if(chain == Chain.ETH)
                    {
                        chainName = "ethereum";
                    }else if(chain == Chain.tETH)
                    {
                        chainName = "ethereum/testnet";
                    }else if(chain == Chain.XIN)
                    {
                        chainName = "mixin";
                    }else if(chain == Chain.XLM)
                    {
                        chainName = "stellar";
                    }else if(chain == Chain.XRP)
                    {
                        chainName = "ripple";
                    }else if(chain == Chain.XTZ)
                    {
                        chainName = "tezos";
                    }else if (chain == Chain.DASH)
                    {
                        chainName = "dash";
                    }else if (chain == Chain.GRS)
                    {
                        chainName = "groestlcoin";
                    }else if (chain == Chain.ABC)
                    {
                        chainName = "bitcoin-abc";
                    }


                    string url = $"https://api.blockchair.com/{chainName}/push/transaction";

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("data", txHex)
                    });

                    HttpResponseMessage httpResp = await client.PostAsync(url, content);

                    string result = await httpResp.Content.ReadAsStringAsync();
                    if (httpResp.IsSuccessStatusCode)
                    {
                        JObject jResult = JObject.Parse(result);
                        resp.Result = "Successfully done. Tx ID: " + jResult["data"]["transaction_hash"].ToString();
                    }
                    else if (httpResp.StatusCode == HttpStatusCode.BadRequest)
                    {
                        JObject jResult = JObject.Parse(result);
                        resp.Result = "Bad request: " + jResult["context"]["error"].ToString();
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
