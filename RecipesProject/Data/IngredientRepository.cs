using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Data
{
    public class IngredientRepository
    {
        private readonly DBContext _context;

        public IngredientRepository(DBContext context)
        {
            _context = context;
        }

        public void Add(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
        }

        public void AddRange(List<Ingredient> ingredients)
        {
            _context.Ingredients.AddRange(ingredients);
            _context.SaveChanges();
        }
    }
}
