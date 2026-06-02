using Microsoft.Win32;
using RecipesProject.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecipesProject.UI.NewRecepts
{
    public partial class StepControl : UserControl
    {
        private ObservableCollection<StepItem> steps = new ObservableCollection<StepItem>();
        Recipe recipe;
        string? pathImage = null;

        //-- Событие сохранения
        public Action OnRecipeSaved { get; set; }

        public StepControl(Recipe recipe)
        {
            InitializeComponent();
            this.recipe = recipe;
            StepsListBox.ItemsSource = steps;

            LoadDataFromTemporarySaving();
            StepTextBox.Text = "";
            UpdatePlaceholderVisibility();
            UpdateHoursPlaceholderVisibility();
            UpdateMinutesPlaceholderVisibility();
        }

        //-- Подгружаем данные из временного хранилища, если таковы есть
        void LoadDataFromTemporarySaving()
        {
            if (recipe != null)
            {
                RecipeNameText.Text = (!String.IsNullOrEmpty(recipe.Title) ?
               recipe.Title : "");

                if (recipe.Steps != null)
                    foreach(var  step in recipe.Steps)
                    {
                        steps.Add( new StepItem() { Description = $"Шаг {step.StepNumber}: " + step.Description, 
                            ImagePath = step.PhotoPath});
                    }

                var time = CookingTimeMethods.ConvertingTimeMinute(recipe.CookingTime);
                HoursTextBox.Text = $"{((time.hour == 0) ? "" : time.hour)}";
                MinutesTextBox.Text = $"{((time.minute == 0) ? "" : time.minute)}";
            }
            else
            {
                RecipeNameText.Text = "Ошибка получения рецепта";
            }
            
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

        //-- Удаление шага
        private void DeleteSelectedStep_Click(object sender, RoutedEventArgs e)
        {
            if (StepsListBox.SelectedItem != null)
            {
                StepItem step = (StepItem)StepsListBox.SelectedItem;

                var result = MessageBox.Show($"Удалить шаг?\n{step.Description}",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    steps.Remove(step);

                    Regex regex = new Regex(@"^Шаг\s*\d+\s*:\s*");
                    List<StepItem> items = new List<StepItem>(steps);
                    steps.Clear();
                    for (int i = 0; i < items.Count; i++)
                    {
                        steps.Add(new StepItem
                        {
                            Description = regex.Replace(items[i].Description, $"Шаг {i + 1}: "),
                            ImagePath = items[i].ImagePath
                        });
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите шаг левой кнопкой", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                steps.Add(new StepItem() { Description = $"Шаг {steps.Count + 1}: {StepTextBox.Text}", ImagePath = pathImage} );
                ViewImage.Source = null;
                pathImage = null;

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
                Save();
                parent.Content = new NewReceptControl(recipe);
            }
        }

        //-- Перед переходом сохраняем данные
        void Save()
        {
            if (recipe != null)
            {
                recipe.Steps = new List<Step>();
                Regex regex = new Regex(@"Шаг\s*(\d+)\s*:\s*(.*)");
                foreach (var item in steps)
                {
                    Match match = regex.Match(item.Description);
                    if (match.Success)
                    {
                        Step step = new Step()
                        {
                            StepNumber = int.Parse(match.Groups[1].Value),
                            Description = match.Groups[2].Value,
                            PhotoPath = item.ImagePath,
                            Recipe = recipe
                        };
                        recipe.Steps.Add(step);
                    }
                }
                string stringTime = FormatCookingTime();
                recipe.CookingTime = CookingTimeMethods.ConvertingTimeFromStringToMinutes(stringTime);
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

            try
            {
                Save();
                NewAndUpdateRecipe.SaveRecipe(recipe);

                MessageBox.Show("Рецепт успешно сохранен!",
                "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);

                OnRecipeSaved?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления рецепта {ex.Message}", "Ошибка",
                   MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }

        private void AddStepImageButton_Click(object sender, RoutedEventArgs e)
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

                pathImage = filePath;
            }
        }
    }
}