using AOS.DataAccessLayer;
using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<Country> GetAllCountrys();
        Country GetCountryById(int CountryId);
        Country GetCountryByName(string CountryName);
        void AddCountry(params Country[] Countrys);
        void UpdateCountry(params Country[] Countrys);
        void RemoveCountry(params Country[] Countrys);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Country> GetAllCountrys()
        {
            return _CountryRepository.GetAll();
        }

        public Country GetCountryById(int CountryId)
        {
            // Return only Country. Skip related tables
            return _CountryRepository.GetSingle(d => d.CountryID.Equals(CountryId));
        }


        public IList<Country> GetCountryListByName(string searchStr)
        {
            // Return only Country. Skip related tables
            return _CountryRepository.GetList(d => d.Name.ToLower().Contains(searchStr.ToLower()));
        }

        public Country GetCountryByName(string CountryName)
        {
            // Return Country and all related tables
            return _CountryRepository.GetSingle(d => d.Name.ToLower().Equals(CountryName.ToLower()));
        }

        public void AddCountry(params Country[] Countrys)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in Countrys)
            {
                //item.CreateDate = DateTime.Now;
                //item.CreateUser = _CurrentUser;
            }

            int numberOfRowsAdded = _CountryRepository.Add(Countrys);

            // Post-processing. When new Countrys are added, all System users must be updated in the UserRoleAccount table
            if (numberOfRowsAdded > 0)
                UpdateSystemUsers();
        }

        public void UpdateCountry(params Country[] Countrys)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in Countrys)
            {
                //item.UpdateDate = DateTime.Now;
                //item.UpdateUser = _CurrentUser;
            }

            _CountryRepository.Update(Countrys);
        }

        public void RemoveCountry(params Country[] Countrys)
        {
            /* Validation and error handling omitted */

            int numberOfRowsRemoved = _CountryRepository.Remove(Countrys);

            // Post-processing. When new Countrys are added, all System users must be updated in the UserRoleAccount table
            if (numberOfRowsRemoved > 0)
                UpdateSystemUsers();
        }
    }
}
