﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? StreetAdress { get; set; }
        public string? PostTown { get; set; }
    }
}
