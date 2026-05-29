using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RecipesProject.Data;
using RecipesProject.Models;

namespace RecipesProject.UI.FavRecepts
{
    public partial class FavReceptsControl : UserControl
    {
        private RecipeRepository _repository;
        private DBContext _dbContext;

        public event EventHandler<int> RecipeSelected;

        public FavReceptsControl()
        {
            InitializeComponent();
            InitializeRepository();
        }

        private void InitializeRepository()
        {
            try
            {
                _dbContext = new DBContext();
                _repository = new RecipeRepository(_dbContext);
                LoadFavorites();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadFavorites()
        {
            if (_repository == null) return;

            var favorites = _repository.GetFavorites();
            UpdateListBox(favorites);
        }

        private void UpdateListBox(List<Recipe> recipes)
        {
            FavReceptsListBox.Items.Clear();

            if (recipes == null || recipes.Count == 0)
            {
                var noResultItem = new ListBoxItem
                {
                    Content = new TextBlock
                    {
                        Text = "Нет избранных рецептов",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 20, 0, 0)
                    }
                };
                FavReceptsListBox.Items.Add(noResultItem);
                return;
            }

            foreach (var recipe in recipes)
            {
                // Карточка рецепта
                var border = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Иконка
                var icon = new TextBlock
                {
                    Text = "❤️",
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
                    Text = $" {recipe.CookingTime} мин",
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

                FavReceptsListBox.Items.Add(item);
            }
        }

        private void FavReceptsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = FavReceptsListBox.SelectedItem as ListBoxItem;
            if (selectedItem?.Tag != null)
            {
                if (int.TryParse(selectedItem.Tag.ToString(), out int recipeId))
                {
                    RecipeSelected?.Invoke(this, recipeId);
                }
            }
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}