using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            // Начальное состояние
            StepTextBox.Text = "";
            UpdatePlaceholderVisibility();
        }

        public StepControl()
        {
            InitializeComponent();
            StepTextBox.Text = "";
            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (PlaceholderText != null)
            {
                PlaceholderText.Visibility = string.IsNullOrEmpty(StepTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void StepTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceholderText != null)
            {
                PlaceholderText.Visibility = Visibility.Collapsed;
            }
        }

        private void StepTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void StepTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StepTextBox.Text))
            {
                steps.Add($"Шаг {steps.Count + 1}: {StepTextBox.Text}");
                StepTextBox.Clear();
                UpdatePlaceholderVisibility();
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

            MessageBox.Show($"Рецепт \"{recipeName}\" успешно сохранен!\n\nИнгредиенты:\n{ingredients}\n\nШаги:\n{allSteps}",
                "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}