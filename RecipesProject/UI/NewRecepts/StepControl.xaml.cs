using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RecipesProject.UI.NewRecepts
{
    public partial class StepControl : UserControl
    {
        private ObservableCollection<string> steps = new ObservableCollection<string>();
        private string recipeName;
        private string ingredients;

        public StepControl(string recipeName, string ingredients)
        {
            InitializeComponent();
            this.recipeName = recipeName;
            this.ingredients = ingredients;
            RecipeNameText.Text = recipeName;
            StepsListBox.ItemsSource = steps;
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StepTextBox.Text) && StepTextBox.Text != "Введите шаг...")
            {
                steps.Add($"Шаг {steps.Count + 1}: {StepTextBox.Text}");
                StepTextBox.Clear();
                StepTextBox.Focus();
            }
            else
            {
                MessageBox.Show("Введите текст шага", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as ContentControl;
            if (parent != null)
            {
                parent.Content = new NewReceptControl();
            }
        }

        private void SaveRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (steps.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один шаг", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string allSteps = string.Join("\n", steps);

            //ТУТ ЛОГИКА СОХРАНЕНИЯ В БД!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            MessageBox.Show($"Рецепт \"{recipeName}\" успешно сохранен!\n\nИнгредиенты:\n{ingredients}\n\nШаги:\n{allSteps}",
                "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}