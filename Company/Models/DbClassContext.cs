using Microsoft.EntityFrameworkCore;
using Company.Models;

namespace Company.Models
{
    public class DbClassContext : DbContext
    {
        public DbClassContext(DbContextOptions<DbClassContext> options) : base(options) { }

        public DbSet<Buses> Buses { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
        public DbSet<Passengers> Passengers { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Stops> Stops { get; set; }
        public DbSet<Flights> Flights { get; set; }
        public DbSet<Routes> Routes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация связи между Routes и Stops для города отправления
            modelBuilder.Entity<Routes>()
                .HasOne(r => r.cityDeparture) // Указываем навигационное свойство для города отправления в Routes
                .WithMany(s => s.routesDeparture) // Указываем коллекцию маршрутов для города отправления в Stops
                .HasForeignKey(r => r.DepartureStopsId) // Указываем внешний ключ в Routes
                .OnDelete(DeleteBehavior.Restrict); // Предотвращаем каскадное удаление

            // Конфигурация связи между Routes и Stops для города прибытия
            modelBuilder.Entity<Routes>()
                .HasOne(r => r.cityArrival) // Указываем навигационное свойство для города прибытия в Routes
                .WithMany(s => s.routesArrival) // Указываем коллекцию маршрутов для города прибытия в Stops
                .HasForeignKey(r => r.ArrivalStopsId) // Указываем внешний ключ в Routes
                .OnDelete(DeleteBehavior.Restrict); // Предотвращаем каскадное удаление
        }
    }
}