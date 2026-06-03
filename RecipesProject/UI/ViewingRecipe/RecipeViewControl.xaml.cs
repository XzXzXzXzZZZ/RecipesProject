using Microsoft.EntityFrameworkCore;
using RecipesProject.Data;
using RecipesProject.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace RecipesProject.UI.ViewingRecipe
{
    public partial class RecipeViewControl : UserControl
    {
        private RecipeRepository recipeRepository;
        private Recipe recipe;
        public RecipeViewControl(int id)
        {
            InitializeComponent();
            DBContext dbContext = new DBContext();
            recipeRepository = new RecipeRepository(dbContext);
            recipe = recipeRepository.GetById(id);
            loadInfoRecipe();
        }
        // метод для Цдаления рецептов (предупреждение)
        private void DeleteRecipeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (recipe != null)
            {
                if (MessageBox.Show($"Вы уверены, что хотите удалить рецепт \"{recipe.Title}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        recipeRepository.Delete(recipe.Id);
                        MessageBox.Show($"Рецепт \"{recipe.Title}\" успешно удалён!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Возвращаемся на главный экран
                        var windowMain = Application.Current.MainWindow as MainMenu.WindowMain;
                        if (windowMain != null)
                        {
                            windowMain.loadMainMenu();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        void loadInfoRecipe()
        {

            if (recipe != null)
            {
                //-- Подгрузка фото
                ViewImage.Source = ImageMethods.readImage(recipe.MainPhotoPath);


                //-- Подгрузка названия
                NameRecipe.Content = recipe.Title;

                IsFavoriteBtn.Content = (recipe.IsFavorite == 1)? "Убрать из избранного" : "В избранное";

                //-- Подгрузка описания
                if (recipe.Description != null)
                {
                    DescriptionRecipe.Text = recipe.Description;
                }
                else
                {
                    gridView.Children.Remove(DescriptionRecipe);
                }

                //-- Подгрузка ингридиентов
                IngredientRecipe.Text += "\n" + recipe.Ingredient.Text;

                //-- Подгрузка сложности
                if (recipe.Difficulty != null)
                {
                    for (int i = 0; i < recipe.Difficulty; i++)
                    {
                        DifficultyRecipe.Content += "★";
                    }
                    int? remains = 5 - recipe.Difficulty;
                    if (remains != null && remains > 0)
                    {
                        for (int i = 0; i < remains; i++)
                        {
                            DifficultyRecipe.Content += "☆";
                        }
                    }
                }
                else
                {
                    gridView.Children.Remove(DifficultyRecipe);
                }

                //-- Подгрузка времени
                CookingTimeRecipe.Content += (recipe.CookingTime > 60) ? 
                    $"{recipe.CookingTime / 60} ч {recipe.CookingTime % 60} мин" 
                    : $"{recipe.CookingTime} мин";

                //-- Подгрузка количества порций
                if (recipe.Servings != null)
                {
                    ServingsRecipe.Content += recipe.Servings.ToString();
                }
                else
                {
                    gridView.Children.Remove(ServingsRecipe);
                }

                //-- Подгрузка шагов через контроллер
                foreach (var step in recipe.Steps)
                {
                    //-- Создаем строку внизу страницы
                    gridView.RowDefinitions.Add(new RowDefinition());

                    UserControl userControl = new UserControl();
                    userControl.Content = new StepReceptControl(step);
                    gridView.Children.Add(userControl);

                    Grid.SetRow(userControl, gridView.RowDefinitions.Count - 1);
                }
            }
            else
            {
                NameRecipe.Content = "Рецепт не найден!";
            }
        }

        //-- Обработчик клика на кнопку "В избранное"
        private void IsFavoriteBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                if(recipe.IsFavorite == 0)
                {
                    recipeRepository.UpdateIsFavorite(recipe.Id, 1);
                    btn.Content = "Убрать из избранного";
                }
                else
                {
                    recipeRepository.UpdateIsFavorite(recipe.Id, 0);
                    btn.Content = "В избранное";
                }
            }
        }

        public void Cleanup()
        {
            // Освобождаем главное фото
            ViewImage.Source = null;

            // Очищаем фото в шагах
            foreach (var child in gridView.Children)
            {
                if (child is UserControl userControl && userControl.Content is StepReceptControl stepControl)
                {
                    stepControl.Cleanup();
                }
            }

            // Очищаем все дочерние элементы грида
            gridView.Children.Clear();
            gridView.RowDefinitions.Clear();
        }
    }
}
