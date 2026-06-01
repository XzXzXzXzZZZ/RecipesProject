using RecipesProject.Data;
using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace RecipesProject.UI.NewRecepts
{
    public static class NewAndUpdateRecipe
    {
        //-- Подготовка данных для сохранения
        public static void CreateAndSaveRecipe(string nameRecipe, string ingredients, 
            string timeCooking, List<string> stepsString, int id = 0)
        {
            try
            {
                Recipe recipe = new Recipe();

                if (!String.IsNullOrEmpty(nameRecipe))
                {
                    recipe.Title = nameRecipe;
                }

                if (!String.IsNullOrEmpty(timeCooking))
                {
                    recipe.CookingTime = CookingTimeMethods.ConvertingTimeFromStringToMinutes(timeCooking);
                }

                Ingredient ingredient = new Ingredient() { Text = ingredients, Recipe = recipe };

                List<Step> steps = new List<Step>();
                for (int i = 0; i < stepsString.Count; i++)
                {
                    Step step = new Step();

                    string description = stepsString[i];
                    Regex regex = new Regex(@"Шаг\s*(\d+)\s*:\s*(.*)");
                    Match match = regex.Match(description);
                    if (match.Success)
                    {
                        step.StepNumber = int.Parse(match.Groups[1].Value);
                        step.Description = match.Groups[2].Value;
                        step.Recipe = recipe;
                    }
                    steps.Add(step);
                }

                recipe.Id = id;
                SaveRecipe(recipe, ingredient, steps);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в CreateAndSaveRecipe {ex.Message}");
            }
            
        }

        //-- Сохранение рецепта
        private static void SaveRecipe(Recipe recipe, Ingredient ingredients, List<Step> steps)
        {
            try
            {
                using (DBContext dBContext = new DBContext())
                {
                    RecipeRepository recipeRepository = new RecipeRepository(dBContext);
                    IngredientRepository ingredientRepository = new IngredientRepository(dBContext);
                    StepRepository stepRepository = new StepRepository(dBContext);

                    if(recipe.Id != 0)
                    {
                        recipe.Ingredients = new List<Ingredient>() { ingredients };
                        recipe.Steps = steps;
                        recipe.IsFavorite = recipeRepository.GetById(recipe.Id).IsFavorite;
                        recipeRepository.Update(recipe);
                    }
                    else
                    {
                        recipeRepository.Add(recipe);
                        ingredientRepository.Add(ingredients);
                        stepRepository.AddRange(steps);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в SaveRecipe {ex.Message}");
            }
        }
    }
}
