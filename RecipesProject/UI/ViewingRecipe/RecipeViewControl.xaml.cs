using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RecipesProject.Data;
using RecipesProject.Models;
using RecipesProject.Services;

namespace RecipesProject.UI.ViewingRecipe
{
    public partial class RecipeViewControl : UserControl
    {
        private RecipeRepository recipeRepository;
        private Recipe recipe;
        private DBContext dbContext;

        public RecipeViewControl(int id)
        {
            InitializeComponent();
            dbContext = new DBContext();
            recipeRepository = new RecipeRepository(dbContext);
            recipe = recipeRepository.GetById(id);
            loadInfoRecipe();
        }

        void loadInfoRecipe()
        {
            if (recipe != null)
            {
                //Подгрузка фото
                ViewImage.Source = ImageMethods.readImage(recipe.MainPhotoPath);

                //Подгрузка названия
                NameRecipe.Content = recipe.Title;

                IsFavoriteBtn.Content = (recipe.IsFavorite == 1) ? "Убрать из избранного" : "В избранное";

                //Подгрузка описания
                if (recipe.Description != null)
                    DescriptionRecipe.Text = recipe.Description;
                else
                    gridView.Children.Remove(DescriptionRecipe);

                //Подгрузка ингредиентов 
                if (recipe.Ingredient != null && recipe.Ingredient.Count > 0)
                {
                    foreach (var ing in recipe.Ingredient)
                        IngredientRecipe.Text += "\n• " + ing.Text;
                }
                else
                {
                    IngredientRecipe.Text += "\n—";
                }

                //Подгрузка сложности
                if (recipe.Difficulty != null)
                {
                    for (int i = 0; i < recipe.Difficulty; i++)
                        DifficultyRecipe.Content += "★";
                    int? remains = 5 - recipe.Difficulty;
                    if (remains != null && remains > 0)
                        for (int i = 0; i < remains; i++)
                            DifficultyRecipe.Content += "☆";
                }
                else
                {
                    gridView.Children.Remove(DifficultyRecipe);
                }

                //Подгрузка времени
                CookingTimeRecipe.Content += (recipe.CookingTime > 60) ?
                    $"{recipe.CookingTime / 60} ч {recipe.CookingTime % 60} мин"
                    : $"{recipe.CookingTime} мин";

                //Подгрузка количества порций
                if (recipe.Servings != null)
                    ServingsRecipe.Content += recipe.Servings.ToString();
                else
                    gridView.Children.Remove(ServingsRecipe);

                //Подгрузка шагов
                foreach (var step in recipe.Steps.OrderBy(s => s.StepNumber))
                {
                    gridView.RowDefinitions.Add(new RowDefinition());
                    var uc = new UserControl { Content = new StepReceptControl(step) };
                    gridView.Children.Add(uc);
                    Grid.SetRow(uc, gridView.RowDefinitions.Count - 1);
                }
            }
            else
            {
                NameRecipe.Content = "Рецепт не найден!";
            }
        }

        //Кнопка В избранное
        private void IsFavoriteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (recipe.IsFavorite == 0)
            {
                recipeRepository.UpdateIsFavorite(recipe.Id, 1);
                IsFavoriteBtn.Content = "Убрать из избранного";
                recipe.IsFavorite = 1;
            }
            else
            {
                recipeRepository.UpdateIsFavorite(recipe.Id, 0);
                IsFavoriteBtn.Content = "В избранное";
                recipe.IsFavorite = 0;
            }
        }

        //Кнопка Сохранить PDF
        private void SavePdfBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF файлы (*.pdf)|*.pdf",
                DefaultExt = ".pdf",
                FileName = $"{recipe.Title}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var exporter = new PdfExporter();
                    exporter.Export(recipe, dialog.FileName);
                    MessageBox.Show($"✅ PDF сохранён:\n{dialog.FileName}", "Успешно",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Ошибка сохранения PDF:\n{ex.Message}", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void Cleanup()
        {
            ViewImage.Source = null;
            foreach (var child in gridView.Children)
            {
                if (child is UserControl uc && uc.Content is StepReceptControl sc)
                    sc.Cleanup();
            }
            gridView.Children.Clear();
            gridView.RowDefinitions.Clear();
            dbContext?.Dispose();
        }
    }
}