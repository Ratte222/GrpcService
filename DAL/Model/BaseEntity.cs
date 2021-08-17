using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Model
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}
