// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Newtonsoft.Json.Linq;
using SharpPusher.Models;
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
            ΤETH,
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

        public override async Task<Response> PushTx(string txHex)
        {
            using HttpClient client = new();
            Response resp = new();

            try
            {
                string chainName = chain switch
                {
                    Chain.BTC => "bitcoin",
                    Chain.TBTC => "bitcoin/testnet",
                    Chain.BCH => "bitcoin-cash",
                    Chain.DOGE => "dogecoin",
                    Chain.LTC => "litecoin",
                    Chain.XMR => "monero",
                    Chain.ADA => "cardano",
                    Chain.BSV => "bitcoin-sv",
                    Chain.EOS => "eos",
                    Chain.ETH => "ethereum",
                    Chain.ΤETH => "ethereum/testnet",
                    Chain.XIN => "mixin",
                    Chain.XLM => "stellar",
                    Chain.XRP => "ripple",
                    Chain.XTZ => "tezos",
                    Chain.DASH => "dash",
                    Chain.GRS => "groestlcoin",
                    Chain.ABC => "bitcoin-abc",
                    Chain.ZEC => "zcash",
                    _ => throw new ArgumentException("Undefined Chain")
                };


                string url = $"https://api.blockchair.com/{chainName}/push/transaction";

                var content = new FormUrlEncodedContent(
                [
                        new KeyValuePair<string, string>("data", txHex)
                ]);

                HttpResponseMessage httpResp = await client.PostAsync(url, content);

                string result = await httpResp.Content.ReadAsStringAsync();
                if (httpResp.IsSuccessStatusCode)
                {
                    JObject jResult = JObject.Parse(result);
                    resp.SetMessage($"Successfully done. Tx ID: {jResult["data"]?["transaction_hash"]}");
                }
                else if (httpResp.StatusCode == HttpStatusCode.BadRequest)
                {
                    JObject jResult = JObject.Parse(result);
                    resp.SetError($"Bad request: {jResult["context"]?["error"]}");
                }
                else
                {
                    resp.SetError(result);
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
