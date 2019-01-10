using MVVMLib;
using SharpPusher.Services;
using SharpPusher.Services.PushServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SharpPusher {
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel() {
            NetworkList = new ObservableCollection<Networks>((Networks[]) Enum.GetValues(typeof(Networks)));
            SetApiList();

            BroadcastTxCommand = new BindableCommand(BroadcastTx, CanBroadcast);

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            versionString = $"Version {ver.Major}.{ver.Minor}.{ver.Build}";
        }

        private string versionString;

        public string VersionString {
            get { return versionString; }
        }

        private string rawTx;

        public string RawTx {
            get { return rawTx; }
            set {
                if (SetField(ref rawTx, value)) {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public enum Networks {
            Mainnet,
            Testnet
        }

        public ObservableCollection<Networks> NetworkList { get; set; }

        private Networks selectedNetwork = Networks.Mainnet;

        public Networks SelectedNetwork {
            get { return selectedNetwork; }
            set {
                if (SetField(ref selectedNetwork, value)) {
                    SetApiList();
                }
            }
        }

        private void SetApiList() {
            switch (SelectedNetwork) {
                case Networks.Mainnet:
                    ApiList = new ObservableCollection<Api> {
                                                                new Chainz(),
                                                                new BlockBook(),
                                                                new Insight(),
                                                            };
                    break;
                case Networks.Testnet:
                    ApiList = new ObservableCollection<Api> {
                                                                new Chainz_Test(),
                                                                new BlockBook_Testnet(),
                                                                new Insight_Test(),
                                                            };
                    break;
            }
            try {
                SelectedApi = ApiList.FirstOrDefault();
            }
            catch { }
        }

        private ObservableCollection<Api> apiList;

        public ObservableCollection<Api> ApiList {
            get { return apiList; }
            set { SetField(ref apiList, value); }
        }

        private ObservableCollection<ResultWrapper> resultList = new ObservableCollection<ResultWrapper>();

        public ObservableCollection<ResultWrapper> ResultList {
            get { return resultList; }
            set { SetField(ref resultList, value); }
        }

        private Api selectedApi;

        public Api SelectedApi {
            get { return selectedApi; }
            set {
                if (SetField(ref selectedApi, value)) {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool isSending;

        public bool IsSending {
            get { return isSending; }
            set {
                if (SetField(ref isSending, value)) {
                    BroadcastTxCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public BindableCommand BroadcastTxCommand { get; private set; }

        private async void BroadcastTx() {
            IsSending = true;
            Errors = string.Empty;
            Status = "Broadcasting Transaction...";

            Response<ResultWrapper> resp = await SelectedApi.PushTx(RawTx);

            ResultList.Add(resp.Result);
            
            if (resp.Errors.Any()) {
                Errors = resp.Errors.GetErrors();
            }
            Status = resp.Result.Result;
            IsSending = false;
        }

        private bool CanBroadcast() {
            return !string.IsNullOrWhiteSpace(RawTx) && !IsSending && selectedApi != null;
        }


    }
}