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

        // Три кнопки меню после выбора рецепта (снизу)
        private void ShowViewReceptButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Collapsed;

            var backBtn = CreateButton("Вернуться на главный экран", "Back", 220);
            backBtn.Click += MainMenuBTN_Click;

            var updateBtn = CreateButton("Изменить рецепт", "Update", 220);
            updateBtn.Click += UpdateBTN_Click;

            var deleteBtn = CreateButton("Удалить рецепт", "Delete", 220);
            deleteBtn.Click += DeleteRecipeBtn_Click;

            ButtonsPanel.Children.Add(backBtn);
            ButtonsPanel.Children.Add(updateBtn);
            ButtonsPanel.Children.Add(deleteBtn);
        }

        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (idRecipe != null)
            {
                //очищение старого контрола
                if (MainContentControl.Content is RecipeViewControl oldView)
                {
                    oldView.Cleanup();
                    //сборщк мусора
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

        //Обработчик удаления рецепта
        private void DeleteRecipeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (idRecipe != null)
            {
                // получение названия рецепта для последующего его вывода в консоли
                string recipeTitle = "";
                using (DBContext dBContext = new DBContext())
                {
                    RecipeRepository recipeRepository = new RecipeRepository(dBContext);
                    var recipe = recipeRepository.GetById((int)idRecipe);
                    if (recipe != null)
                    {
                        recipeTitle = recipe.Title;
                    }
                }

                if (MessageBox.Show($"Вы уверены, что хотите удалить рецепт \"{recipeTitle}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (DBContext dBContext = new DBContext())
                        {
                            RecipeRepository recipeRepository = new RecipeRepository(dBContext);
                            recipeRepository.Delete((int)idRecipe);
                        }
                        // Возвращение к глав экрану
                        loadMainMenu();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}