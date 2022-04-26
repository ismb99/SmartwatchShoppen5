using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public double Price { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        //public int CategoryId { get; set; }
        //[ForeignKey("CategoryId")]
        //[ValidateNever]
        //public Category Category { get; set; }
        //public int WristbandId { get; set; }
        //[ForeignKey("WristbandId")]
        //[ValidateNever]
        //public Wristband Wristband { get; set; }
    }
}
