using Microsoft.EntityFrameworkCore;
using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RecipesProject.Data
{
    public class RecipeRepository
    {
        private readonly DBContext _context;

        public RecipeRepository(DBContext context)
        {
            _context = context;
        }

        public List<Recipe> GetAll()
        {
            return _context.Recipes
                .Include(r => r.Steps)
                .Include(r => r.Ingredients)
                .ToList();
        }

        public Recipe GetById(int id)
        {
            return _context.Recipes
                .Include(r => r.Steps)
                .Include(r => r.Ingredients)
                .FirstOrDefault(r => r.Id == id);
        }

        public void Add(Recipe recipe)
        {
            recipe.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
        }

        public void Update(Recipe updatedRecipe)
        {
            var existing = _context.Recipes
                .Include(r => r.Steps)
                .Include(r => r.Ingredients)
                .FirstOrDefault(r => r.Id == updatedRecipe.Id);

            if (existing != null)
            {
                // Обновляем ВСЕ поля кроме Id
                existing.Title = updatedRecipe.Title;
                existing.Description = updatedRecipe.Description;
                existing.CookingTime = updatedRecipe.CookingTime;
                existing.Difficulty = updatedRecipe.Difficulty;
                existing.Servings = updatedRecipe.Servings;
                existing.IsFavorite = updatedRecipe.IsFavorite;
                existing.MainPhotoPath = updatedRecipe.MainPhotoPath;
                existing.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Обновляем связанные коллекции
                _context.Steps.RemoveRange(existing.Steps);
                existing.Steps = updatedRecipe.Steps;

                _context.Ingredients.RemoveRange(existing.Ingredients);
                existing.Ingredients = updatedRecipe.Ingredients;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
            }
        }

        public List<Recipe> GetFavorites()
        {
            return _context.Recipes
                .Where(r => r.IsFavorite == 1)
                .Include(r => r.Steps)
                .Include(r => r.Ingredients)
                .ToList();
        }

        public List<Recipe> SearchByTitle(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Recipe>();

            return _context.Recipes
                .Where(r => r.Title.Contains(query))
                .Include(r => r.Steps)
                .Include(r => r.Ingredients)
                .ToList();
        }
    }
}
