using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Formula
{
    public abstract class BaseModel
    {
        [NotMapped]
        public string _state { get; set; }
    }
}
