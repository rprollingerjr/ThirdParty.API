using System;
using System.Collections.Generic;
using ThirdParty.API.Models;

namespace ThirdParty.API.Interfaces
{
    public interface IInstagramPostManager
    {
        public List<InstagramPostModel> GetPosts();
    }
}
