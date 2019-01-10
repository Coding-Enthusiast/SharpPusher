using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPusher
{
    public class ResultWrapper
    {
        public ResultWrapper() {
            CreatedTime = DateTime.Now;
        }

        public DateTime? CreatedTime { get; set; }
        public string TxnId { get; set; }
        public string Result { get; set; }
        public string Output { get; set; }
    }
}
