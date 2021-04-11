// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Encoders;
using MVVMLib;
using SharpPusher.Services;
using SharpPusher.Services.PushServices;
using System;
using System.Collections.ObjectModel;
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
            BitcoinCash
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
                    };
                    break;
                case Networks.BitcoinCash:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Blockchair(Blockchair.Chain.BCH),
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
            if (!Base16.IsValid(RawTx))
            {
                Status = "Invalid hex.";
                return;
            }

            IsSending = true;
            Errors = string.Empty;
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
