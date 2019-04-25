namespace AgendaTech.Business.Entities
{
    public class ScheduleDTO
    {
        public int IDSchedule { get; set; }
        public int IDProfessional { get; set; }
        public string ProfessionalName { get; set; }
        public string ServiceName { get; set; }
        public string ConsumerName { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public decimal Price { get; set; }
        public int Time { get; set; }
        public bool Bonus { get; set; }
    }
}
