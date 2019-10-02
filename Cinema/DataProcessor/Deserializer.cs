namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Cinema.Data.Models;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDto = JsonConvert.DeserializeObject<List<JsonImportMovie>>(jsonString);

            var movies = new List<Movie>();

            var result = new StringBuilder();

            foreach (var mov in moviesDto)
            {
                var movieExists = context.Movies.Any(m => m.Title == mov.Title);
                var isValidEnum = Enum.TryParse(typeof(Genre), mov.Genre, out object genre);
                var isValidDto = IsValid(mov);

                if (movieExists || !isValidEnum || !isValidDto)
                {
                    result.AppendLine(ErrorMessage);
                }

                else
                {
                    var movie = Mapper.Map<Movie>(mov);

                    movies.Add(movie);

                    result.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("F2")));
                }
            }

            context.Movies.AddRange(movies);

            context.SaveChanges();

            return result.ToString().Trim();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsDto = JsonConvert.DeserializeObject<List<JsonImportHall>>(jsonString);

            var result = new StringBuilder();

            var halls = new List<Hall>();

            foreach (var hallDto in hallsDto)
            {
                if (!IsValid(hallDto))
                {
                    result.AppendLine(ErrorMessage);
                }

                else
                {
                    var hall = Mapper.Map<Hall>(hallDto);

                    var projType = "";

                    if (hall.Is4Dx)
                    {
                        projType += "4Dx";

                        if (hall.Is3D)
                        {
                            projType += "/3D";
                        }

                    }

                    else if (hall.Is3D)
                    {
                        projType = "3D";
                    }

                    else
                    {
                        projType = "Normal";
                    }

                    result.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, projType, hallDto.Places));

                    for (int i = 0; i < hallDto.Places; i++)
                    {
                        hall.Seats.Add(new Seat());
                    }

                    halls.Add(hall);
                }
            }

            context.Halls.AddRange(halls);

            context.SaveChanges();

            return result.ToString().Trim();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var ser = new XmlSerializer(typeof(List<XmlImportProjection>), new XmlRootAttribute("Projections"));

            var projectionsDto = (List<XmlImportProjection>)ser.Deserialize(new StringReader(xmlString));

            var projections = new List<Projection>();

            var result = new StringBuilder();

            foreach (var proj in projectionsDto)
            {
                if (!IsValid(proj) || !context.Movies.Any(m => m.Id == proj.MovieId) || !context.Halls.Any(h => h.Id == proj.HallId))
                {
                    result.AppendLine(ErrorMessage);
                }

                else
                {
                    var projection = Mapper.Map<Projection>(proj);

                    projection.DateTime = DateTime.ParseExact(proj.TimeOfProj, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    projections.Add(projection);

                    var movieTitle = context.Movies
                        .Find(projection.MovieId)
                        .Title;

                    result.AppendLine(string.Format(SuccessfulImportProjection, movieTitle, projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
                }
            }
             
            context.Projections.AddRange(projections);

            context.SaveChanges();

            return result.ToString().Trim();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var ser = new XmlSerializer(typeof(List<XmlImportCustomer>), new XmlRootAttribute("Customers"));

            var customersDto = (List<XmlImportCustomer>)ser.Deserialize(new StringReader(xmlString));

            var result = new StringBuilder();

            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                if (!IsValid(customerDto))
                {
                    result.AppendLine(ErrorMessage);
                }

                else
                {
                    var customer = Mapper.Map<Customer>(customerDto);

                    foreach (var tick in customerDto.Talons)
                    {
                        if (IsValid(tick) && context.Projections.Any(p => p.Id == tick.ProjectionId))
                        {
                            customer.Tickets.Add(Mapper.Map<Ticket>(tick));
                        }
                    }

                    result.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));

                    customers.Add(customer);
                }
            }

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return result.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
} 