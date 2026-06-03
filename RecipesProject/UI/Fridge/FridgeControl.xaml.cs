using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace RecipesProject.UI.Fridge
{
    public partial class FridgeControl : UserControl
    {
        // Коллекция выбранных ингредиентов
        private ObservableCollection<string> selectedIngredients = new ObservableCollection<string>();

        // Коллекция постоянных продуктов
        private ObservableCollection<ConstantProduct> constantProducts = new ObservableCollection<ConstantProduct>();

        // Список всех доступных ингредиентов
        private string[] allIngredients = new string[]
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

        // Параметры пагинации
        private int itemsPerPage = 12;
        private int currentPage = 0;
        private int totalPages = 0;

        // Выбранный рецепт для удаления
        private string selectedRecipeToDelete = null;

        public FridgeControl()
        {
            InitializeComponent();
            CalculateTotalPages();
            UpdateDisplay();
            LoadConstantProducts();
        }

        private void CalculateTotalPages()
        {
            totalPages = (int)System.Math.Ceiling((double)allIngredients.Length / itemsPerPage);
        }

        private void UpdateDisplay()
        {
            var currentPageIngredients = allIngredients
                .Skip(currentPage * itemsPerPage)
                .Take(itemsPerPage)
                .ToArray();

            IngredientsPanel.Children.Clear();

            foreach (string ingredient in currentPageIngredients)
            {
                ToggleButton button = new ToggleButton
                {
                    Content = ingredient,
                    Style = (Style)FindResource("IngredientToggleButtonStyle"),
                    Tag = ingredient
                };
                if (selectedIngredients.Contains(ingredient))
                {
                    button.IsChecked = true;
                }

                button.Checked += IngredientButton_Checked;
                button.Unchecked += IngredientButton_Unchecked;

                IngredientsPanel.Children.Add(button);
            }

            UpdatePaginationButtons();

            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = currentPage < totalPages - 1;
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

        private void IngredientButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            if (button != null && button.Tag != null)
            {
                string ingredient = button.Tag.ToString();
                if (!selectedIngredients.Contains(ingredient))
                {
                    selectedIngredients.Add(ingredient);
                }
                System.Diagnostics.Debug.WriteLine($"Выбран ингредиент: {ingredient}");
            }
        }

        private void IngredientButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            if (button != null && button.Tag != null)
            {
                string ingredient = button.Tag.ToString();
                if (selectedIngredients.Contains(ingredient))
                {
                    selectedIngredients.Remove(ingredient);
                }
                System.Diagnostics.Debug.WriteLine($"Снят ингредиент: {ingredient}");
            }
        }

        // ========== ПОСТОЯННЫЕ ПРОДУКТЫ ==========

        private void LoadConstantProducts()
        {
            // Загрузка сохранённых постоянных продуктов (позже из БД)
            // Пока пример:
            constantProducts.Add(new ConstantProduct { Name = "Соль", IsPermanent = true });
            constantProducts.Add(new ConstantProduct { Name = "Перец", IsPermanent = true });
            constantProducts.Add(new ConstantProduct { Name = "Сахар", IsPermanent = true });
            constantProducts.Add(new ConstantProduct { Name = "Масло растительное", IsPermanent = false });

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
                Border productBorder = new Border
                {
                    Background = product.IsPermanent ? (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 245, 230)) : (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 240, 230)),
                    BorderBrush = product.IsPermanent ? (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 230, 200)) : (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 200, 200)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10, 5, 10, 5),
                    Margin = new Thickness(5)
                };

                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };

                TextBlock productText = new TextBlock
                {
                    Text = product.Name,
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
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = System.Windows.Media.Brushes.Gray
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
            if (btn != null && btn.Tag is ConstantProduct product)
            {
                if (MessageBox.Show($"Удалить продукт '{product.Name}'?", "Подтверждение",
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
            IsPermanentCheckBox.IsChecked = true;
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

            constantProducts.Add(new ConstantProduct
            {
                Name = productName,
                IsPermanent = IsPermanentCheckBox.IsChecked == true
            });

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
                    // TODO: Логика удаления рецепта из БД
                    MessageBox.Show($"Рецепт '{selectedRecipeToDelete}' удалён", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    selectedRecipeToDelete = null;
                    DeleteRecipeBtn.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Метод для отображения кнопки удаления (вызывается при выборе рецепта)
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

            foreach (UIElement element in IngredientsPanel.Children)
            {
                if (element is ToggleButton button)
                {
                    button.IsChecked = false;
                }
            }
        }
    }

    // Класс для постоянного продукта
    public class ConstantProduct
    {
        public string Name { get; set; }
        public bool IsPermanent { get; set; }
    }
}