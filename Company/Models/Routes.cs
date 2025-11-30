namespace Company.Models
{
    public class Routes
    {
        public int id { get; set; }
        public string intermediateStops { get; set; }
        public float light { get; set; }

        // Навигационное свойство для города отправления
        public Stops? cityDeparture { get; set; }
        // Внешний ключ, указывающий на город отправления
        public int DepartureStopsId { get; set; }

        // Навигационное свойство для города прибытия
        public Stops? cityArrival { get; set; }
        // Внешний ключ, указывающий на город прибытия
        public int ArrivalStopsId { get; set; }

        // Коллекция рейсов, связанных с маршрутом
        public ICollection<Flights> flightsId { get; set; } = new List<Flights>();
    }
}
