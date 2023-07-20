using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class PromoCode
    {
        
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Discount { get; set; }
    }
}
