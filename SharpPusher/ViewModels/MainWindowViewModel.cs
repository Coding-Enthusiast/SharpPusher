﻿// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Autarkysoft.Bitcoin.Blockchain.Scripts;
using Autarkysoft.Bitcoin.Blockchain.Transactions;
using Autarkysoft.Bitcoin.Encoders;
using SharpPusher.MVVM;
using SharpPusher.Services;
using SharpPusher.Services.PushServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace SharpPusher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            NetworkList = new ObservableCollection<Networks>((Networks[])Enum.GetValues(typeof(Networks)));
            _selNet = NetworkList[0];
            SetApiList();
            _selApi = ApiList[0];

            BroadcastTxCommand = new BindableCommand(BroadcastTx, CanBroadcast);

            Version ver = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
            VersionString = ver.ToString(3);

            _rawTx = string.Empty;
        }

        public string VersionString { get; }


        private string _rawTx;
        public string RawTx
        {
            get => _rawTx;
            set
            {
                if (SetField(ref _rawTx, value))
                {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public enum Networks
        {
            Bitcoin,
            BitcoinTestnet,
            BitcoinCash,
            BitcoinSV,
            Dogecoin,
            Litecoin,
            Monero,
            Zcash,
            Ethereum,
            EthereumTestnet,
            Dash,
            Ripple,
            Groestlcoin,
            Stellar,
            Cardano,
            Mixin,
            Tezos,
            EOS,
            BitcoinABC
        }

        public ObservableCollection<Networks> NetworkList { get; set; }

        private Networks _selNet;
        public Networks SelectedNetwork
        {
            get { return _selNet; }
            set
            {
                if (SetField(ref _selNet, value))
                {
                    SetApiList();
                }
            }
        }
        private void SetApiList()
        {
            switch (SelectedNetwork)
            {
                case Networks.Bitcoin:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.BTC),
                        new Smartbit(),
                        new BlockCypher(),
                        new P2P(true)
                    };
                    break;
                case Networks.BitcoinCash:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.BCH),
                    };
                    break;
                case Networks.Dogecoin:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.DOGE),
                    };
                    break;
                case Networks.Litecoin:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.LTC),
                    };
                    break;
                case Networks.Monero:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.XMR),
                    };
                    break;
                case Networks.BitcoinTestnet:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.TBTC),
                        new P2P(false)
                    };
                    break;
                case Networks.BitcoinSV:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.BSV),
                    };
                    break;
                case Networks.Zcash:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.ZEC),
                    };
                    break;
                case Networks.Ripple:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.XRP),
                    };
                    break;
                case Networks.Stellar:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.XLM),
                    };
                    break;
                case Networks.Cardano:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.ADA),
                    };
                    break;
                case Networks.Mixin:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.XIN),
                    };
                    break;
                case Networks.Tezos:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.XTZ),
                    };
                    break;
                case Networks.EOS:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.EOS),
                    };
                    break;
                case Networks.Ethereum:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.ETH),
                    };
                    break;
                case Networks.EthereumTestnet:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.ΤETH),
                    };
                    break;
                case Networks.Groestlcoin:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.GRS),
                    };
                    break;
                case Networks.Dash:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.DASH),
                    };
                    break;
                case Networks.BitcoinABC:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.ABC),
                    };
                    break;
            }
        }


        private ObservableCollection<Api> _apiList = new();
        public ObservableCollection<Api> ApiList
        {
            get => _apiList;
            set => SetField(ref _apiList, value);
        }


        private Api _selApi;
        public Api SelectedApi
        {
            get => _selApi;
            set
            {
                if (SetField(ref _selApi, value))
                {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }


        [DependsOnProperty(nameof(SelectedNetwork))]
        public bool IsCheckTxVisible => SelectedNetwork == Networks.Bitcoin || SelectedNetwork == Networks.BitcoinTestnet ||
                                        SelectedNetwork == Networks.BitcoinCash || SelectedNetwork == Networks.BitcoinABC ||
                                        SelectedNetwork == Networks.BitcoinSV || SelectedNetwork == Networks.Litecoin ||
                                        SelectedNetwork == Networks.Dogecoin;

        private bool _checkTx = true;
        public bool CheckTx
        {
            get => _checkTx;
            set => SetField(ref _checkTx, value);
        }

        public static string CheckTxToolTip => "Enable to check the transaction hex using Bitcoin.Net library by deserializing " +
            "and evaluating all its scripts.";


        private bool _isSending;
        public bool IsSending
        {
            get => _isSending;
            set
            {
                if (SetField(ref _isSending, value))
                {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }



        public BindableCommand BroadcastTxCommand { get; private set; }
        private async void BroadcastTx()
        {
            Errors = string.Empty;

            if (!Base16.TryDecode(RawTx, out byte[] result))
            {
                Status = "Invalid hex.";
                return;
            }

            if (IsCheckTxVisible && CheckTx)
            {
                Debug.Assert(result != null);
                var stream = new FastStreamReader(result);
                var tx = new Transaction();
                if (!tx.TryDeserialize(stream, out string error))
                {
                    Status = $"Invalid transaction. Error message: {error}";
                    return;
                }

                for (int i = 0; i < tx.TxInList.Length; i++)
                {
                    if (!tx.TxInList[i].SigScript.TryEvaluate(ScriptEvalMode.Legacy, out _, out _, out error))
                    {
                        Status = $"Invalid input signature script at index {i}. Error message: {error}";
                        return;
                    }
                }
                for (int i = 0; i < tx.TxOutList.Length; i++)
                {
                    if (!tx.TxOutList[i].PubScript.TryEvaluate(ScriptEvalMode.Legacy, out _, out _, out error))
                    {
                        Status = $"Invalid input pubkey script at index {i}. Error message: {error}";
                        return;
                    }
                }
            }

            IsSending = true;
            Status = "Broadcasting Transaction...";

            Response<string> resp = await SelectedApi.PushTx(RawTx);
            if (resp.Errors.Any())
            {
                Errors = resp.Errors.GetErrors();
                Status = "Finished with error.";
            }
            else
            {
                Status = resp.Result;
            }
            IsSending = false;
        }
        private bool CanBroadcast()
        {
            if (!string.IsNullOrWhiteSpace(RawTx) && !IsSending && SelectedApi != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
