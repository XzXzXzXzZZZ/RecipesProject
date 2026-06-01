using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecipesProject.UI.NewRecepts
{
    public partial class StepControl : UserControl
    {
        private ObservableCollection<string> steps = new ObservableCollection<string>();
        private string recipeName;
        private string ingredients;

        //-- Событие сохранения
        public Action OnRecipeSaved { get; set; }

        public StepControl()
        {
            InitializeComponent();

            //-- Подгружаем данные из временного хранилища, если те имеются
            this.recipeName = (!String.IsNullOrEmpty(TemporarySavingRecipe.Title) ?
            TemporarySavingRecipe.Title : "");

            this.ingredients = (!String.IsNullOrEmpty(TemporarySavingRecipe.IngredientsText) ?
                TemporarySavingRecipe.IngredientsText : "");

            RecipeNameText.Text = (!String.IsNullOrEmpty(TemporarySavingRecipe.Title) ?
                TemporarySavingRecipe.Title : "");

            StepsListBox.ItemsSource = steps;
            if(TemporarySavingRecipe.Steps!= null)
                for(int i=0; i < TemporarySavingRecipe.Steps.Count; i++)
                {
                    steps.Add($"Шаг {i+1}: " + TemporarySavingRecipe.Steps[i]);
                }

            var time = CookingTimeMethods.ConvertingTimeString(TemporarySavingRecipe.CookingTime);
            HoursTextBox.Text = $"{((time.hour == 0)? "": time.hour)}";
            MinutesTextBox.Text = $"{((time.minute == 0) ? "": time.minute)}";

            StepTextBox.Text = "";
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
                //-- Перед переходом сохраняем данные
                Regex regex = new Regex(@"^Шаг\s*\d+\s*:\s*");

                TemporarySavingRecipe.Title = recipeName;
                TemporarySavingRecipe.IngredientsText = ingredients;
                TemporarySavingRecipe.Steps = steps.Select(s => regex.Replace(s, "")).ToList();
                TemporarySavingRecipe.CookingTime = FormatCookingTime();

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

            List<string> stepsString = steps.ToList();

            try
            {
                NewAndUpdateRecipe.CreateAndSaveRecipe(recipeName, ingredients, cookingTime, stepsString, TemporarySavingRecipe.Id);

                MessageBox.Show($"Рецепт \"{recipeName}\" успешно сохранен!\n\n" +
                $"Ингредиенты:\n{ingredients}\n\n" +
                $"Время приготовления: {cookingTime}\n\n" +
                $"Шаги:\n{allSteps}",
                "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);

                OnRecipeSaved?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления рецепта {ex.Message}", "Ошибка",
                   MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }
    }
}