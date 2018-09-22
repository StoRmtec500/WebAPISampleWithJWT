using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPISampleWithJWT.Resource
{
    public class TokenResource
    {
        public string Token { get; set; }
        public string refreshToken { get; set; }
    }
}
