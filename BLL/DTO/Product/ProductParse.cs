using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Product
{
    public class ProductParse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description {
            get
            {
                int start = NoParseDescription.IndexOf("Экран");
                return NoParseDescription.Substring(start);
            }
        }
        public string NoParseDescription { get; set; }
        public string Cost { get; set; }
        public string Currency {  get; set; }

        //public ProductParse(string content)
        //{
        //    int endName = content.IndexOf("отзыв") - 4;
        //    Name = content.Substring(0, endName);
        //    int startCost = endName + 7, endCost = content.IndexOf('₴');
        //}
    }
}
