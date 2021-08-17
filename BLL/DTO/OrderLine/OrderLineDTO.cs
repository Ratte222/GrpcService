using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.OrderLine
{
    public class OrderLineDTO
    {
        public long Id { get; set; }
        public string Product { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DateTimeRequest {  get; set; }
    }
}
