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
            "ыыыыыаяустала",
            "Яблоки",
            "Бананы",
            "Клубники",
            "Киви",
            "Баклажаны",
            "Перцы",
            "Авокадо"
        };

        // Параметры пагинации
        private int itemsPerPage = 12; // Количество ингредиентов на одной странице
        private int currentPage = 0;
        private int totalPages = 0;

        public FridgeControl()
        {
            InitializeComponent();
            CalculateTotalPages();
            UpdateDisplay();
        }

        private void CalculateTotalPages()
        {
            totalPages = (int)System.Math.Ceiling((double)allIngredients.Length / itemsPerPage);
        }

        private void UpdateDisplay()
        {
            // ингредиенты для текуцш страницы
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

        //это надо для поиска рецептов (потом)
        public ObservableCollection<string> GetSelectedIngredients()
        {
            return selectedIngredients;
        }

        //сброс тугл кнопок
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
}