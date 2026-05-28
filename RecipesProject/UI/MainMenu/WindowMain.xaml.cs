using Microsoft.EntityFrameworkCore;
using RecipesProject.Models;
using RecipesProject.UI.AllRecepts;   
using RecipesProject.UI.FavRecepts;   
using RecipesProject.UI.NewRecepts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();

            AddTestData.push();
            LoadAllRecipes();
            ShowMainMenuButtons();
        }

        // Метод для отображения кнопок главного меню
        private void ShowMainMenuButtons()
        {
            ButtonsPanel.Children.Clear();

            var favoriteBtn = CreateButton("❤ Любимое", "Favorite", 100);
            var newReceptBtn = CreateButton("➕ Добавить", "NewRecept", 100);
            var allReceptBtn = CreateButton("📖 Все рецепты", "AllRecepts", 120);

            favoriteBtn.Click += FavoriteBTN_Click;
            newReceptBtn.Click += NewReceptBTN_Click;
            allReceptBtn.Click += AllReceptBTN_Click;

            ButtonsPanel.Children.Add(favoriteBtn);
            ButtonsPanel.Children.Add(newReceptBtn);
            ButtonsPanel.Children.Add(allReceptBtn);
        }

        // Метод для отображения кнопок в режиме "Все рецепты"
        private void ShowAllReceptsButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Visible;
            SearchTextBox.Text = "ищите среди своих рецептов...";
            SearchTextBox.Width = 400; // Делаем поисковик меньше

            var favoriteBtn = CreateButton("❤ Любимое", "Favorite", 100);
            var newReceptBtn = CreateButton("➕ Добавить", "NewRecept", 100);
            var mainMenuBtn = CreateButton("🏠 Главный экран", "MainMenu", 140);

            favoriteBtn.Click += FavoriteBTN_Click;
            newReceptBtn.Click += NewReceptBTN_Click;
            mainMenuBtn.Click += MainMenuBTN_Click;

            ButtonsPanel.Children.Add(favoriteBtn);
            ButtonsPanel.Children.Add(newReceptBtn);
            ButtonsPanel.Children.Add(mainMenuBtn);
        }

        // Метод для отображения кнопок в режиме "Любимое"
        private void ShowFavReceptsButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Visible;
            SearchTextBox.Text = "ищите среди своих рецептов...";
            SearchTextBox.Width = 400; // Делаем поисковик меньше

            var mainMenuBtn = CreateButton("🏠 Главный экран", "MainMenu", 140);
            var newReceptBtn = CreateButton("➕ Добавить", "NewRecept", 100);
            var allReceptBtn = CreateButton("📖 Все рецепты", "AllRecepts", 120);

            mainMenuBtn.Click += MainMenuBTN_Click;
            newReceptBtn.Click += NewReceptBTN_Click;
            allReceptBtn.Click += AllReceptBTN_Click;

            ButtonsPanel.Children.Add(mainMenuBtn);
            ButtonsPanel.Children.Add(newReceptBtn);
            ButtonsPanel.Children.Add(allReceptBtn);
        }

        // Метод для отображения кнопок в режиме "Добавить рецепт"
        private void ShowNewReceptButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Collapsed; // Скрываем поиск

            var backBtn = CreateButton("🔙 Вернуться на главный экран", "Back", 220);
            backBtn.Click += MainMenuBTN_Click;

            ButtonsPanel.Children.Add(backBtn);
        }

        // Вспомогательный метод для создания кнопок
        private Button CreateButton(string content, string name, double width)
        {
            return new Button
            {
                Content = content,
                Name = name + "BTN",
                Width = width,
                Height = 40,
                Margin = new Thickness(5),
                FontSize = 14,
                Cursor = Cursors.Hand
            };
        }

        // Обработчики нажатий кнопок
        private void FavoriteBTN_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new FavReceptsControl();
            ShowFavReceptsButtons();
        }

        private void NewReceptBTN_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new NewReceptControl();
            ShowNewReceptButtons();
        }

        private void AllReceptBTN_Click(object sender, RoutedEventArgs e)
        {
            LoadAllRecipes();
            ShowAllReceptsButtons();
        }

        private void MainMenuBTN_Click(object sender, RoutedEventArgs e)
        {
            LoadAllRecipes();
            ShowMainMenuButtons();
            SearchBorder.Visibility = Visibility.Visible;
            SearchTextBox.Text = "Поиск...";
            SearchTextBox.Width = 500; // Возвращаем нормальный размер
        }
    }
}