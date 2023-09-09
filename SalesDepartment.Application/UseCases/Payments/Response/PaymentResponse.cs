namespace SalesDepartment.Application.UseCases.Payments.Response
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string AmountsInWords { get; set; }
        public int ContractId { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
