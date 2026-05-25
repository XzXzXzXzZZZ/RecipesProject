using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CookingTime {  get; set; }
        public int? Difficulty {  get; set; }
        public int? Servings {  get; set; }
        public int? IsFavorite {  get; set; }
        public string? MainPhotoPath {  get; set; }
        public string? CreatedAt {  get; set; }
        public string? UpdatedAt {  get; set; }

        public ICollection<Step> Steps = new List<Step>();
        public ICollection<Ingredient> Ingredients = new List<Ingredient>();
    }
}
