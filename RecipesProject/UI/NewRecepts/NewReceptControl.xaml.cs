using Microsoft.VisualBasic;
using Microsoft.Win32;
using RecipesProject.Models;
using RecipesProject.UI.MainMenu;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RecipesProject.UI.NewRecepts
{
    public partial class NewReceptControl : UserControl
    {
        Recipe? recipe = null;
        public NewReceptControl(Recipe? recipe)
        {
            InitializeComponent();
            this.recipe = recipe;
            LoadDataFromTemporarySaving();

            if (SearchIngredientTextBox != null)
            {
                SearchIngredientTextBox.Text = "Какой ингредиент вы ищете?";
                SearchIngredientTextBox.Foreground = Brushes.Gray;
            }
        }

        public NewReceptControl()
        {
            InitializeComponent();

            if (SearchIngredientTextBox != null)
            {
                SearchIngredientTextBox.Text = "Какой ингредиент вы ищете?";
                SearchIngredientTextBox.Foreground = Brushes.Gray;
            }
        }

        //-- Подгружаем данные
        void LoadDataFromTemporarySaving()
        {
            if(recipe != null)
            {
                NameTextBox.Text = (!String.IsNullOrEmpty(recipe.Title) ?
               recipe.Title : "");

                ViewImage.Source = ImageMethods.readImage(recipe.MainPhotoPath);

                DescriptionTextBox.Text = (!String.IsNullOrEmpty(recipe.Description) ?
                   recipe.Description : "");

                if (0 <= recipe.Difficulty && recipe.Difficulty <= 5)
                    DifficultyIntegerUpDown.Value = recipe.Difficulty;

                if (1 <= recipe.Servings && recipe.Servings <= 100)
                    ServingsIntegerUpDown.Value = recipe.Servings;

                IngredientsTextBox.Text = (!String.IsNullOrEmpty(recipe.Ingredient.Text) ? recipe.Ingredient.Text : "");
            }
        }

        private void SearchIngredientTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchIngredientTextBox.Text == "Какой ингредиент вы ищете?")
            {
                SearchIngredientTextBox.Text = "";
                SearchIngredientTextBox.Foreground = Brushes.Black;
            }
        }

        private void SearchIngredientTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchIngredientTextBox.Text))
            {
                SearchIngredientTextBox.Text = "Какой ингредиент вы ищете?";
                SearchIngredientTextBox.Foreground = Brushes.Gray;
            }
        }

        // Переход к созданию шагов
        private void GoToStepsButton_Click(object sender, RoutedEventArgs e)
        {
            //Проверки
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите название рецепта", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(IngredientsTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите ингредиенты", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                IngredientsTextBox.Focus();
                return;
            }

            var parent = this.Parent as ContentControl;
            if (parent != null)
            {
                //-- Перед переходом в другой контроллер сохраняем данные
                if(recipe == null)
                {
                    recipe = new Recipe();
                }

                recipe.Title = NameTextBox.Text;
                recipe.Description = DescriptionTextBox.Text;
                recipe.Difficulty = DifficultyIntegerUpDown.Value;
                recipe.Servings = ServingsIntegerUpDown.Value;
                recipe.Ingredient = new Ingredient() {Text = IngredientsTextBox.Text, Recipe = recipe };

                ViewImage.Source = null;

                var stepControl = new StepControl(recipe);
                stepControl.OnRecipeSaved = () => ReturnToMainScreen();
                parent.Content = stepControl;
            }
        }

        //-- При вызове события сохранения - переходим в главное меню
        private void ReturnToMainScreen()
        {
            //-- Возвращаемся на главный экран
            var windowMain = Application.Current.MainWindow as WindowMain;
            if (windowMain != null)
            {
                windowMain.loadMainMenu();
            }
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Все файлы (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                //-- Подгрузка фото
                ViewImage.Source = ImageMethods.readImage(filePath); ;

                if (recipe == null)
                {
                    recipe = new Recipe();
                }
                recipe.MainPhotoPath = filePath;
            }

        }
    }
}