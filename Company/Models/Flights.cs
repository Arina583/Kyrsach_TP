namespace Company.Models
{
    public class Flights
    {
        public int Id { get; set; }
        public int numberFlight { get; set; }
        public TimeOnly timeDeparture { get; set; }
        public TimeOnly timeArrival { get; set; }
        public string status { get; set; }

        //связи между моделями
        public Routes? Route { get; set; }
        public int RouteId { get; set; } //Внешний ключ для routeId
        public Buses? Bus { get; set; }
        public int BusId { get; set; } //Внешний ключ для busId
        public Drivers? Driver { get; set; }
        public int DriverId { get; set; } //Внешний ключ для driverId

        public ICollection<Tickets> ticketId { get; set; } = new List<Tickets>();
    }
}
