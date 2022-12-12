using WebAPI.Models;

namespace WebApi.Dtos
{
    public class PropertyDetailDto : PropertyListDto
    {
        public int CarpetArea { get; set; }
        public string Adress { get; set; }
        public string Adress2 { get; set; }
        public int FloorNo { get; set; }
        public int TotalFloors { get; set; }    
        public string MainEntrance { get; set; }
        public int Security { get; set; }
        public bool Gated { get; set; }
        public int Maintenance { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
        public ICollection<PhotoDto> MyProperty { get; set; }
    }
}