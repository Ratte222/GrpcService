using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BLL.DTO.Product
{
    public class CreateProductDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(0, Double.MaxValue)]
        public int Cost { get; set; }
    }
}
