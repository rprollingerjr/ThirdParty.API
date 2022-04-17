using System;
namespace ThirdParty.API.Models
{
    public class InstagramPostModel
    {
        public InstagramPostModel()
        {

        }

        public long? ID { get; set; }

        public string Username { get; set; }

        public string MediaType { get; set; }

        public string MediaURL { get; set; }

        public string PostDate { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
