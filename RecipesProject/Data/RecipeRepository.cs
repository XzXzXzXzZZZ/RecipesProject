using Microsoft.EntityFrameworkCore;
using RecipesProject.Models;
using System.IO;
using System.Windows;


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

            if(recipe.MainPhotoPath != null)
            {
                recipe.MainPhotoPath = AddImage(recipe.MainPhotoPath);
            }

            if(recipe.Steps != null)
            {
                List<Step> steps = new List<Step>();
                foreach(Step step in recipe.Steps)
                {
                    if (!String.IsNullOrEmpty(step.PhotoPath))
                        step.PhotoPath = AddImage(step.PhotoPath);
                    steps.Add(step);
                }
                recipe.Steps = steps;
            }
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
                //-- Обработка изображения
                if (!string.IsNullOrEmpty(updatedRecipe.MainPhotoPath)
                    && updatedRecipe.MainPhotoPath != existing.MainPhotoPath)
                {
                    DeleteImage(existing.MainPhotoPath);
                    updatedRecipe.MainPhotoPath = AddImage(updatedRecipe.MainPhotoPath);
                }

                if (existing.Steps != null)
                {
                    foreach (Step step in existing.Steps)
                    {
                        if (!String.IsNullOrEmpty(step.PhotoPath))
                            DeleteImage(step.PhotoPath);
                    }
                }

                if (updatedRecipe.Steps != null)
                {
                    List<Step> steps = new List<Step>();
                    foreach (Step step in updatedRecipe.Steps)
                    {
                        if(!String.IsNullOrEmpty(step.PhotoPath))
                            step.PhotoPath = AddImage(step.PhotoPath);
                        steps.Add(step);
                    }
                    updatedRecipe.Steps = steps;
                }

                // Обновляем ВСЕ поля кроме Id
                existing.Title = updatedRecipe.Title;
                existing.Description = updatedRecipe.Description;
                existing.CookingTime = updatedRecipe.CookingTime;
                existing.Difficulty = (updatedRecipe.Difficulty != null)? updatedRecipe.Difficulty : 0;
                existing.Servings = (updatedRecipe.Servings != null) ? updatedRecipe.Servings : 4;
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

        public void UpdateIsFavorite(int id, int isFavorite)
        {
            var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);
            if (recipe != null)
            {
                recipe.IsFavorite = isFavorite;
                recipe.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                if(!String.IsNullOrEmpty(recipe.MainPhotoPath))
                    DeleteImage(recipe.MainPhotoPath);
                if (recipe.Steps != null)
                {
                    foreach (Step step in recipe.Steps)
                    {
                        if (!String.IsNullOrEmpty(step.PhotoPath))
                            DeleteImage(step.PhotoPath);
                    }
                }

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


        public List<Recipe> FindByIngredients(List<string> fridgeProducts)
        {
            var recipes = GetAll();
            List<Recipe> result = new List<Recipe>();
            if (recipes != null)
            {
                var fridgeLower = fridgeProducts.Select(p => p.ToLower()).ToList();

                foreach (var recipe in recipes)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        var ingredientLower = ingredient.Text.ToLower();
                        if (fridgeLower.Any(fridgeProduct => ingredientLower.Contains(fridgeProduct)))
                        {
                            result.Add(recipe);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private void DeleteImage(string PhotoPath)
        {
            //-- Удаляем старый файл, если он существует
            if (!string.IsNullOrEmpty(PhotoPath) && File.Exists(PhotoPath))
            {
                try
                {
                    //-- Ждём, пока файл освободится (максимум 1 секунду)
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            File.Delete(PhotoPath);
                            break;
                        }
                        catch (IOException)
                        {
                            System.Threading.Thread.Sleep(100); // Ждём 100 мс
                        }
                    }
                }
                catch (Exception)
                {
                    //-- Если не удалось удалить — просто пропускаем (пока)
                }
            }
        }

        private string AddImage(string PhotoPath)
        {
            //-- Сохраняем новый файл в папку проекта
            string projectFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(projectFolder))
                Directory.CreateDirectory(projectFolder);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(PhotoPath)}";
            string newPath = Path.Combine(projectFolder, fileName);

            //-- Копируем через поток — файл не будет заблокирован
            using (var sourceStream = new FileStream(PhotoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var destStream = new FileStream(newPath, FileMode.Create))
            {
                sourceStream.CopyTo(destStream);
            }

            return newPath;
        }
    }
}
