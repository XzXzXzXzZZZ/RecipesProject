using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RecipesProject.UI.ViewingRecipe
{
    /// <summary>
    /// Логика взаимодействия для StepReceptControl.xaml
    /// </summary>
    public partial class StepReceptControl : UserControl
    {
        public StepReceptControl(Step step)
        {
            InitializeComponent();
            loadInfoStep(step);
        }

        void loadInfoStep(Step step)
        {
            NumberStep.Content += step.StepNumber.ToString();
            ViewImage.Source = ImageMethods.readImage(step.PhotoPath);
            DescriptionStep.Text = step.Description;
        }

        public void Cleanup()
        {
            // Очищаем изображение шага, если оно есть
            if (ViewImage != null)
            {
                ViewImage.Source = null;
            }
        }
    }
}
