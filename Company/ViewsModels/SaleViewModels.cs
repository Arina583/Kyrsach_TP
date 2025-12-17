using Company.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Company.ViewsModels
{
    public class SaleViewModels
    {
        public Passengers Passenger { get; set; } = new Passengers();
        public Flights Flight { get; set; } = new Flights();
        public IEnumerable<SelectListItem> AvailableFlights { get; set; } = Enumerable.Empty<SelectListItem>();

    }
}
