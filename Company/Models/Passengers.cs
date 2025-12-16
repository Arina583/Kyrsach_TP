namespace Company.Models
{
    public class Passengers
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string patronymic { get; set; }
        public string passportData { get; set; }
        public string email { get; set; }

        //связи между моделями 
        public ICollection<Tickets> ticketId { get; set; } = new List<Tickets>();
    }
}
