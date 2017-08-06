using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public sealed class Blockr : Api
    {
        public override string ApiName
        {
            get { return "Blockr.io"; }
        }


        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = await PushTx(txHex, "hex", "http://btc.blockr.io/api/v1/tx/push");
            if (resp.Errors.Any())
            {
                return resp;
            }

            JObject jResult = JObject.Parse(resp.Result);
            if (jResult["status"].ToString() == "success")
            {
                resp.Result = "Successfully done. Tx ID: " + jResult["data"].ToString();
            }
            else
            {
                string er = (jResult["message"] == null) ? jResult["data"].ToString() : jResult["data"].ToString() + " " + jResult["message"].ToString();
                resp.Errors.Add(er);
            }

            return resp;
        }

    }
}
