using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            // Начальное состояние для поля шагов
            StepTextBox.Text = "";
            UpdatePlaceholderVisibility();

            // Начальное состояние для полей времени
            HoursTextBox.Text = "";
            MinutesTextBox.Text = "";
            UpdateHoursPlaceholderVisibility();
            UpdateMinutesPlaceholderVisibility();
        }

        public StepControl()
        {
            InitializeComponent();
            StepTextBox.Text = "";
            HoursTextBox.Text = "";
            MinutesTextBox.Text = "";
            UpdatePlaceholderVisibility();
            UpdateHoursPlaceholderVisibility();
            UpdateMinutesPlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (PlaceholderText != null)
            {
                PlaceholderText.Visibility = string.IsNullOrEmpty(StepTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateHoursPlaceholderVisibility()
        {
            if (HoursPlaceholderText != null)
            {
                HoursPlaceholderText.Visibility = string.IsNullOrEmpty(HoursTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateMinutesPlaceholderVisibility()
        {
            if (MinutesPlaceholderText != null)
            {
                MinutesPlaceholderText.Visibility = string.IsNullOrEmpty(MinutesTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
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

        // Обработчики для часов
        private void HoursTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (HoursPlaceholderText != null)
            {
                HoursPlaceholderText.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(HoursTextBox.Text))
            {
                HoursTextBox.Text = "";
            }
        }

        private void HoursTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateHoursPlaceholderVisibility();
            if (string.IsNullOrEmpty(HoursTextBox.Text))
            {
                HoursTextBox.Text = "";
            }
        }

        private void HoursTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHoursPlaceholderVisibility();
        }

        private void HoursTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Обработчики для минут
        private void MinutesTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MinutesPlaceholderText != null)
            {
                MinutesPlaceholderText.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(MinutesTextBox.Text))
            {
                MinutesTextBox.Text = "";
            }
        }

        private void MinutesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateMinutesPlaceholderVisibility();

            // Проверка: если минуты указаны, они не должны быть больше 59
            if (!string.IsNullOrEmpty(MinutesTextBox.Text))
            {
                int minutes = int.Parse(MinutesTextBox.Text);
                if (minutes > 59)
                {
                    MessageBox.Show("Минуты не могут быть больше 59",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    MinutesTextBox.Text = "";
                    MinutesTextBox.Focus();
                }
            }
        }

        private void MinutesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMinutesPlaceholderVisibility();
        }

        private void MinutesTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
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

        private string FormatCookingTime()
        {
            bool hasHours = !string.IsNullOrEmpty(HoursTextBox.Text) && HoursTextBox.Text != "0";
            bool hasMinutes = !string.IsNullOrEmpty(MinutesTextBox.Text) && MinutesTextBox.Text != "0";

            // Если ни часы, ни минуты не указаны
            if (!hasHours && !hasMinutes)
            {
                return "-";
            }

            int hours = hasHours ? int.Parse(HoursTextBox.Text) : 0;
            int minutes = hasMinutes ? int.Parse(MinutesTextBox.Text) : 0;

            string result = "";
            if (hours > 0 && minutes > 0)
            {
                result = $"{hours}ч {minutes}м";
            }
            else if (hours > 0)
            {
                result = $"{hours}ч";
            }
            else
            {
                result = $"{minutes}м";
            }

            return result;
        }

        private void SaveRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия шагов (обязательно)
            if (steps.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один шаг", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string allSteps = string.Join("\n", steps);
            string cookingTime = FormatCookingTime();

            MessageBox.Show($"Рецепт \"{recipeName}\" успешно сохранен!\n\n" +
                $"Ингредиенты:\n{ingredients}\n\n" +
                $"Время приготовления: {cookingTime}\n\n" +
                $"Шаги:\n{allSteps}",
                "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}