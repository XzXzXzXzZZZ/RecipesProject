using Microsoft.EntityFrameworkCore;
using RecipesProject.Data;
using RecipesProject.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RecipesProject.UI.Fridge
{
    public partial class FridgeControl : UserControl
    {
        private readonly DBContext dBContext;
        private ProductRepository productRepository;

        // Коллекция выбранных ингредиентов
        private ObservableCollection<IngredientItem> selectedIngredients = new ObservableCollection<IngredientItem>();

        // Коллекция постоянных продуктов
        private ObservableCollection<IngredientItem> constantIngredients = new ObservableCollection<IngredientItem>();

        // Список базовых ингредиентов (можно удалять)
        private ObservableCollection<IngredientItem> baseIngredients = new ObservableCollection<IngredientItem>();

        // Параметры пагинации
        private int itemsPerPage = 12;
        private int currentPage = 0;
        private int totalPages = 0;

        // Выбранный рецепт для удаления
        private string selectedRecipeToDelete = null;

        public FridgeControl()
        {
            InitializeComponent();
            dBContext = new DBContext();
            productRepository = new ProductRepository(dBContext);
            InitializeIngredients();
            CalculateTotalPages();
            UpdateDisplay();
            UpdateConstantProductsDisplay();
        }

        private void InitializeIngredients()
        {
            try
            {
                if(productRepository != null)
                {
                    var allIngredients = productRepository.GetAllMyProducts();
                    baseIngredients.Clear();
                    constantIngredients.Clear();

                    foreach (var product in allIngredients)
                    {
                        var ingredientItem = new IngredientItem
                        {
                            Id = product.Id,
                            Name = product.Name
                        };

                        if (product.IsPermanent == false)
                            baseIngredients.Add(ingredientItem);
                        else
                            constantIngredients.Add(ingredientItem);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки продуктов...", "Ошибка",
                   MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateTotalPages()
        {
            totalPages = (int)System.Math.Ceiling((double)baseIngredients.Count / itemsPerPage);
        }

        private void UpdateDisplay()
        {
            if (baseIngredients.Count == 0)
            {
                IngredientsPanel.Children.Clear();
                EmptyBaseText.Visibility = Visibility.Visible;
                UpdatePaginationButtons();
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                return;
            }
            else
            {
                EmptyBaseText.Visibility = Visibility.Collapsed;
                IngredientsPanel.Children.Clear();

                var currentPageIngredients = baseIngredients
                    .Skip(currentPage * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToArray();

                foreach (var ingredient in currentPageIngredients)
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
                        Background = IsIngredientSelected(ingredient)
                            ? (Brush)new SolidColorBrush(Color.FromRgb(160, 160, 160))
                            : (Brush)new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                        Foreground = IsIngredientSelected(ingredient)
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
                        Text = ingredient.Name,
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
            }
            UpdatePaginationButtons();

            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = currentPage < totalPages - 1;
        }

        private bool IsIngredientSelected(IngredientItem ingredient)
        {
            return selectedIngredients.Any(i => i.Id == ingredient.Id);
        }

        private void IngredientButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag is IngredientItem ingredient)
            {
                if (IsIngredientSelected(ingredient))
                {
                    var selected = selectedIngredients.FirstOrDefault(i => i.Id == ingredient.Id);
                    if (selected != null)
                        selectedIngredients.Remove(selected);
                    button.Background = (Brush)new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    button.Foreground = Brushes.Black;
                }
                else
                {
                    selectedIngredients.Add(ingredient);
                    button.Background = (Brush)new SolidColorBrush(Color.FromRgb(160, 160, 160));
                    button.Foreground = Brushes.White;
                }

                System.Diagnostics.Debug.WriteLine($"Ингредиент: {ingredient.Name}, Выбрано: {IsIngredientSelected(ingredient)}");
            }
        }

        private void DeleteBaseIngredient_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is IngredientItem ingredient)
            {
                if (MessageBox.Show($"Удалить базовый ингредиент '{ingredient.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    baseIngredients.Remove(ingredient);
                    productRepository.RemoveMyProduct(ingredient.Id, false);
                    var selected = selectedIngredients.FirstOrDefault(i => i.Id == ingredient.Id);
                    if (selected != null)
                        selectedIngredients.Remove(selected);
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

            bool existsInBase = baseIngredients.Any(i => i.Name == ingredientName);
            bool existsInConstant = constantIngredients.Any(i => i.Name == ingredientName);

            if (!existsInBase && !existsInConstant)
            {
                productRepository.AddMyProduct(ingredientName, false);
                InitializeIngredients();
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

        private void UpdateConstantProductsDisplay()
        {
            if (constantIngredients.Count == 0)
            {
                ConstantProductsPanel.Children.Clear();
                EmptyConstantText.Visibility = Visibility.Visible;
                return;
            }
            EmptyConstantText.Visibility = Visibility.Collapsed;
            ConstantProductsPanel.Children.Clear();

            foreach (var ingredient_const in constantIngredients)
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };

                Border ingredient_const_Border = new Border
                {
                    Style = (Style)FindResource("ProductItemStyle"),
                    Tag = ingredient_const
                };

                TextBlock ingredient_const_Text = new TextBlock
                {
                    Text = ingredient_const.Name,
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
                    Tag = ingredient_const,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Gray
                };
                deleteBtn.Click += DeleteConstantProduct_Click;

                panel.Children.Add(ingredient_const_Text);
                panel.Children.Add(deleteBtn);
                ingredient_const_Border.Child = panel;
                ConstantProductsPanel.Children.Add(ingredient_const_Border);
            }
        }

        private void DeleteConstantProduct_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is IngredientItem ingredient_const)
            {
                if (MessageBox.Show($"Удалить постоянный ингредиент '{ingredient_const.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    constantIngredients.Remove(ingredient_const);
                    productRepository.RemoveMyProduct(ingredient_const.Id, true);
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
            string ingredient_const_Name = NewProductNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(ingredient_const_Name))
            {
                MessageBox.Show("Введите название ингредиента", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool existsInBase = baseIngredients.Any(i => i.Name == ingredient_const_Name);
            bool existsInConstant = constantIngredients.Any(i => i.Name == ingredient_const_Name);

            if (!existsInBase && !existsInConstant)
            {
                productRepository.AddMyProduct(ingredient_const_Name, true);
                InitializeIngredients();
                UpdateConstantProductsDisplay();
            }
            else
            {
                MessageBox.Show("Такой ингредиент уже существует", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            AddProductPanel.Visibility = Visibility.Collapsed;
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

        public ObservableCollection<IngredientItem> GetSelectedIngredients()
        {
            return selectedIngredients;
        }

        public void ClearAllSelections()
        {
            selectedIngredients.Clear();
            UpdateDisplay();
        }
    }

    public class IngredientItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}