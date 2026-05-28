using RecipesProject.UI.AllRecepts;
using RecipesProject.UI.FavRecepts;
using RecipesProject.UI.ViewingRecipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        private void LoadAllRecipes()
        {
            //-- Создаем контрол для списка рецептов
            var allReceptsControl = new AllReceptsControl();
            //-- Подписываемся на событие выбора рецепта
            allReceptsControl.RecipeSelected += OnRecipeSelected;
            //-- Добавляем контент контроллера
            MainContentControl.Content = allReceptsControl;
        }

        //-- Метод для отображения кнопок в режиме "Просмотр рецепта"
        private void ShowViewReceptButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Collapsed; // Скрываем поиск

            var backBtn = CreateButton("🔙 Вернуться на главный экран", "Back", 220);
            backBtn.Click += MainMenuBTN_Click;
            var updateBtn = CreateButton("Изменить рецепт", "Update", 220);
            updateBtn.Click += UpdateBTN_Click;

            ButtonsPanel.Children.Add(backBtn);
            ButtonsPanel.Children.Add(updateBtn);
        }

        //-- Обработчик кнопки обновления
        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы нажали на 'Изменить рецепт'. Эта кнопка должна открывать экран добавления рецепта, но с вбитыми данными");
        }

        //-- Обработчик события
        private void OnRecipeSelected(object sender, int selectedRecipeId)
        {
            MainContentControl.Content = new RecipeViewControl(selectedRecipeId);
            ShowViewReceptButtons();
        }

    }
}
