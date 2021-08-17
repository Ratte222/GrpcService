using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Model
{
    public class OrderLine : BaseEntity<long>
    {
        public long ProductId {  get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product {  get; set; }
        public decimal Quantity { get; set; }
    }
}
