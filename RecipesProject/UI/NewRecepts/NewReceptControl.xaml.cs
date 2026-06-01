using RecipesProject.UI.MainMenu;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RecipesProject.UI.NewRecepts
{
    public partial class NewReceptControl : UserControl
    {
        public NewReceptControl()
        {
            InitializeComponent();

            //-- Подгружаем данные из временного хранилища, если таковы есть
            NameTextBox.Text = (!String.IsNullOrEmpty(TemporarySavingRecipe.Title) ?
               TemporarySavingRecipe.Title : "");
            IngredientsTextBox.Text = (!String.IsNullOrEmpty(TemporarySavingRecipe.IngredientsText) ?
                TemporarySavingRecipe.IngredientsText : "");

            if (SearchIngredientTextBox != null)
            {
                SearchIngredientTextBox.Text = "Какой ингредиент вы ищете?";
                SearchIngredientTextBox.Foreground = Brushes.Gray;
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
                TemporarySavingRecipe.Title = NameTextBox.Text;
                TemporarySavingRecipe.IngredientsText = IngredientsTextBox.Text;

                var stepControl = new StepControl();
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
    }
}