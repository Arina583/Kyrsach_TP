namespace Company.Models
{
    public class Buses
    {
        public int id { get; set; }
        public string stateNumber { get; set; }
        public string status { get; set; }
        public int numberSeat { get; set; }

        //связи между моделями
        public ICollection<Flights> flightsId { get; set; } = new List<Flights>();
    }
}
