using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RecipesProject.UI.Fridge
{
    public partial class FridgeControl : UserControl
    {
        // Коллекция выбранных ингредиентов
        private ObservableCollection<string> selectedIngredients = new ObservableCollection<string>();

        // Коллекция постоянных продуктов
        private ObservableCollection<string> constantProducts = new ObservableCollection<string>();

        // Список базовых ингредиентов (можно удалять)
        private ObservableCollection<string> baseIngredients = new ObservableCollection<string>();

        // Параметры пагинации
        private int itemsPerPage = 12;
        private int currentPage = 0;
        private int totalPages = 0;

        // Выбранный рецепт для удаления
        private string selectedRecipeToDelete = null;

        public FridgeControl()
        {
            InitializeComponent();
            InitializeBaseIngredients();
            CalculateTotalPages();
            UpdateDisplay();
            LoadConstantProducts();
        }

        private void InitializeBaseIngredients()
        {
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

            foreach (var ingredient in defaultIngredients)
            {
                baseIngredients.Add(ingredient);
            }
        }

        private void CalculateTotalPages()
        {
            totalPages = (int)System.Math.Ceiling((double)baseIngredients.Count / itemsPerPage);
        }

        private void UpdateDisplay()
        {
            var currentPageIngredients = baseIngredients
                .Skip(currentPage * itemsPerPage)
                .Take(itemsPerPage)
                .ToArray();

            IngredientsPanel.Children.Clear();

            foreach (string ingredient in currentPageIngredients)
            {
                // Создаём кастомную кнопку с крестиком
                Button ingredientButton = new Button
                {
                    Tag = ingredient,
                    Width = 110,
                    Height = 40,
                    Margin = new Thickness(5),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    BorderThickness = new Thickness(1),
                    BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    Background = selectedIngredients.Contains(ingredient)
                        ? (Brush)new SolidColorBrush(Color.FromRgb(160, 160, 160))
                        : (Brush)new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    Foreground = selectedIngredients.Contains(ingredient)
                        ? Brushes.White
                        : Brushes.Black
                };

                // Создаём Grid для содержимого кнопки
                Grid buttonGrid = new Grid();
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = System.Windows.GridLength.Auto });

                // Текст ингредиента
                TextBlock ingredientText = new TextBlock
                {
                    Text = ingredient,
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Кнопка-крестик для удаления
                Button deleteButton = new Button
                {
                    Content = "✖",
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(5, 0, 8, 0),
                    FontSize = 12,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = ingredient,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Gray,
                    ToolTip = "Удалить ингредиент"
                };
                deleteButton.Click += DeleteBaseIngredient_Click;

                Grid.SetColumn(ingredientText, 0);
                Grid.SetColumn(deleteButton, 1);

                buttonGrid.Children.Add(ingredientText);
                buttonGrid.Children.Add(deleteButton);

                ingredientButton.Content = buttonGrid;
                ingredientButton.Click += IngredientButton_Click;

                IngredientsPanel.Children.Add(ingredientButton);
            }

            UpdatePaginationButtons();

            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = currentPage < totalPages - 1;
        }

        private void IngredientButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string ingredient = button.Tag.ToString();

                if (selectedIngredients.Contains(ingredient))
                {
                    selectedIngredients.Remove(ingredient);
                    button.Background = (Brush)new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    button.Foreground = Brushes.Black;
                }
                else
                {
                    selectedIngredients.Add(ingredient);
                    button.Background = (Brush)new SolidColorBrush(Color.FromRgb(160, 160, 160));
                    button.Foreground = Brushes.White;
                }

                System.Diagnostics.Debug.WriteLine($"Ингредиент: {ingredient}, Выбрано: {selectedIngredients.Contains(ingredient)}");
            }
        }

        private void DeleteBaseIngredient_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is string ingredient)
            {
                if (MessageBox.Show($"Удалить базовый продукт '{ingredient}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    baseIngredients.Remove(ingredient);
                    selectedIngredients.Remove(ingredient);
                    CalculateTotalPages();
                    if (currentPage >= totalPages && currentPage > 0)
                    {
                        currentPage--;
                    }
                    UpdateDisplay();
                }
            }
        }

        private void UpdatePaginationButtons()
        {
            PagesPanel.Children.Clear();

            for (int i = 0; i < totalPages; i++)
            {
                Button pageButton = new Button
                {
                    Content = (i + 1).ToString(),
                    Tag = i,
                    Style = currentPage == i
                        ? (Style)FindResource("ActivePaginationButtonStyle")
                        : (Style)FindResource("PaginationButtonStyle")
                };
                pageButton.Click += PageButton_Click;
                PagesPanel.Children.Add(pageButton);
            }
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag is int pageNumber)
            {
                currentPage = pageNumber;
                UpdateDisplay();
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdateDisplay();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;
                UpdateDisplay();
            }
        }

        // ========== ДОБАВЛЕНИЕ БАЗОВОГО ИНГРЕДИЕНТА ==========

        private void AddBaseIngredientBtn_Click(object sender, RoutedEventArgs e)
        {
            AddBaseIngredientPanel.Visibility = Visibility.Visible;
            NewBaseIngredientTextBox.Clear();
            NewBaseIngredientTextBox.Focus();
        }

        private void ConfirmAddBaseBtn_Click(object sender, RoutedEventArgs e)
        {
            string ingredientName = NewBaseIngredientTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                MessageBox.Show("Введите название ингредиента", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!baseIngredients.Contains(ingredientName))
            {
                baseIngredients.Add(ingredientName);
                CalculateTotalPages();
                UpdateDisplay();
            }
            else
            {
                MessageBox.Show("Такой ингредиент уже существует", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            AddBaseIngredientPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelAddBaseBtn_Click(object sender, RoutedEventArgs e)
        {
            AddBaseIngredientPanel.Visibility = Visibility.Collapsed;
        }

        // ========== ПОСТОЯННЫЕ ПРОДУКТЫ ==========

        private void LoadConstantProducts()
        {
            constantProducts.Add("Соль");
            constantProducts.Add("Перец");
            constantProducts.Add("Сахар");

            UpdateConstantProductsDisplay();
        }

        private void UpdateConstantProductsDisplay()
        {
            ConstantProductsPanel.Children.Clear();

            if (constantProducts.Count == 0)
            {
                EmptyConstantText.Visibility = Visibility.Visible;
                return;
            }

            EmptyConstantText.Visibility = Visibility.Collapsed;

            foreach (var product in constantProducts)
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };

                Border productBorder = new Border
                {
                    Style = (Style)FindResource("ProductItemStyle"),
                    Tag = product
                };

                TextBlock productText = new TextBlock
                {
                    Text = product,
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button deleteBtn = new Button
                {
                    Content = "✖",
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(10, 0, 0, 0),
                    FontSize = 12,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = product,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Gray
                };
                deleteBtn.Click += DeleteConstantProduct_Click;

                panel.Children.Add(productText);
                panel.Children.Add(deleteBtn);
                productBorder.Child = panel;
                ConstantProductsPanel.Children.Add(productBorder);
            }
        }

        private void DeleteConstantProduct_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is string product)
            {
                if (MessageBox.Show($"Удалить постоянный продукт '{product}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    constantProducts.Remove(product);
                    UpdateConstantProductsDisplay();
                }
            }
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            AddProductPanel.Visibility = Visibility.Visible;
            NewProductNameTextBox.Clear();
            NewProductNameTextBox.Focus();
        }

        private void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
        {
            string productName = NewProductNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Введите название продукта", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            constantProducts.Add(productName);
            AddProductPanel.Visibility = Visibility.Collapsed;
            UpdateConstantProductsDisplay();
        }

        private void CancelAddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddProductPanel.Visibility = Visibility.Collapsed;
        }

        // ========== УДАЛЕНИЕ РЕЦЕПТА ==========

        private void DeleteRecipeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipeToDelete != null)
            {
                if (MessageBox.Show($"Удалить рецепт '{selectedRecipeToDelete}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Рецепт '{selectedRecipeToDelete}' удалён", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    selectedRecipeToDelete = null;
                    DeleteRecipeBtn.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void ShowDeleteButton(string recipeName)
        {
            selectedRecipeToDelete = recipeName;
            DeleteRecipeBtn.Visibility = Visibility.Visible;
        }

        public void HideDeleteButton()
        {
            selectedRecipeToDelete = null;
            DeleteRecipeBtn.Visibility = Visibility.Collapsed;
        }

        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

        public ObservableCollection<string> GetSelectedIngredients()
        {
            return selectedIngredients;
        }

        public void ClearAllSelections()
        {
            selectedIngredients.Clear();
            UpdateDisplay();
        }
    }
}