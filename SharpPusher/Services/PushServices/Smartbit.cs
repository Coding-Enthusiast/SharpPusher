using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public class Smartbit : Api
    {
        public override string ApiName
        {
            get { return "Smartbit"; }
        }


        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = await base.PushTx(txHex, "hex", "https://api.smartbit.com.au/v1/blockchain/pushtx");
            if (resp.HasErrors)
            {
                return resp;
            }

            JObject jResult = JObject.Parse(resp.Result);
            if ((bool)jResult["success"] == true)
            {
                resp.Result = "Successfully done. Tx ID: " + jResult["txid"].ToString();
            }
            else
            {
                resp.AddError(jResult["error"]["message"].ToString());
            }

            return resp;
        }

    }
}
