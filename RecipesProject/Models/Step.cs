using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models
{
    public class Step
    {
        public int Id { get; set; }
        public int RecipeId {  get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? PhotoPath {  get; set; }

        public Recipe Recipe { get; set; }
    }
}
