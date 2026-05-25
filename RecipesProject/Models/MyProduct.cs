using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models
{
    public class MyProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? IsPermanent {  get; set; }
    }
}
