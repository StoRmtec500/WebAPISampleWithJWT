using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPISampleWithJWT.Models;
using WebAPISampleWithJWT.Resource;

namespace WebAPISampleWithJWT.Services
{
    public class MitarbeiterService : IMitarbeiterService
    {
        private MitarbeiterContext _context;
        private IConfiguration _config;

        public MitarbeiterService(MitarbeiterContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public Mitarbeiter Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var mitarbeiter = _context.Mitarbeiter.SingleOrDefault(x => x.Name == username && x.Passwort == password);

            if (mitarbeiter == null)
                return null;

            return mitarbeiter;

        }


        public IEnumerable<Mitarbeiter> GetAll()
        {
            return _context.Mitarbeiter;
        }

        public Mitarbeiter RefreshAccessToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
