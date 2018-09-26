using SynthGuru.BusinessLayer.DTO;
using SynthGuru.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.BusinessLayer
{
    public class AdminBL
    {
        BusinessLayer bll;

        private Manufacturer[] ManufacturerSeedData = new Manufacturer[] { new Manufacturer { Name = "Korg", Country = "Japan" },
                                                                           new Manufacturer { Name = "Roland", Country = "Japan"},
                                                                           new Manufacturer { Name = "Moog Music", Country = "USA"},
                                                                           new Manufacturer { Name = "Sequential Circuits", Country = "USA"},
                                                                           new Manufacturer { Name = "ARP", Country = "Japan"}};

        private SynthModel[] SynthModelSeedData = new SynthModel[] { new SynthModel { ManufacturerId = 1,
                                                                                      Name = "PolySix",
                                                                                      Year = 1981,
                                                                                      Polyphony = "6 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "32 presets"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 1,
                                                                                      Name = "Mono/Poly",
                                                                                      Year = 1981,
                                                                                      Polyphony = "4 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 1,
                                                                                      Name = "MS-20",
                                                                                      Year = 1978,
                                                                                      Polyphony = "Monophonic",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 2,
                                                                                      Name = "Jupiter-8",
                                                                                      Year = 1981,
                                                                                      Polyphony = "8 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "64 presets"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 2,
                                                                                      Name = "Juno-106",
                                                                                      Year = 1982,
                                                                                      Polyphony = "6 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "56 presets"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 2,
                                                                                      Name = "System-100",
                                                                                      Year = 1979,
                                                                                      Polyphony = "Monophonic",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 3,
                                                                                      Name = "MiniMoog",
                                                                                      Year = 1970,
                                                                                      Polyphony = "Monophonic",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId =3,
                                                                                      Name = "PolyMoog",
                                                                                      Year = 1975,
                                                                                      Polyphony = "71 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 3,
                                                                                      Name = "MiniMoog Voyager",
                                                                                      Year = 2002,
                                                                                      Polyphony = "Monophonic",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "128 presets"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 4,
                                                                                      Name = "Prophet-5",
                                                                                      Year = 1978,
                                                                                      Polyphony = "5 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "100 presets"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 4,
                                                                                      Name = "Six-Trak",
                                                                                      Year = 1984,
                                                                                      Polyphony = "6 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 4,
                                                                                      Name = "Pro-One",
                                                                                      Year = 1981,
                                                                                      Polyphony = "Monophonic",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 5,
                                                                                      Name = "Odyssey",
                                                                                      Year = 1972,
                                                                                      Polyphony = "2 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 5,
                                                                                      Name = "2600",
                                                                                      Year = 1971,
                                                                                      Polyphony = "Monophonic",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "None"
                                                                                    },
                                                                     new SynthModel { ManufacturerId = 5,
                                                                                      Name = "Quadra",
                                                                                      Year = 1978,
                                                                                      Polyphony = "3 voices",
                                                                                      SynthesisTypeId = 1,
                                                                                      StorageMemory = "16 presets"
                                                                                    }};

        public AdminBL()
        {
            bll = new BusinessLayer();
        }

        public GenericResponse Reset(ResetModel model)
        {
            var resp = new GenericResponse();

            try
            {
                // Validate passwords - hardcoded in this example
                if (model.Password1 != "wipeall!")
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Invalid password 1";

                    return resp;
                }

                if (model.Password2 != "andseed!")
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Invalid password 2";

                    return resp;
                }

                // Reset database
                var resetResp = bll.ResetDatabase();
                if (!resetResp.IsOK)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"An error occurred while resetting database: {resetResp.ErrorMsg}";

                    return resp;
                }

                // Reseed table data
                bll.AddManufacturer(ManufacturerSeedData);
                bll.AddSynthModel(SynthModelSeedData);
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
