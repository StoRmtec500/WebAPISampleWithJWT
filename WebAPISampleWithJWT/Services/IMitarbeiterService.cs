using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPISampleWithJWT.Models;

namespace WebAPISampleWithJWT.Services
{
    public interface IMitarbeiterService
    {
        Mitarbeiter Authenticate(string username, string password);
        Mitarbeiter RefreshAccessToken(string token);
        IEnumerable<Mitarbeiter> GetAll();
    }
}
