using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Helpers
{
    public class AppSettings
    {
        public string Issuer { get; set; }    
        public string Audience { get; set; }  
        public int Lifetime { get; set; }     
        public string Secret { get; set; }    
    }
}
