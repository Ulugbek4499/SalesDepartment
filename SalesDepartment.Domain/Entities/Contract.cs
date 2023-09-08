using SalesDepartment.Domain.Commons;

namespace SalesDepartment.Domain.Entities
{
    public class Contract:BaseAuditableEntity
    {
        public string ContractNumber { get; set; }
        public DateTime ContractStartDate { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Passport { get; set; }
        public string PassportIssuedBy  { get; set; }
        public string Address { get; set; }
        public string NumberOfRooms { get; set; }
        public double TotalAmountOfContract { get; set; }
        public DateTime ContractEndDate { get; set; }




    }
}