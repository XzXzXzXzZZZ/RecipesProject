using Microsoft.Win32;
using RecipesProject.Models;
using RecipesProject.UI.MainMenu;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        }
        public NewReceptControl()
        {
            InitializeComponent();
        }

        //Подгружаем данные
        void LoadDataFromTemporarySaving()
        {
            if (recipe == null) return;

            NameTextBox.Text = recipe.Title ?? "";
            ViewImage.Source = ImageMethods.readImage(recipe.MainPhotoPath);
            DescriptionTextBox.Text = recipe.Description ?? "";

            if (recipe.Difficulty >= 0 && recipe.Difficulty <= 5)
                DifficultyIntegerUpDown.Value = recipe.Difficulty;

            if (recipe.Servings >= 1 && recipe.Servings <= 100)
                ServingsIntegerUpDown.Value = recipe.Servings;

            // Ингредиенты как коллекция 
            if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
                IngredientsTextBox.Text = string.Join("\n", recipe.Ingredients.Select(i => i.Text));
        }

        // Переход к созданию шагов
        private void GoToStepsButton_Click(object sender, RoutedEventArgs e)
        {
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
                if (recipe == null)
                    recipe = new Recipe();

                recipe.Title = NameTextBox.Text;
                recipe.Description = DescriptionTextBox.Text;
                recipe.Difficulty = DifficultyIntegerUpDown.Value;
                recipe.Servings = ServingsIntegerUpDown.Value;

                // Ингредиенты — создаём список из строк
                var lines = IngredientsTextBox.Text.Split('\n', '\r')
                    .Where(l => !string.IsNullOrWhiteSpace(l));
                var list = new List<Ingredient>();
                foreach (var line in lines)
                    list.Add(new Ingredient { Text = line.Trim(), Recipe = recipe });
                recipe.Ingredients = list;

                ViewImage.Source = null;

                var stepControl = new StepControl(recipe);
                //-- При вызове события сохранения - переходим в главное меню
                stepControl.OnRecipeSaved = () => ReturnToMainScreen();
                parent.Content = stepControl;
            }
        }
            
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