using RecipesProject.Data;
using RecipesProject.Models;
using System.Windows;

namespace RecipesProject.UI.NewRecepts
{
    public static class NewAndUpdateRecipe
    {
        //-- Сохранение
        public static void SaveRecipe(Recipe recipe)
        {
            try
            {
                if(recipe != null)
                {
                    using (DBContext dBContext = new DBContext())
                    {
                        RecipeRepository recipeRepository = new RecipeRepository(dBContext);

                        if (recipe.Id != 0)
                        {
                            recipeRepository.Update(recipe);
                        }
                        else
                        {
                            recipeRepository.Add(recipe);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var inner = ex.InnerException?.Message ?? "нет";

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += "\n→ " + ex.Message;
                }

                MessageBox.Show($"Ошибка сохранения:\n{message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
