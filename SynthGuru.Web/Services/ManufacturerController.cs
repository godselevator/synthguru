using Newtonsoft.Json;
using SynthGuru.BusinessLayer;
using SynthGuru.BusinessLayer.DTO;
using SynthGuru.DomainModel;
using SynthGuru.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SynthGuru.Web.Services
{
    [RoutePrefix("api/Manufacturer")]
    public class ManufacturerController : ApiController
    {
        /// <summary>
        /// Get all synthesizer manufacturers
        /// </summary>
        /// <returns>List of synthesizer manufacturers</returns>
        public HttpResponseMessage Get()
        {
            var bl = new ManufacturerBL();

            var resp = bl.GetAll();

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) : 
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(resp.ReturnObj, Formatting.None));
        }

        /// <summary>
        /// Get a single synthesizer manufacturer by id
        /// </summary>
        /// <param name="id">Manufacturer id</param>
        /// <returns>A single synthesizer manufacturer</returns>
        public HttpResponseMessage Get(int id)
        {
            var bl = new ManufacturerBL();

            var resp = bl.GetById(id);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(resp.ReturnObj, Formatting.None));
        }

        /// <summary>
        /// Create a new synthesizer manufacturer
        /// </summary>
        /// <param name="model">New manufacturer entry</param>
        public HttpResponseMessage Post([FromBody]ManufacturerEditDTO model)
        {
            var bl = new ManufacturerBL();

            var resp = bl.Create(model);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.Created, JsonConvert.SerializeObject("Manufacturer successfully created", Formatting.None));
        }

        /// <summary>
        /// Modify synthesizer manufacturer
        /// </summary>
        /// <param name="id">Id of manufacturer to modify</param>
        /// <param name="value">Modified manufacturer data</param>
        public HttpResponseMessage Put(int id, [FromBody]ManufacturerEditDTO model)
        {
            var bl = new ManufacturerBL();

            var resp = bl.Modify(id, model);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject("Manufacturer successfully modified", Formatting.None));
        }

        /// <summary>
        /// Delete an existing synthesizer manufacturer
        /// </summary>
        /// <param name="id">Id of manufacturer to delete</param>
        public HttpResponseMessage Delete(int id)
        {
            var bl = new ManufacturerBL();

            var resp = bl.Delete(id);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject("Manufacturer successfully deleted", Formatting.None));
        }
    }
}
