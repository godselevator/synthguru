using SynthGuru.BusinessLayer.DTO;
using SynthGuru.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.BusinessLayer
{
    public class SynthModelBL
    {
        BusinessLayer bll;

        public SynthModelBL()
        {
            bll = new BusinessLayer();
        }

        public GenericResponse<List<SynthModelDTO>> GetAll()
        {
            var resp = new GenericResponse<List<SynthModelDTO>>();
            resp.ReturnObj = new List<SynthModelDTO>();

            try
            {
                var currList = bll.GetAllSynthModels();

                resp.ReturnObj = (from a in currList
                                  select new SynthModelDTO()
                                  {
                                      Id = a.Id,
                                      ManufacturerId = a.ManufacturerId,
                                      Name = a.Name,
                                      Year = a.Year,
                                      Polyphony = a.Polyphony,
                                      SynthesisTypeId = a.SynthesisTypeId,
                                      StorageMemory = a.StorageMemory
                                  }).ToList();
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse<SynthModelDTO> GetById(int id)
        {
            var resp = new GenericResponse<SynthModelDTO>();
            resp.ReturnObj = new SynthModelDTO();

            try
            {
                var currSynthModel = bll.GetSynthModelById(id);
                if (currSynthModel == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"SynthModel with id: {id} not found";

                    return resp;
                }

                resp.ReturnObj.Id = currSynthModel.Id;
                resp.ReturnObj.ManufacturerId = currSynthModel.ManufacturerId;
                resp.ReturnObj.Name = currSynthModel.Name;
                resp.ReturnObj.Year = currSynthModel.Year;
                resp.ReturnObj.Polyphony = currSynthModel.Polyphony;
                resp.ReturnObj.SynthesisTypeId = currSynthModel.SynthesisTypeId;
                resp.ReturnObj.StorageMemory = currSynthModel.StorageMemory;
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse<List<SynthModelDTO>> GetByManufacturerName(string name)
        {
            var resp = new GenericResponse<List<SynthModelDTO>>();
            resp.ReturnObj = new List<SynthModelDTO>();

            try
            {
                // Validate the Manufacturer name
                var currManufacturer = bll.GetManufacturerByName(name);
                if (currManufacturer == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Manufacturer name: {name} could not be found";

                    return resp;
                }

                var currList = bll.GetSynthModelsByManufacturerId(currManufacturer.Id);

                resp.ReturnObj = (from a in currList
                                  select new SynthModelDTO()
                                  {
                                      Id = a.Id,
                                      ManufacturerId = a.ManufacturerId,
                                      Name = a.Name,
                                      Year = a.Year,
                                      Polyphony = a.Polyphony,
                                      SynthesisTypeId = a.SynthesisTypeId,
                                      StorageMemory = a.StorageMemory
                                  }).ToList();
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse Create(SynthModelEditDTO model)
        {
            var resp = new GenericResponse();

            try
            {
                // Ensure that SynthModel not already exist
                var currSynthModel = bll.GetSynthModelByName(model.Name);
                if (currSynthModel != null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"SynthModel with name: {model.Name} already exists";

                    return resp;
                }

                // Validate Manufacturer
                var currManufacturer = bll.GetManufacturerById(model.ManufacturerId);
                if (currManufacturer == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"ManufacturerId: {model.ManufacturerId} does not exist";

                    return resp;
                }

                // Validate SynthesisType
                var currSynthesisType = bll.GetSynthesisTypeById(model.SynthesisTypeId);
                if (currSynthesisType == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"SynthesisTypeId: {model.SynthesisTypeId} does not exist";

                    return resp;
                }

                // Create the new entry
                var newEntry = new SynthModel
                {
                    ManufacturerId = model.ManufacturerId,
                    Name = model.Name,
                    Year = model.Year,
                    Polyphony = model.Polyphony,
                    SynthesisTypeId = model.SynthesisTypeId,
                    StorageMemory = model.StorageMemory
                };

                bll.AddSynthModel(newEntry);
            }
            catch (Exception ex)
            {
                resp.IsOK = false;
                resp.ErrorMsg = $"Exception: {ex.Message}";
            }

            return resp;
        }

        public GenericResponse Modify(int id, SynthModelEditDTO model)
        {
            var resp = new GenericResponse();

            try
            {
                // Ensure that SynthModel not already exis
                var currSynthModel = bll.GetSynthModelById(id);
                if (currSynthModel == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"SynthModel with id: {id} could not be found";

                    return resp;
                }

                // Modify entry
                currSynthModel.ManufacturerId = model.ManufacturerId;
                currSynthModel.Name = model.Name;
                currSynthModel.Year = model.Year;
                currSynthModel.Polyphony = model.Polyphony;
                currSynthModel.SynthesisTypeId = model.SynthesisTypeId;
                currSynthModel.StorageMemory = model.StorageMemory;

                bll.UpdateSynthModel(currSynthModel);
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
                var currSynthModel = bll.GetSynthModelById(id);
                if (currSynthModel == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"SynthModel with id: {id} not found";

                    return resp;
                }

                // Delete entry
                bll.RemoveSynthModel(currSynthModel);
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
