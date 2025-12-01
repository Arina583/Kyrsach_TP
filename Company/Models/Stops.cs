namespace Company.Models
{
    public class Stops
    {
        public int Id { get; set; }
        public string nameStop { get; set; }
        public string address { get; set; }
        public TimeOnly timeParking { get; set; }

        // Маршруты, где этот город - отправление
        public ICollection<Routes> routesDeparture { get; set; } = new List<Routes>();
        // Маршруты, где этот город - прибытие
        public ICollection<Routes> routesArrival { get; set; } = new List<Routes>();
        public ICollection<Tickets> ticketId { get; set; } = new List<Tickets>();
    }
}
