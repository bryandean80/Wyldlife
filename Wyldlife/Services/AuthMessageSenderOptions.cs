using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Wyldlife.Services
{
    public class AuthMessageSenderOptions
    {
       
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }

        public AuthMessageSenderOptions()
        {
            string configFile = File.ReadAllText("./config.json");
            dynamic config = JObject.Parse(configFile);
            SendGridUser = config.SendGridUser;
            SendGridKey = config.SendGridKey;
        }
    }
}
