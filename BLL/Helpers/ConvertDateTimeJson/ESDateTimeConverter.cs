using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Helpers.ConvertDateTimeJson
{
    public class ESDateTimeConverter : IsoDateTimeConverter
    {
        public ESDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        }
    }
}
