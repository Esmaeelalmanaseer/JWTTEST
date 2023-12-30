using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTEST.Models
{
    public class ApplictionDbContext:IdentityDbContext<ApplictionUser>
    {
        public ApplictionDbContext(DbContextOptions<ApplictionDbContext>option):base(option)
        {

        }
    }
}
