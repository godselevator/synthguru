using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynthGuru.BusinessLayer.DTO
{
    public class ManufacturerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class ManufacturerEditDTO
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class SynthModelDTO
    {
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Polyphony { get; set; }
        public int SynthesisTypeId { get; set; }
        public string StorageMemory { get; set; }
    }

    public class SynthModelEditDTO
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Polyphony { get; set; }
        public int SynthesisTypeId { get; set; }
        public string StorageMemory { get; set; }
    }

    public class ResetModel
    {
        public string Password1 { get; set; }
        public string Password2 { get; set; }
    }
}