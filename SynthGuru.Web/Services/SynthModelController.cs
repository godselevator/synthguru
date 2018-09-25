using Newtonsoft.Json;
using SynthGuru.BusinessLayer;
using SynthGuru.BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SynthGuru.Web.Services
{
    [RoutePrefix("api/SynthModel")]
    public class SynthModelController : ApiController
    {
        /// <summary>
        /// Get all synthesizer models
        /// </summary>
        /// <returns>List of synthesizer models</returns>
        public HttpResponseMessage Get()
        {
            var bl = new SynthModelBL();

            var resp = bl.GetAll();

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(resp.ReturnObj, Formatting.None));
        }

        /// <summary>
        /// Get a single synthesizer SynthModel by id
        /// </summary>
        /// <param name="id">SynthModel id</param>
        /// <returns>A single synthesizer SynthModel</returns>
        public HttpResponseMessage Get(int id)
        {
            var bl = new SynthModelBL();

            var resp = bl.GetById(id);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(resp.ReturnObj, Formatting.None));
        }

        /// <summary>
        /// Get a list of SynthModels by manufacturer id
        /// </summary>
        /// <param name="name">Manufacturer name</param>
        /// <returns>A list of SynthModels</returns>
        [HttpGet]
        [Route("GetByManufacturer")]
        public HttpResponseMessage GetByManufacturer(string name)
        {
            var bl = new SynthModelBL();

            var resp = bl.GetByManufacturerName(name);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(resp.ReturnObj, Formatting.None));
        }

        /// <summary>
        /// Create a new synthesizer SynthModel
        /// </summary>
        /// <param name="model">New SynthModel entry</param>
        public HttpResponseMessage Post([FromBody]SynthModelEditDTO model)
        {
            var bl = new SynthModelBL();

            var resp = bl.Create(model);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.Created, JsonConvert.SerializeObject("SynthModel successfully created", Formatting.None));
        }

        /// <summary>
        /// Modify synthesizer SynthModel
        /// </summary>
        /// <param name="id">Id of SynthModel to modify</param>
        /// <param name="value">Modified SynthModel data</param>
        public HttpResponseMessage Put(int id, [FromBody]SynthModelEditDTO model)
        {
            var bl = new SynthModelBL();

            var resp = bl.Modify(id, model);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject("SynthModel successfully modified", Formatting.None));
        }

        /// <summary>
        /// Delete an existing synthesizer SynthModel
        /// </summary>
        /// <param name="id">Id of SynthModel to delete</param>
        public HttpResponseMessage Delete(int id)
        {
            var bl = new SynthModelBL();

            var resp = bl.Delete(id);

            return (!resp.IsOK) ? Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resp.ErrorMsg) :
                Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject("SynthModel successfully deleted", Formatting.None));
        }
    }
}
