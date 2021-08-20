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
        public string PathTemp { get; set; }
        
        public string PathSaveProductPhoto {
            get
            {
                return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                    PathTemp);
            }
        }
        public int PathSliceDepth { get; set; }
    }
}
