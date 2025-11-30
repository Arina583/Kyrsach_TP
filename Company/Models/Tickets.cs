namespace Company.Models
{
    public class Tickets
    {
        public int id { get; set; }
        public int numberSeat { get; set; }
        public double pice { get; set; }
        public string status { get; set; }

        //связи между моделями
        public Flights? Flight { get; set; }
        public int FlightId { get; set; } //Внешний ключ
        public Passengers? Passenger { get; set; }
        public int PassengerId { get; set; } //Внешний ключ
        public Stops? Stop { get; set; }
        public int StopId { get; set; } //Внешний ключ
    }
}
