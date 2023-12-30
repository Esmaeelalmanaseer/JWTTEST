using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTEST.Models
{
    public class AuthModel
    {
        public string Massege { get; set; }
        public bool IsAuthenticated { get; set; }
        public string  UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
