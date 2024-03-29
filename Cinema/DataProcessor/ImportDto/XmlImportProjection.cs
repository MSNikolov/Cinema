﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class XmlImportProjection
    {
        [XmlElement("MovieId")]
        [Required]
        public int MovieId { get; set; }

        [XmlElement("HallId")]
        [Required]
        public int HallId { get; set; }

        [XmlElement("DateTime")]
        [Required]
        public string TimeOfProj { get; set; }
    }
}
