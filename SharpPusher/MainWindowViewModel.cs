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
            versionString = string.Format("Version {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }


        private string versionString;
        public string VersionString
        {
            get { return versionString; }
        }


        private string rawTx;
        public string RawTx
        {
            get { return rawTx; }
            set
            {
                if (SetField(ref rawTx, value))
                {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public enum Networks
        {
            Mainnet,
            Testnet
        }

        public ObservableCollection<Networks> NetworkList { get; set; }

        private Networks selectedNetwork;
        public Networks SelectedNetwork
        {
            get { return selectedNetwork; }
            set
            {
                if (SetField(ref selectedNetwork, value))
                {
                    SetApiList();
                }
            }
        }
        private void SetApiList()
        {
            switch (SelectedNetwork)
            {
                case Networks.Mainnet:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Insight(),
                        new BlockBook(),
                        //new Blockr(),
                        //new Smartbit(),
                        //new BlockCypher(),
                        //new BlockExplorer(),
                        //new BlockchainInfo() /*I can't get BLockchain.info API to work, and the code is exact copy of their repository! So I'll just remove it for now (only from this list) until I can fix it.*/
                    };
                    break;
                case Networks.Testnet:
                    ApiList = new ObservableCollection<Api>()
                    {
                        new Insight_Test(),
                        //new BlockDozer(),
                    };
                    break;
            }
        }


        private ObservableCollection<Api> apiList;
        public ObservableCollection<Api> ApiList
        {
            get { return apiList; }
            set { SetField(ref apiList, value); }
        }


        private Api selectedApi = new BlockBook();
        public Api SelectedApi
        {
            get { return selectedApi; }
            set
            {
                if (SetField(ref selectedApi, value))
                {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private bool isSending;
        public bool IsSending
        {
            get { return isSending; }
            set
            {
                if (SetField(ref isSending, value))
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
            if (!string.IsNullOrWhiteSpace(RawTx) && !IsSending && selectedApi != null)
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
