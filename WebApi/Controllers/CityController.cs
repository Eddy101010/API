using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase


    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public CityController(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            throw new UnauthorizedAccessException();
            var cities = await uow.CityRepository.GetCitiesAsync();
            var citiesDto = mapper.Map<IEnumerable<CityDto>>(cities);

            return Ok(citiesDto);
        }

        //Post data in JSON format
        [HttpPost("post")]
        public async Task<IActionResult> AddCity(CityDto cityDto)
        {
            var city = mapper.Map<City>(cityDto);
            city.LastUpdatedBy = 1;
            city.LastUpdatedOn = DateTime.Now;

            //var city = new City {
            //    Name = cityDto.Name,
            //    LastUpdatedBy = 1,
            //    LastUpdatedOn = DateTime.Now
            //};

            uow.CityRepository.AddCity(city);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
        {
                if (id != cityDto.Id)
                    return BadRequest("Update denied");

                var cityFromDb = await uow.CityRepository.FindCity(id);
                if (cityFromDb == null)
                    return BadRequest("Update denied");

                cityFromDb.LastUpdatedBy = 1;
                cityFromDb.LastUpdatedOn = DateTime.Now;
                mapper.Map(cityDto, cityFromDb);

                throw new Exception("Unknown error occured");
                await uow.SaveAsync();
                return StatusCode(200);


                return StatusCode(500, "Unknown error has occured");
                
        }

        [HttpPut("updateCityName/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityUpdateDto cityDto)
        {
           var cityFromDb = await uow.CityRepository.FindCity(id);
            cityFromDb.LastUpdatedBy = 1;
            cityFromDb.LastUpdatedOn = DateTime.Now;
           mapper.Map(cityDto, cityFromDb);
           await uow.SaveAsync();
            return StatusCode(200);
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCityPatch(int id, JsonPatchDocument <City> citytoPatch)
        {
            var cityFromDb = await uow.CityRepository.FindCity(id);
            cityFromDb.LastUpdatedBy = 1;
            cityFromDb.LastUpdatedOn = DateTime.Now;

            citytoPatch.ApplyTo(cityFromDb, ModelState);
            await uow.SaveAsync();
            return StatusCode(200);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            uow.CityRepository.DeleteCity(id);
            await uow.SaveAsync();
            return Ok(id);  
        }

        //[HttpPost]
        //[HttpPost("add/{cityname")]
        //public async Task<IActionResult> AddCity(string cityName)
        //public async Task<IActionResult> AddCity(string cityName)
        //{
        //    City city = new City();
        //   city.Name = cityName;
        //  await dc.Cities.AddAsync(city);
        //    await dc.SaveChangesAsync();
        //  return Ok(city);
        //}

    }
}
