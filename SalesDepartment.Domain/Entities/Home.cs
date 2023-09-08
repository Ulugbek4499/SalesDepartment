using SalesDepartment.Domain.Commons;

namespace SalesDepartment.Domain.Entities
{
    public class Home:BaseAuditableEntity
    {
        public string Block { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string ApartmentNumber { get; set; }
        public double Area { get; set; }
    }
}