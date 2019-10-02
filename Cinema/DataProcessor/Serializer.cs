namespace Cinema.DataProcessor
{
    using System;
    using System.Linq;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Text;
    using System.IO;

    public class Serializer
    {

        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Projections.Any(p => p.Tickets.Any()) && m.Rating >= rating)
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new JsonExportTopMovies
                {
                    Name = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    Income = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = m.Projections.SelectMany(p => p.Tickets).Select(t => new JsonExportCustomer
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = t.Customer.Balance.ToString("F2")
                    })
                    .OrderByDescending(c => c.Balance)
                    .ThenBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToList()
                })     
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Select(c => new XmlExportCustomer
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromSeconds(c.Tickets.Sum(t => t.Projection.Movie.Duration.TotalSeconds)).ToString("hh\\:mm\\:ss", CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToList();

            var ser = new XmlSerializer(typeof(List<XmlExportCustomer>), new XmlRootAttribute("Customers"));

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            var sb = new StringBuilder();

            ser.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().Trim();
        }
    }
}