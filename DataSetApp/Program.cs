using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSetTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            int Id=1;
            CitiesDatabaseOperator db = new CitiesDatabaseOperator();
            Id = db.AddCountry("Россия");
            db.AddCity(Id, "Пермь");
            db.AddCity(Id, "Ухта");
            db.AddCity(Id, "Москва");

            Id = db.AddCountry("США");
            db.AddCity(Id, "Вашингтон");
            db.AddCity(Id, "Нью-Йорк");


            foreach (var city in db.GetCitiesForCountry((from country in db.FindCountry("США") select country.Id).First()))
            {
                Console.WriteLine(city);
            }

            Console.WriteLine(db.GetCountryId("америка"));



            // xml
            db.WriteCitiesToXML("cities.xml");




        }


    }
}
