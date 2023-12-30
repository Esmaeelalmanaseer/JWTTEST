using JWTTEST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTEST.service
{
   public interface IAuthService
    {
        Task<AuthModel> RegisterAsynce(RegstirModel model);
        Task<AuthModel> GetTokenAsynce(TokenReguestModel model);
    }
}
