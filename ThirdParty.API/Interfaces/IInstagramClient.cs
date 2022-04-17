using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ThirdParty.API.Models;

namespace ThirdParty.API.Interfaces
{
    public interface IInstagramClient
    {
        public JObject Get(string resource, Dictionary<string, object> queryParams);
    }
}
