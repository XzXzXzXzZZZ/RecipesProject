using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using RecipesProject.UI.AllRecepts;  
using RecipesProject.UI.FavRecepts;   
using RecipesProject.UI.NewRecepts;
using RecipesProject.Models;
using RecipesProject.Data;

namespace RecipesProject.UI.MainMenu
{
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    dbContext.Database.Migrate();
                    string[] defaultIngredients = new string[]
                    {
                        "Огурцы",
                        "Помидоры",
                        "Яйца",
                        "Сыр",
                        "Мясо",
                        "Рыба",
                        "Курица",
                        "Картошка",
                        "Морковка",
                        "Лук",
                        "Чеснок",
                        "Брокколи",
                        "Грибы",
                        "Рис",
                        "Паста",
                        "Молоко",
                        "Масло",
                        "Зелень",
                        "Хлеб",
                        "Мед",
                        "Лимоны",
                        "Апельсины",
                        "Яблоки",
                        "Бананы",
                        "Клубника",
                        "Киви",
                        "Баклажаны",
                        "Перцы",
                        "Авокадо"
                    };
                    ProductRepository productRepository = new ProductRepository(dbContext);
                    productRepository.SeedPermanentProducts();
                    foreach (string productName in defaultIngredients)
                    {
                        productRepository.AddMyProduct(productName, false);
                    }
                }
            }
            catch(Exception e) { 
                MessageBox.Show(e.ToString());
            }
            
            InitializeComponent();
            loadMainMenu();
        }

        // Глав меню
        private void ShowMainMenuButtons()
        {
            ButtonsPanel.Children.Clear();

            var favoriteBtn = CreateButton("❤ Избранное", "Favorite", 100);
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
            PlaceholderText.Text = "Найдите рецепт. . .";
            SearchTextBox.Width = 500;

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
            PlaceholderText.Text = "Найдите рецепт среди избранных. . .";
            SearchTextBox.Width = 500;

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
            SearchBorder.Visibility = Visibility.Collapsed;

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

        //-- Обработчики нажатий кнопок
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
            loadMainMenu();
        }

        public void loadMainMenu()
        {
            LoadAllRecipes();
            ShowMainMenuButtons();
            SearchBorder.Visibility = Visibility.Visible;
            PlaceholderText.Text = "Найдите рецепт. . .";
            SearchTextBox.Width = 500;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///           ХОЛОДИЛЬНИК
        private void FridgeBTN_Click(object sender, RoutedEventArgs e)
        {
            var fridgeControl = new Fridge.FridgeControl();
            fridgeControl.RecipeSelected += AllReceptsControl_RecipeSelected;
            MainContentControl.Content = fridgeControl;
            SearchBorder.Visibility = Visibility.Collapsed;
            ShowFridgeButtons();
        }

        //Кнопки в режиме холодильника
        private void ShowFridgeButtons()
        {
            ButtonsPanel.Children.Clear();

            var backToMenuBtn = CreateButton("🏠 Главный экран", "MainMenu", 140);
            var clearSelectionBtn = CreateButton("🗑 Очистить выбор", "ClearSelection", 140);

            backToMenuBtn.Click += MainMenuBTN_Click;
            clearSelectionBtn.Click += ClearSelectionBTN_Click;

            ButtonsPanel.Children.Add(backToMenuBtn);
            ButtonsPanel.Children.Add(clearSelectionBtn);
        }

        private void ClearSelectionBTN_Click(object sender, RoutedEventArgs e)
        {
            if (MainContentControl.Content is Fridge.FridgeControl fridgeControl)
            {
                fridgeControl.ClearAllSelections();
            }
            SearchTextBox.Width = 500;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainContentControl?.Content is AllReceptsControl allReceptsControl)
            {
                string searchText = SearchTextBox.Text;

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    PlaceholderText.Visibility = Visibility.Visible;
                    allReceptsControl.LoadAllRecipes();
                }
                else
                {
                    PlaceholderText.Visibility = Visibility.Collapsed;
                    allReceptsControl.SearchRecipes(searchText);
                }
            }

            if(MainContentControl?.Content is FavReceptsControl favReceptsControl)
            {
                string searchText = SearchTextBox.Text;

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    PlaceholderText.Visibility = Visibility.Visible;
                    favReceptsControl.LoadFavorites();
                }
                else
                {
                    PlaceholderText.Visibility = Visibility.Collapsed;
                    favReceptsControl.SearchRecipesInFavorite(searchText);
                }
            }
        }
    }
}