using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Ticket")]
    public class XmlImportTicket
    {
        [Required]
        [Range(0.01, double.MaxValue)]

        [XmlElement("Price")]
        public decimal Price { get; set; }

        [Required]
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }
    }
}