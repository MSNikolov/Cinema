using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data.Models
{
    public class Projection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }

        [Required]
        public int HallId { get; set; }

        [ForeignKey(nameof(HallId))]
        public Hall Hall { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public HashSet<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}