using Newtonsoft.Json;
using SynthGuru.BusinessLayer;
using SynthGuru.BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SynthGuru.Web.Services
{
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        /// <summary>
        /// Resets/re-seeds database
        /// </summary>
        /// <param name="password"></param>
        [Route("ResetDatabase")]
        public HttpResponseMessage Post([FromBody]ResetModel model)
        {
            // API-key validation
            if (!ValidateAPIKey())
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Invalid API-key");
            }

            var bl = new AdminBL();

            var resp = bl.Reset(model);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.Created, JsonConvert.SerializeObject("Database successfully reset", Formatting.None));

        }

        private bool ValidateAPIKey()
        {
            try
            {
                string uri = Request.RequestUri.AbsoluteUri;
                string queryString = new Uri(uri).Query;
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);
                var apiKey = queryDictionary["api_Key"];

                var apiKeyFromConfigFile = ConfigurationManager.AppSettings["APIKey"];

                return (apiKey == apiKeyFromConfigFile);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
