namespace AgendaTec.Business.Entities
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Time { get; set; }
    }
}
