using SynthGuru.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.BusinessLayer
{
    public partial class BusinessLayer
    {
        private readonly IManufacturerRepository _ManufacturerRepository;
        private readonly ISynthesisTypeRepository _SynthesisTypeRepository;
        private readonly ISynthModelRepository _SynthModelRepository;

        public BusinessLayer()
        {
            _ManufacturerRepository = new ManufacturerRepository();
            _SynthesisTypeRepository = new SynthesisTypeRepository();
            _SynthModelRepository = new SynthModelRepository();
        }

        public GenericResponse ResetDatabase()
        {
            var resp = new GenericResponse();

            try
            {
                var dalAdmin = new DALAdmin();

                dalAdmin.ResetDatabase();
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
