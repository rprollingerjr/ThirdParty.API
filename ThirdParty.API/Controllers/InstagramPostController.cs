using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThirdParty.API.Models;
using ThirdParty.API.Interfaces;

namespace ThirdParty.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramPostController : ControllerBase
    {
        private readonly IInstagramPostManager _instagramPostManager;

        public InstagramPostController(IInstagramPostManager instagramPostManager)
        {
            _instagramPostManager = instagramPostManager;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var user = _instagramPostManager.GetPosts();
            return new JsonResult(user);
        }
    }
}
