namespace Company.Models
{
    public class Drivers
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string patronymic { get; set; }
        public string driverLicense { get; set; }
        public string contactInformation { get; set; }
        public string status { get; set; }

        //связи между моделями
        public ICollection<Flights> flightsId { get; set; } = new List<Flights>();
    }
}
