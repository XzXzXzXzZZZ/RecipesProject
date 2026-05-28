using RecipesProject.Data;
using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject
{
    static public class AddTestData
    {
        static public void push()
        {
            using(DBContext dBContext = new DBContext())
            {
                RecipeRepository recipeRepository = new RecipeRepository(dBContext);
                StepRepository stepRepository = new StepRepository(dBContext);
                IngredientRepository ingredientRepository = new IngredientRepository(dBContext);

                if(recipeRepository.GetAll().Count() <= 0)
                {
                    recipeRepository.Add(new Recipe()
                    {
                        Title = "Борщ",
                        Description = "описание  а мук виква ив",
                        CookingTime = 100,
                    });
                    recipeRepository.Add(new Recipe()
                    {
                        Title = "Пельмени",
                        Difficulty = 3,
                        CookingTime = 30,
                        Servings = 4
                    });
                    recipeRepository.Add(new Recipe()
                    {
                        Title = "Оливье",
                        Description = "описание кпквиеареиаимвыа",
                        Difficulty = 5,
                        CookingTime = 5,
                        Servings = 4
                    });

                    ingredientRepository.Add(new Ingredient()
                    {
                        Text = "Хлеб 1 чайная ложка\n Соль 900 гр.",
                        RecipeId = 1
                    });
                    ingredientRepository.Add(new Ingredient()
                    {
                        Text = "Шоколад 1 чайная ложка\n Перец 900 гр.",
                        RecipeId = 2
                    });
                    ingredientRepository.Add(new Ingredient()
                    {
                        Text = "Молоко 1 чайная ложка\n Сахар 900 гр.",
                        RecipeId = 3
                    });

                    List<Step> Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepNumber = 1,
                        RecipeId= 1,
                        Description = "Описание какокмивашм магмиг рисгвим шы"
                    },
                    new Step()
                    {
                        StepNumber = 2,
                        RecipeId= 1,
                        Description = "Описание ifuf 2 какокмивашм магмиг рисгвим шы"
                    },
                    new Step()
                    {
                        StepNumber = 1,
                        RecipeId= 2,
                        Description = "222Описание какокмивашм магмиг рисгвим шы"
                    },
                    new Step()
                    {
                        StepNumber = 2,
                        RecipeId= 2,
                        Description = "222Описание ifuf 2 какокмивашм магмиг рисгвим шы"
                    },
                    new Step()
                    {
                        StepNumber = 3,
                        RecipeId= 2,
                        Description = "2222Описание какокмивашм магмиг рисгвим шы"
                    },
                    new Step()
                    {
                        StepNumber = 1,
                        RecipeId= 3,
                        Description = "333Описание ifuf 2 какокмивашм магмиг рисгвим шы"
                    },
                };

                    stepRepository.AddRange(Steps);
                }
                
            }
            
        }
    }
}
