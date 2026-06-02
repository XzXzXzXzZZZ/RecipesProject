using RecipesProject.Data;
using RecipesProject.UI.AllRecepts;
using RecipesProject.UI.NewRecepts;
using RecipesProject.UI.ViewingRecipe;
using System.Windows;
using RecipesProject.Models;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        private int? idRecipe = null;

        // Загрузка всех рецептов в главное окно
        private void LoadAllRecipes()
        {
            try
            {
                var allReceptsControl = new AllReceptsControl();
                allReceptsControl.RecipeSelected += AllReceptsControl_RecipeSelected;
                MainContentControl.Content = allReceptsControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки рецептов: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // выбора рецепта
        private void AllReceptsControl_RecipeSelected(object sender, int recipeId)
        {
            MainContentControl.Content = new RecipeViewControl(recipeId);
            idRecipe = recipeId;
            ShowViewReceptButtons();
        }

        //-- Кнопки после выбора рецепта
        private void ShowViewReceptButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Collapsed;

            var backBtn = CreateButton("Вернуться на главный экран", "Back", 220);
            backBtn.Click += MainMenuBTN_Click;
            var updateBtn = CreateButton("Изменить рецепт", "Update", 220);
            updateBtn.Click += UpdateBTN_Click;

            ButtonsPanel.Children.Add(backBtn);
            ButtonsPanel.Children.Add(updateBtn);
        }

        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (idRecipe != null)
            {
                //-- Очищаем старый контрол и освобождаем ресурсы
                if (MainContentControl.Content is RecipeViewControl oldView)
                {
                    oldView.Cleanup();
                                                     
                    //-- Вызываем сборщик мусора принудительно
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                using (DBContext dBContext = new DBContext())
                {
                    RecipeRepository recipeRepository = new RecipeRepository(dBContext);
                    var recipe = recipeRepository.GetById((int)idRecipe);
                    MainContentControl.Content = new NewReceptControl(recipe);
                    ShowNewReceptButtons();
                }
            }
        }

    }
}