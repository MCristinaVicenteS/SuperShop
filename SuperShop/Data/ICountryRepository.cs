using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Data.Entities;
using SuperShop.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        //método q devolve os países com as cidades
        IQueryable GetCountriesWithCities();

        //através do id devolve o objecto país
        Task<Country> GetCountryWithCitiesAsync(int id);

        //através do id devolve o objecto cidade
        Task<City> GetCityAsync(int id);

        //recebe o modelo e adiciona-lhe a cidade
        Task AddCityAsync(CityViewModel model);

        //método q vai fazer o update das cidades - devolve um int q é o id da cidade q ele fez o uptdate
        Task<int> UpdateCityAsync(City city);

        //método para apagar as cidades
        Task<int> DeleteCityAsync(City city);

        //método q preenche a combo dos países
        IEnumerable<SelectListItem> GetComboCountries();

        //método q recebe o id do país e carrega a combo das cidades c as cidd desse país
        IEnumerable<SelectListItem> GetComboCities(int countryId);

        //Task q recebe uma cidade e retorna um país
        Task<Country> GetCountryAsync(City city);
    }
}
