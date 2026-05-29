using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RecipesProject.Models;
using RecipesProject.Data;
using RecipesProject.UI.AllRecepts;
using RecipesProject.UI.FavRecepts;
using RecipesProject.UI.NewRecepts;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        private RecipeRepository _repository;
        private DBContext _dbContext;

        public WindowMain()
        {
            InitializeComponent();

            _dbContext = new DBContext();
            _dbContext.Database.EnsureCreated();
            _repository = new RecipeRepository(_dbContext);

            LoadAllRecipes();
            ShowMainMenuButtons();
        }

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
            var recipe = _repository.GetById(recipeId);
            if (recipe != null)
            {
                MessageBox.Show($"📖 {recipe.Title}\n\n" +
                               $"{recipe.Description}\n\n" +
                               $"⏱️ Время: {recipe.CookingTime} мин\n" +
                               $"👥 Порций: {recipe.Servings}\n" +
                               $"📊 Сложность: {recipe.Difficulty}/5\n" +
                               $"⭐ Избранное: {(recipe.IsFavorite == 1 ? "Да" : "Нет")}",
                               "Просмотр рецепта",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
        }

        // Глав меню
        private void ShowMainMenuButtons()
        {
            ButtonsPanel.Children.Clear();

            var favoriteBtn = CreateButton("❤ Любимое", "Favorite", 100);
            var newReceptBtn = CreateButton("➕", "NewRecept", 100);
            var allReceptBtn = CreateButton("📖 Все рецепты", "AllRecepts", 120);

            favoriteBtn.Click += FavoriteBTN_Click;
            newReceptBtn.Click += NewReceptBTN_Click;
            allReceptBtn.Click += AllReceptBTN_Click;

            ButtonsPanel.Children.Add(favoriteBtn);
            ButtonsPanel.Children.Add(newReceptBtn);
            ButtonsPanel.Children.Add(allReceptBtn);
        }

        // раздел все рецепты
        private void ShowAllReceptsButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Visible;
            SearchTextBox.Text = "ищите среди своих рецептов...";
            SearchTextBox.Width = 400;

            var favoriteBtn = CreateButton("❤ Любимое", "Favorite", 100);
            var newReceptBtn = CreateButton("➕", "NewRecept", 100);
            var mainMenuBtn = CreateButton("🏠 Главный экран", "MainMenu", 140);

            favoriteBtn.Click += FavoriteBTN_Click;
            newReceptBtn.Click += NewReceptBTN_Click;
            mainMenuBtn.Click += MainMenuBTN_Click;

            ButtonsPanel.Children.Add(favoriteBtn);
            ButtonsPanel.Children.Add(newReceptBtn);
            ButtonsPanel.Children.Add(mainMenuBtn);
        }

        //раздел любимый
        private void ShowFavReceptsButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Visible;
            SearchTextBox.Text = "ищите среди своих рецептов...";
            SearchTextBox.Width = 400;

            var mainMenuBtn = CreateButton("🏠 Главный экран", "MainMenu", 140);
            var newReceptBtn = CreateButton("➕", "NewRecept", 100);
            var allReceptBtn = CreateButton("📖 Все рецепты", "AllRecepts", 120);

            mainMenuBtn.Click += MainMenuBTN_Click;
            newReceptBtn.Click += NewReceptBTN_Click;
            allReceptBtn.Click += AllReceptBTN_Click;

            ButtonsPanel.Children.Add(mainMenuBtn);
            ButtonsPanel.Children.Add(newReceptBtn);
            ButtonsPanel.Children.Add(allReceptBtn);
        }

        // раздел адд рецепт
        private void ShowNewReceptButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Collapsed;

            var backBtn = CreateButton("Вернуться на главный экран", "Back", 220);
            backBtn.Click += MainMenuBTN_Click;

            ButtonsPanel.Children.Add(backBtn);
        }

        private Button CreateButton(string content, string name, double width, Brush background = null)
        {
            return new Button
            {
                Content = content,
                Name = name + "BTN",
                Width = width,
                Height = 40,
                Margin = new Thickness(5),
                FontSize = 14,
                Cursor = Cursors.Hand,
                Background = background ?? Brushes.IndianRed,
                Foreground = Brushes.White
            };
        }

        private void FavoriteBTN_Click(object sender, RoutedEventArgs e)
        {
            var favControl = new FavReceptsControl();
            favControl.RecipeSelected += AllReceptsControl_RecipeSelected;
            MainContentControl.Content = favControl;
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
            SearchTextBox.Width = 500;
        }


        // холод
        private void FridgeBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция холодильника в разработке", "Информация",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnClosed(EventArgs e)
        {
            _dbContext?.Dispose();
            base.OnClosed(e);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск..." || SearchTextBox.Text == "ищите среди своих рецептов...")
            {
                SearchTextBox.Text = "";
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск...";
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainContentControl?.Content is AllReceptsControl allReceptsControl)
            {
                string searchText = SearchTextBox.Text;

                if (string.IsNullOrWhiteSpace(searchText) ||
                    searchText == "Поиск..." ||
                    searchText == "ищите среди своих рецептов...")
                {
                    allReceptsControl.LoadAllRecipes();
                }
                else
                {
                    allReceptsControl.SearchRecipes(searchText);
                }
            }
        }
    }
}