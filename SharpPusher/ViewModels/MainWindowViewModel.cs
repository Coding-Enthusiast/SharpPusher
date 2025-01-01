// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin;
using Autarkysoft.Bitcoin.Blockchain.Scripts;
using Autarkysoft.Bitcoin.Blockchain.Transactions;
using Autarkysoft.Bitcoin.Encoders;
using SharpPusher.Models;
using SharpPusher.MVVM;
using SharpPusher.Services;
using SharpPusher.Services.PushServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace SharpPusher.ViewModels
{
    public class MainWindowViewModel : InpcBase
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
        }

        public string VersionString { get; }


        private string _rawTx = string.Empty;
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


        public ObservableCollection<Networks> NetworkList { get; set; }

        private Networks _selNet;
        public Networks SelectedNetwork
        {
            get => _selNet;
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
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.BTC),
                        new BlockCypher(),
                        new P2P(true)
                    ];
                    break;
                case Networks.BitcoinCash:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.BCH),
                    ];
                    break;
                case Networks.Dogecoin:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.DOGE),
                    ];
                    break;
                case Networks.Litecoin:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.LTC),
                    ];
                    break;
                case Networks.Monero:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.XMR),
                    ];
                    break;
                case Networks.BitcoinTestnet:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.TBTC),
                        new P2P(false)
                    ];
                    break;
                case Networks.BitcoinSV:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.BSV),
                    ];
                    break;
                case Networks.Zcash:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.ZEC),
                    ];
                    break;
                case Networks.Ripple:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.XRP),
                    ];
                    break;
                case Networks.Stellar:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.XLM),
                    ];
                    break;
                case Networks.Cardano:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.ADA),
                    ];
                    break;
                case Networks.Mixin:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.XIN),
                    ];
                    break;
                case Networks.Tezos:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.XTZ),
                    ];
                    break;
                case Networks.EOS:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.EOS),
                    ];
                    break;
                case Networks.Ethereum:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.ETH),
                    ];
                    break;
                case Networks.EthereumTestnet:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.ΤETH),
                    ];
                    break;
                case Networks.Groestlcoin:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.GRS),
                    ];
                    break;
                case Networks.Dash:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.DASH),
                    ];
                    break;
                case Networks.BitcoinABC:
                    ApiList =
                    [
                        new Blockchair(Blockchair.Chain.ABC),
                    ];
                    break;
            }
        }


        private ObservableCollection<IApi> _apiList = new();
        public ObservableCollection<IApi> ApiList
        {
            get => _apiList;
            set => SetField(ref _apiList, value);
        }


        private IApi _selApi;
        public IApi SelectedApi
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

        private string _msg = string.Empty;
        public string Message
        {
            get => _msg;
            set => SetField(ref _msg, value);
        }

        private State _state = State.Ready;
        public State CurrentState
        {
            get => _state;
            set => SetField(ref _state, value);
        }


        [DependsOnProperty(nameof(SelectedNetwork))]
        public bool IsCheckTxVisible => SelectedNetwork is Networks.Bitcoin
                                                        or Networks.BitcoinTestnet
                                                        or Networks.BitcoinCash
                                                        or Networks.BitcoinABC
                                                        or Networks.BitcoinSV
                                                        or Networks.Litecoin
                                                        or Networks.Dogecoin;

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
            Message = string.Empty;
            CurrentState = State.Ready;

            if (!Base16.TryDecode(RawTx, out byte[] result))
            {
                Message = "Invalid hex.";
                return;
            }

            if (IsCheckTxVisible && CheckTx)
            {
                Debug.Assert(result != null);
                FastStreamReader stream = new(result);
                Transaction tx = new();
                if (!tx.TryDeserialize(stream, out Errors error))
                {
                    Message = $"Could not deserialize transaction. Error message:{Environment.NewLine}{error.Convert()}";
                    return;
                }

                for (int i = 0; i < tx.TxInList.Length; i++)
                {
                    if (!tx.TxInList[i].SigScript.TryEvaluate(ScriptEvalMode.Legacy, out _, out _, out error))
                    {
                        Message = $"Could not evaluate {(i + 1).ToOrdinal()} input's signature script. " +
                                  $"Error message:{Environment.NewLine}{error}";
                        return;
                    }
                }
                for (int i = 0; i < tx.TxOutList.Length; i++)
                {
                    if (!tx.TxOutList[i].PubScript.TryEvaluate(ScriptEvalMode.Legacy, out _, out _, out error))
                    {
                        Message = $"Could not evaluate {(i + 1).ToOrdinal()} output's pubkey script. " +
                                  $"Error message: {error}";
                        return;
                    }
                }
            }

            IsSending = true;
            CurrentState = State.Broadcasting;

            Response resp = await SelectedApi.PushTx(RawTx);
            Message = resp.Message;
            CurrentState = resp.IsSuccess ? State.Success : State.Failed;

            IsSending = false;
        }
        private bool CanBroadcast()
        {
            return !string.IsNullOrWhiteSpace(RawTx) && !IsSending && SelectedApi != null;
        }
    }
}
