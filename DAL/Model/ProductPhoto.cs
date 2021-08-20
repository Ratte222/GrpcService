using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Model
{
    public class ProductPhoto : BaseEntity<long>
    {
        [Required]
        public DateTime DateTime { get; set; }
        public long? UserId { get; set; }
        [Required]
        public string Name { get; set; }
        //[Required]
        public long Size { get; set; }
        [StringLength(127 + 1 + 127)]//prefix + "/" + suffix
        public string MimeType { get; set; }
        public Guid LocalId { get; set; }
        public bool IsPublic { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }

        public ProductPhoto() : this(0) { }

        public ProductPhoto(long id)
        {
            Id = id;
            LocalId = Guid.NewGuid();
        }
    }
}
