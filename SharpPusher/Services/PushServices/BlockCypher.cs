using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SharpPusher.Services.PushServices
{
    public class BlockCypher : Api
    {
        public override string ApiName
        {
            get { return "BlockCypher"; }
        }


        public override async Task<Response<string>> PushTx(string txHex)
        {
            Response<string> resp = await base.PushTx(txHex, "tx", "https://api.blockcypher.com/v1/bcy/test/txs/push");
            if (resp.HasErrors)
            {
                return resp;
            }

            JObject jResult = JObject.Parse(resp.Result);
            if (jResult["error"] != null)
            {
                resp.AddError(jResult["error"].ToString());
            }
            else
            {
                resp.Result = "Successfully done. Tx ID: " + jResult["hash"].ToString();
            }

            return resp;
        }

    }
}
