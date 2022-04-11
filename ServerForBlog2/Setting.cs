using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer {
    public class Setting {
        public string IPAddress { get; set; }
        public string RootDir { get; set; }
       

        public Setting() {
            IPAddress = "127.0.0.1";
            RootDir = "";
        }
    }
}
