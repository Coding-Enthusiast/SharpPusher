using MVVMLib;
using SharpPusher.Services;
using SharpPusher.Services.PushServices;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace SharpPusher
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            NetworkList = new ObservableCollection<Networks>((Networks[])Enum.GetValues(typeof(Networks)));
            SetApiList();

            BroadcastTxCommand = new BindableCommand(BroadcastTx, CanBroadcast);

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            VersionString = ver.ToString(3);
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


        private ObservableCollection<Api> _apiList;
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
