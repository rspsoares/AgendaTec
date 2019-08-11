namespace AgendaTec.Business.Entities
{
    public class ScheduleReportDTO
    {
        public string Date { get; set; }
        public string ConsumerName { get; set; }
        public string ServiceDescription { get; set; }
        public decimal Price { get; set; }
        public string Attended { get; set; }        
    }
}
