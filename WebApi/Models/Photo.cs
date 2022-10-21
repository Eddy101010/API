using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Photo: BaseEntity
    {
        [Required]
        public string PublicId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public bool IsPrimay { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}   