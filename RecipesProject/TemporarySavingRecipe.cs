using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipesProject
{
    //-- Временное хранение рецепта для перемещениями между контроллерами
    public static class TemporarySavingRecipe
    {
        public static int Id { get; set; } = 0;
        public static string? Title { get; set; }
        public static string? IngredientsText { get; set; }
        public static List<string>? Steps {  get; set; }
        public static string? CookingTime { get; set; }

        public static void Clear()
        {
            Id = 0;
            Title = "";
            IngredientsText = "";
            CookingTime = "";
            Steps = null;
        }
        
    }
}
