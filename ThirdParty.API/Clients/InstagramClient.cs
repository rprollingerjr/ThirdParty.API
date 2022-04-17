using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThirdParty.API.Interfaces;
using ThirdParty.API.Models;

namespace ThirdParty.API.Clients
{
    public class InstagramClient : IInstagramClient
    {
        private readonly IRestClient _restClient;
        public InstagramClient(IRestClient restClient)
        {
            _restClient = restClient;
        }

        private const string DefaultBaseAddress = "https://graph.instagram.com"; 

        JObject IInstagramClient.Get(string resource, Dictionary<string, object> queryParams)
        {
            RestRequest request = SetupRequest(resource, Method.GET, queryParams);

            JObject result = new JObject();
            _restClient.BaseUrl = new Uri(DefaultBaseAddress);

            try
            {
                IRestResponse response = _restClient.Execute(request);
                result = HandleResponse(response, Method.GET);
            }
            catch (Exception ex)
            {
                result.Add("Message", $"An exception occurred when trying to get data from Instragram \n\n{ex.ToString()}");
                result.Add("Success", false);
            }

            var datas = result["data"];

            return result;
        }

        internal RestRequest SetupRequest(string resource, Method method, Dictionary<string, object> query)
        {
            RestRequest request = new RestRequest(resource, method);

            foreach (KeyValuePair<string, object> param in query)
            {
                Parameter q = new Parameter(param.Key, param.Value, ParameterType.QueryString);

                request.AddParameter(q);
            }

            return request;
        }

        internal JObject HandleResponse(IRestResponse response, Method method)
        {
            JObject result = new JObject();

            if (response == null)
            {
                result.Add("Success", false);
                result.Add("Message", "Error: Could not parse response. Response was null");
                return result;
            }

            int statusCode = (int)response.StatusCode;
            string errorException = response.ErrorException?.ToString() ?? "";
            string errorMessage = response.ErrorMessage?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                //return an error response content is null or whitespace. Try to use Error Exception info if any exists for error message.
                result.Add("Success", false);
                result.Add("Message", $"Response content is null. Exception: {errorException} ErrorMessage: {errorMessage} StatusCode:{statusCode} ResponseStatus:{response.ResponseStatus}");

                return result;
            }

            JObject responseMessage;
            try
            {
                //Response content is not null so try to deserialize content 
                responseMessage = JsonConvert.DeserializeObject<JObject>(response.Content);
            }
            catch (Exception ex)
            {
                result.Add("Success", false);
                result.Add("Message", $"An exception occurred trying to deserialize response.Content to JObject. Response.ErrorException: {errorException} Response.ErrorMessage: {errorMessage} StatusCode:{statusCode} ResponseStatus:{response.ResponseStatus} response.Content: {response?.Content}  Exception: {ex.ToString()}");
                return result;
            }

            string code = responseMessage["code"]?.Value<string>() ?? "";
            string message = responseMessage["msg"]?.Value<string>() ?? "";

            if (statusCode >= 200 && statusCode <= 399 && response.ResponseStatus == ResponseStatus.Completed)
            {
                if ((!string.IsNullOrWhiteSpace(code) && !string.Equals(code, "success", StringComparison.OrdinalIgnoreCase)) || ((method == Method.POST || method == Method.DELETE) && string.IsNullOrWhiteSpace(code)))
                {
                    //If successful status code is returned, but the code returned from Iterable is not success return error 
                    result.Add("Success", false);
                    result.Add("Message", $"Code: {code} IterableMessage: {message} StatusCode:{statusCode}");
                }
                else if (method == Method.GET)
                {
                    if (string.Equals("{}", response.Content))
                    {
                        //Return error since data requested from Iterable does not exist
                        result.Add("Message", "Does not exist");
                        result.Add("Success", false);
                    }
                    else
                    {
                        //Return data with no error
                        result = responseMessage;
                        result.Add("Success", true);
                        result.Add("Message", message);
                    }
                }
                else
                {
                    //return no error since it looks like request was successful
                    result.Add("Success", true);
                    result.Add("Message", message);
                }
            }
            else
            {
                //return an error since status code is not successful. Use response content data for message

                result.Add("Success", false);
                result.Add("Message", $"Code: {code} IterableMessage: {message} Exception:{response.ErrorException} ErrorMessage:{errorMessage} StatusCode:{statusCode} ResponseStatus:{response.ResponseStatus}");
            }
            return result;
        }

    }
}
