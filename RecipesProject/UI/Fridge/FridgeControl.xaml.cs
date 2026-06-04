using Microsoft.EntityFrameworkCore;
using RecipesProject.Data;
using RecipesProject.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RecipesProject.UI.Fridge
{
    public partial class FridgeControl : UserControl
    {
        private readonly DBContext dBContext;
        private ProductRepository productRepository;

        public event EventHandler<int> RecipeSelected;

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
        private int selectedRecipeToDelete = 0;

        public FridgeControl()
        {
            InitializeComponent();
            dBContext = new DBContext();
            productRepository = new ProductRepository(dBContext);
            InitializeIngredients();
            CalculateTotalPages();
            UpdateDisplay();
            UpdateConstantProductsDisplay();

            this.PreviewMouseDown += FridgeControl_MouseDown;
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
            UpdateRecipesContainer();
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
                UpdateRecipesContainer();
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

            UpdateRecipesContainer();
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
            if (selectedRecipeToDelete != 0)
            {
                try 
                {
                    RecipeRepository recipeRepository = new RecipeRepository(dBContext);
                    var selectedRecipe = recipeRepository.GetById(selectedRecipeToDelete);
                    if(selectedRecipe != null)
                    {
                        if (MessageBox.Show($"Удалить рецепт '{selectedRecipe.Title}'?", "Подтверждение",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            recipeRepository.Delete(selectedRecipeToDelete);
                            MessageBox.Show($"Рецепт '{selectedRecipe.Title}' удалён", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            selectedRecipeToDelete = 0;
                            DeleteRecipeBtn.Visibility = Visibility.Collapsed;
                            UpdateRecipesContainer();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Рецепт '{selectedRecipeToDelete}'. Ошибка удаления", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public void ShowDeleteButton(int id)
        {
            selectedRecipeToDelete = id;
            DeleteRecipeBtn.Visibility = Visibility.Visible;
        }

        public void HideDeleteButton()
        {
            selectedRecipeToDelete = 0;
            DeleteRecipeBtn.Visibility = Visibility.Collapsed;
        }

        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

        public void ClearAllSelections()
        {
            selectedIngredients.Clear();
            UpdateDisplay();
            UpdateRecipesContainer();
        }

        void UpdateRecipesContainer()
        {
            RecipesContainer.Items.Clear();
            if(selectedIngredients.Count == 0 && constantIngredients.Count == 0)
            {
                RecipesContainer.Visibility = Visibility.Collapsed;
                EmptyRecipesText.Visibility = Visibility.Visible;
                return;
            }
            EmptyRecipesText.Visibility= Visibility.Collapsed;
            RecipesContainer.Visibility= Visibility.Visible;
            RecipeRepository recipeRepository = new RecipeRepository(dBContext);

            List<string> ingredients = selectedIngredients.Select(i => i.Name).ToList();
            ingredients.AddRange(constantIngredients.Select(i=>i.Name).ToList());

            var result = recipeRepository.FindByIngredients(ingredients);

            bool hasAnyRecipe = false;

            if (result.Count > 0)
            {
                foreach (var recipe in result)
                {
                    var border = new Border
                    {
                        Background = Brushes.White,
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(15),
                        Margin = new Thickness(0, 0, 0, 5),
                        Tag = recipe.Id
                    };

                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    // Иконка
                    var icon = new TextBlock
                    {
                        Text = "🍽️",
                        FontSize = 20,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(icon, 0);
                    grid.Children.Add(icon);

                    // Название
                    var title = new TextBlock
                    {
                        Text = recipe.Title,
                        FontSize = 15,
                        FontWeight = FontWeights.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    Grid.SetColumn(title, 1);
                    grid.Children.Add(title);

                    // Время
                    var time = new TextBlock
                    {
                        Text = $" ⏱️ {recipe.CookingTime} мин",
                        FontSize = 12,
                        Foreground = Brushes.Gray,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(time, 2);
                    grid.Children.Add(time);

                    border.Child = grid;

                    var item = new ListBoxItem
                    {
                        Content = border,
                        Tag = recipe.Id,
                        Cursor = System.Windows.Input.Cursors.Hand
                    };

                    RecipesContainer.Items.Add(item);
                    hasAnyRecipe = true;
                } 
            }
            if (!hasAnyRecipe)
            {
                RecipesContainer.Visibility = Visibility.Collapsed;
                EmptyRecipesText.Visibility = Visibility.Visible;
            }
        }

        private void ReceptsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = RecipesContainer.SelectedItem as ListBoxItem;
            if (selectedItem?.Tag != null)
            {
                if (int.TryParse(selectedItem.Tag.ToString(), out int recipeId))
                {
                    RecipeSelected?.Invoke(this, recipeId);
                }
            }
        }

        private void RecipesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если ничего не выбрано - скрываем кнопку удаления
            if (RecipesContainer.SelectedItem == null)
            {
                HideDeleteButton();
                return;
            }

            var selectedItem = RecipesContainer.SelectedItem;

            // Определяем ID рецепта
            int? recipeId = null;

            if (selectedItem is ListBoxItem listBoxItem && listBoxItem.Tag != null)
            {
                if (int.TryParse(listBoxItem.Tag.ToString(), out int id))
                    recipeId = id;
            }
            else if (selectedItem is Recipe recipe)
            {
                recipeId = recipe.Id;
            }

            // Показываем кнопку удаления, если есть ID
            if (recipeId.HasValue)
            {
                ShowDeleteButton(recipeId.Value);
            }
            else
            {
                HideDeleteButton();
            }
        }

        private void FridgeControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var originalSource = e.OriginalSource as DependencyObject;
            bool isClickOnListBox = IsElementInListBox(originalSource);

            if (!isClickOnListBox)
            {
                RecipesContainer.SelectedItem = null;
                HideDeleteButton();
            }
        }

        private bool IsElementInListBox(DependencyObject element)
        {
            while (element != null)
            {
                if (element == RecipesContainer || element is ListBoxItem || element == DeleteRecipeBtn)
                    return true;
                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }
    }

    public class IngredientItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}