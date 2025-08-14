// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Autarkysoft.Bitcoin.Blockchain.Transactions;
using Autarkysoft.Bitcoin.Clients;
using Autarkysoft.Bitcoin.P2PNetwork.Messages;
using Autarkysoft.Bitcoin.P2PNetwork.Messages.MessagePayloads;
using SharpPusher.Models;
using System.Threading.Tasks;

namespace SharpPusher.Services
{
    public class P2P : IApi
    {
        public P2P(bool isMainNet)
        {
            netType = isMainNet ? NetworkType.MainNet : NetworkType.TestNet3;
        }


        private readonly NetworkType netType;

        public static readonly string[] DnsMain = new string[]
        {
            "seed.bitcoin.sipa.be", // Pieter Wuille, only supports x1, x5, x9, and xd
            "dnsseed.bluematt.me", // Matt Corallo, only supports x9
            "dnsseed.bitcoin.dashjr.org", // Luke Dashjr
            "seed.bitcoinstats.com", // Christian Decker, supports x1 - xf
            "seed.bitcoin.jonasschnelli.ch", // Jonas Schnelli, only supports x1, x5, x9, and xd
            "seed.btc.petertodd.org", // Peter Todd, only supports x1, x5, x9, and xd
            "seed.bitcoin.sprovoost.nl", // Sjors Provoost
            "dnsseed.emzy.de", // Stephan Oeste
            "seed.bitcoin.wiz.biz", // Jason Maurice
        };

        public static readonly string[] DnsTest = new string[]
        {
            "testnet-seed.bitcoin.jonasschnelli.ch",
            "seed.tbtc.petertodd.org",
            "seed.testnet.bitcoin.sprovoost.nl",
            "testnet-seed.bluematt.me",
        };


        public string ApiName => "P2P";

        public async Task<Response> PushTx(string txHex)
        {
            Response resp = new();

            Transaction tx = new(txHex);
            Message msg = new(new TxPayload(tx), netType);

            MinimalClientSettings settings = new(netType, 4, null)
            {
                DnsSeeds = netType == NetworkType.MainNet ? DnsMain : DnsTest
            };
            using MinimalClient client = new(settings);
            client.Start();
            await Task.Delay(TimeConstants.MilliSeconds.FiveSec);
            client.Send(msg);

            resp.SetMessage($"Transaction was sent to {settings.MaxConnectionCount} nodes. Transaction ID: {tx.GetTransactionId()}");
            return resp;
        }
    }
}
