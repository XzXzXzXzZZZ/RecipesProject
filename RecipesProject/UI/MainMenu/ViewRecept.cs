using RecipesProject.UI.AllRecepts;
using RecipesProject.UI.ViewingRecipe;
using System.Windows;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        private void LoadAllRecipes()
        {
            var allReceptsControl = new AllReceptsControl();
            allReceptsControl.RecipeSelected += OnRecipeSelected;
            MainContentControl.Content = allReceptsControl;
        }

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
            MessageBox.Show("Вы нажали на 'Изменить рецепт'. Эта кнопка должна открывать экран добавления рецепта, но с введенными данными");
        }

        private void OnRecipeSelected(object sender, int selectedRecipeId)
        {
            MainContentControl.Content = new RecipeViewControl(selectedRecipeId);
            ShowViewReceptButtons();
        }
    }
}