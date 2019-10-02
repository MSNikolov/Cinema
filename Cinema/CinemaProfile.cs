using AutoMapper;
using Cinema.Data.Models;
using Cinema.DataProcessor.ExportDto;
using Cinema.DataProcessor.ImportDto;

namespace Cinema
{
    public class CinemaProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public CinemaProfile()
        {
            this.CreateMap<JsonImportMovie, Movie>();

            this.CreateMap<JsonImportHall, Hall>();

            this.CreateMap<XmlImportProjection, Projection>();

            this.CreateMap<XmlImportTicket, Ticket>();

            this.CreateMap<XmlImportCustomer, Customer>();

            this.CreateMap<Customer, JsonExportCustomer>();

            this.CreateMap<Movie, JsonExportTopMovies>();

            this.CreateMap<Customer, XmlExportCustomer>();
        }
    }
}
