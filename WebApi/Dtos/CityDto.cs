using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "Name is mandatory")]
        [StringLength (50, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }
            
    }
}
