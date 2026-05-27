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
                parent.Content = new StepControl(NameTextBox.Text, IngredientsTextBox.Text);
            }
        }
    }
}