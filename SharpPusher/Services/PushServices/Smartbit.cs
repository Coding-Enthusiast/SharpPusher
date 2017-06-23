using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class Smartbit : Api
    {
        public override string ApiName
        {
            get { return "Smartbit"; }
        }


        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = await PushTx(txHex, "hex", "https://api.smartbit.com.au/v1/blockchain/pushtx");
            if (resp.Errors.Any())
            {
                return resp;
            }

            JObject jResult = JObject.Parse(resp.Result);
            if ((bool)jResult["success"])
            {
                resp.Result = "Successfully done. Tx ID: " + jResult["txid"].ToString();
            }
            else
            {
                resp.Errors.Add(jResult["error"]["message"].ToString());
            }

            return resp;
        }

    }
}
