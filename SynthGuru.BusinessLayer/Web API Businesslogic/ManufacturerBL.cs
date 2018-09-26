using SynthGuru.BusinessLayer.DTO;
using SynthGuru.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.BusinessLayer
{
    public class ManufacturerBL
    {
        BusinessLayer bll;

        public ManufacturerBL()
        {
            bll = new BusinessLayer();
        }

        public GenericResponse<List<ManufacturerDTO>> GetAll()
        {
            var resp = new GenericResponse<List<ManufacturerDTO>>();
            resp.ReturnObj = new List<ManufacturerDTO>();

            try
            {
                var currList = bll.GetAllManufacturers();

                resp.ReturnObj = (from a in currList
                                  select new ManufacturerDTO()
                                  {
                                      Id = a.Id,
                                      Name = a.Name,
                                      Country = a.Country
                                  }).ToList();
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse<ManufacturerDTO> GetById(int id)
        {
            var resp = new GenericResponse<ManufacturerDTO>();
            resp.ReturnObj = new ManufacturerDTO();

            try
            {
                var currManufacturer = bll.GetManufacturerById(id);
                if (currManufacturer == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Manufacturer with id: {id} not found";

                    return resp;
                }

                resp.ReturnObj.Id = currManufacturer.Id;
                resp.ReturnObj.Name = currManufacturer.Name;
                resp.ReturnObj.Country = currManufacturer.Country;
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse Create(ManufacturerEditDTO model)
        {
            var resp = new GenericResponse();

            try
            {
                // Ensure that manufacturer not already exis
                var currManufacturer = bll.GetManufacturerByName(model.Name);
                if (currManufacturer != null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Manufacturer with name: {model.Name} already exists";

                    return resp;
                }

                // Create the new entry
                var newEntry = new Manufacturer
                {
                    Name = model.Name,
                    Country = model.Country
                };

                bll.AddManufacturer(newEntry);
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse Modify(int id, ManufacturerEditDTO model)
        {
            var resp = new GenericResponse();

            try
            {
                // Ensure that manufacturer not already exis
                var currManufacturer = bll.GetManufacturerById(id);
                if (currManufacturer == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Manufacturer with id: {id} could not be found";

                    return resp;
                }

                // Modify entry
                currManufacturer.Name = model.Name;
                currManufacturer.Country = model.Country;

                bll.UpdateManufacturer(currManufacturer);
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse Delete(int id)
        {
            var resp = new GenericResponse();

            try
            {
                var currManufacturer = bll.GetManufacturerById(id);
                if (currManufacturer == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Manufacturer with id: {id} not found";

                    return resp;
                }

                // Check if there are existing synthmodels for this manufacturer
                var existingList = bll.GetSynthModelsByManufacturerId(id);
                if (existingList.Count > 0)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Manufacturer with id: {id} cannot be deleted because there are existing synth models for this manufacturer. Delete synth models first";

                    return resp;
                }

                // Delete entry
                bll.RemoveManufacturer(currManufacturer);
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }
    }
}
