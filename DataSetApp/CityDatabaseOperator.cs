using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using NHibernate.Linq;
using System.IO;

namespace DataSetTest
{
    class CitiesDatabaseOperator
    {
        private readonly CitiesDataSet dataset;

        public CitiesDatabaseOperator()
        {
            dataset = new CitiesDataSet();
        }

        public int AddCountry(string name)
        {
            int newId = dataset.Countries.Count() + 1;
            dataset.Countries.AddCountriesRow(newId, name);
            dataset.AcceptChanges();
            return newId;
        }

        public int GetCountryId(string name) => 
            (from c in dataset.Countries where c.Name.ToLower() == name.ToLower() select c.Id).FirstOrDefault(); 

        public int AddCity(int countryId, string name)
        {
            int newId = dataset.Cities.Count() + 1;
            dataset.Cities.AddCitiesRow(newId, name, countryId);
            dataset.AcceptChanges();
            return newId;
        }

        public int GetCityId(string name) => 
            (from c in dataset.Cities where c.Name.ToLower() == name.ToLower() select c.Id).FirstOrDefault();
        

        public List<string> GetCitiesForCountry(int countryId)
        {
            var result = from c in dataset.Cities where c.CountryId == countryId select c.Name;
            return result.ToList();
        }

        public List<CitiesDataSet.CitiesRow> FindCities(string pattern)
        {
            var result = from c in dataset.Cities where c.Name.ToLower().Contains(pattern.ToLower()) select c;
            return result.ToList();
        }

        public List<CitiesDataSet.CountriesRow> FindCountry(string pattern)
        {
            var result = from c in dataset.Countries where c.Name.ToLower().Contains(pattern.ToLower()) select c;
            return result.ToList();
        }

        public void WriteCitiesToXML(string filename)
        {
            dataset.Cities.WriteXml(filename, System.Data.XmlWriteMode.IgnoreSchema, false);
            
        }


    }
}
