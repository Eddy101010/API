using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebAPI.Controllers;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebApi.Controllers
{
    public class PropertyController : BaseController
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public PropertyController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet("list/{sellRent}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyList(int sellRent)
        {
            var properties = await uow.PropertyRepository.GetPropertiesAsync(sellRent);
            var propertyListDto = mapper.Map<IEnumerable<PropertyListDto>>(properties);
            return Ok(propertyListDto);
        }

        [HttpGet("detail/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyDetail(int id)
        {
            var property = await uow.PropertyRepository.GetPropertyDetailAsync(id);
            var propertyDto = mapper.Map<PropertyDetailDto>(property);
            return Ok(propertyDto);
        }

        [HttpPost("add/property")]
        [Authorize]
        public async Task<IActionResult> AddProperty(PropertyDto propertyDto)
        {
            var property = mapper.Map<Property>(propertyDto);
            var userId = GetUserId();
            property.PostedBy = userId; 
            property.LastUpdatedBy = userId;
            uow.PropertyRepository.AddProperty(property);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        //add photo to cloud
        [HttpPost("add/photo/{propId}")]
        [Authorize]
        public async Task<IActionResult> AddPropertyCloudPhoto(IFormFile file, int propId)   
        {
            var result = await photoService.UploadPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var property = await uow.PropertyRepository.GetPropertyByIdAsync(propId); 

            var photo = new Photo
            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            property.Photos.Add(photo);
            if (property.Photos.Count == 1)
            {
                photo.IsPrimary = true;
            }

            
            await uow.SaveAsync();
            return StatusCode(201);
        }

        //update primary photo from cloud
        [HttpPost("add/set-primary-photo/{propId}/{photoPublicId}")]
        [AllowAnonymous]
        public async Task<IActionResult> SetCloudPrimaryPhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();

            var property = await uow.PropertyRepository.GetPropertyByIdAsync(propId);

            if (property == null)
                return BadRequest("No property of photo exists");

            if (property.PostedBy != userId)
                return BadRequest("You are not authorized to change the photo");

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);

            if (photo == null) 
            return BadRequest("No property of photo exists");

            if (photo.IsPrimary)
                return BadRequest("This already is a primary photo");

            var currentPrimary = property.Photos.FirstOrDefault(p => p.IsPrimary);
            if (currentPrimary != null) currentPrimary.IsPrimary = false;
            photo.IsPrimary = true;

            if (await uow.SaveAsync()) return NoContent();
            
            return BadRequest("Failed to set primary photo");
                
        }

        //delete photo from cloud and database
        [HttpDelete("delete-photo/{propId}/{photoPublicId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCloudPhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();

            var property = await uow.PropertyRepository.GetPropertyByIdAsync(propId);

            if (property == null)
                return BadRequest("No property of photo exists");

            if (property.PostedBy != userId)
                return BadRequest("You are not authorized to delete the photo");

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);

            if (photo == null)
                return BadRequest("No property of photo exists");

            if (photo.IsPrimary)
                return BadRequest("Not authorized to delete primary photo");

            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);

            property.Photos.Remove(photo);

            if (await uow.SaveAsync()) return Ok();

            return BadRequest("Failed to delete photo");

        }

        //add static photo
        [HttpPost("UploadStaticFile")]
        [AllowAnonymous]
        public bool UploadFile(int id, IFormFile file)
        {

            try
            {
                string name = file.FileName;
                string extension = Path.GetExtension(file.FileName);
                //read the file
                string path = Path.Combine(Environment.CurrentDirectory, "wwwroot/images");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (Stream fileStream = new FileStream(Path.Combine(path, id.ToString() + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //delete static photo   
        [HttpDelete("delete-static-photo/{id}")]
        [AllowAnonymous]
        public bool DeleteStaticFile(int id, IFormFile file)
        {
            string name = file.FileName;
            string extension = Path.GetExtension(file.FileName);
            //read the file
            string path = Path.Combine(Environment.CurrentDirectory, "wwwroot/images");
            if (!Directory.Exists(path))
            {
                return false;
            }
            var location = Path.Combine(path, id.ToString() + extension);
            if (System.IO.File.Exists(location))
                System.IO.File.Delete(location);
                      
            return true;
        }

    }
}
