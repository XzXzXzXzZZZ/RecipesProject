using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RecipesProject.UI.AllRecepts;   
using RecipesProject.UI.FavRecepts;   
using RecipesProject.UI.NewRecepts;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();
            MainContentControl.Content = new AllReceptsControl();
            ShowMainMenuButtons();
        }

        //КНОПКИ ГЛАВ МЕНЮ
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

        //Метод для глав кнопок (раздел ВСЕ РЕЦЕПТЫ)
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

        //Метод для глав кнопок (раздел ЛЮБИМОЕ)
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

        //Метод для глав кнопок (раздел ДОБАВТЬ РЕЦЕПТ)
        private void ShowNewReceptButtons()
        {
            ButtonsPanel.Children.Clear();
            SearchBorder.Visibility = Visibility.Collapsed; //поисковик офф

            var backBtn = CreateButton("Вернуться на главный экран", "Back", 220);
            backBtn.Click += MainMenuBTN_Click;

            ButtonsPanel.Children.Add(backBtn);
        }

        //для создания кнопокк
        private Button CreateButton(string content, string name, double width, Brush background = null)
        {
            Button button = new Button();
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

        //обробатывают нажатия кнопок
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
            MainContentControl.Content = new AllReceptsControl();
            ShowAllReceptsButtons();
        }

        private void MainMenuBTN_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new AllReceptsControl();
            ShowMainMenuButtons();
            SearchBorder.Visibility = Visibility.Visible;
            SearchTextBox.Text = "Поиск...";
            SearchTextBox.Width = 500;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}