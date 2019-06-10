namespace AgendaTec.Business.Entities
{
    public class ScheduleDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public int IdProfessional { get; set; }
        public string ProfessionalName { get; set; }
        public int IdService { get; set; }
        public string ServiceName { get; set; }
        public string IdConsumer { get; set; }
        public string ConsumerName { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public string Finish { get; set; }
        public decimal Price { get; set; }
        public int Time { get; set; }
        public bool Bonus { get; set; }
        public bool Attended { get; set; }
    }
}
