using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThirdParty.API.Clients;
using ThirdParty.API.Interfaces;
using ThirdParty.API.Models;

namespace ThirdParty.API.Managers
{
    public class InstagramPostManager : IInstagramPostManager
    {
        private readonly IInstagramClient _instagramClient; 

        public InstagramPostManager(IInstagramClient instagramClient)
        {
            _instagramClient = instagramClient;
        }

        public List<InstagramPostModel> GetPosts()
        {
            var qp = new Dictionary<string, object>();

            var fields = "id,media_type,media_url,username,timestamp";
            qp.Add(nameof(fields), fields);

            var access_token = "IGQVJWVW9PczFyVTZAMa0kwTEdrTkNUQUQwRDZA4VFRkTWhGZAklVbEEtU2daUE96TDhpY0lLemtZAQXFxZAUo1cUZAFY3ZAkSmlGaUVfZA1ZAmUjJhREZAJSGJKWjlsN3BtbHZAnNHp5RnNxZAlJzSW5jSEJCZA3JVSQZDZD";
            qp.Add(nameof(access_token), access_token);

            JObject result = _instagramClient.Get("me/media", qp);

            JToken successValue;

            var models = new List<InstagramPostModel>();
            if (result.TryGetValue("Success", out successValue) && successValue.Value<bool>())
            {
                var mediaData = result.GetValue("data").Value<JToken>();
                var localModels = mediaData.ToObject<List<InstagramPostLocalModel>>();

                string errorMessage = "";
                foreach (InstagramPostLocalModel post in localModels)
                {
                    var model = new InstagramPostModel();
                    if (TryParsePost(ref model, post, ref errorMessage))
                    {
                        model.Success = true;
                        models.Add(model);
                    }
                }
            }
            else
            {
                return null;
            }


            return models;
        }

        private bool TryParsePost(ref InstagramPostModel model, InstagramPostLocalModel dataModel, ref string error)
        {
            try
            {
                model.ID = dataModel.ID;
                model.MediaType = dataModel.MediaType;
                model.MediaURL = dataModel.MediaURL;
                model.PostDate = dataModel.PostDate;
                model.Username = dataModel.Username;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return true;
        }
    }
}
