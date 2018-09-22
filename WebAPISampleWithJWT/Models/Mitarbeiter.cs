using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPISampleWithJWT.Models
{
    public class Mitarbeiter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Passwort { get; set; }
        public int LoginErlaubt { get; set; }
    }
}
