using RecipesProject.Models;
using System.Windows;
using System.Windows.Controls;

namespace RecipesProject.UI.AllRecepts
{
    public partial class AllReceptsControl : UserControl
    {
        //-- Объявляем событие, которое будет вызвано при двойном клике
        public event EventHandler<int> RecipeSelected;

        public AllReceptsControl()
        {
            InitializeComponent();
        }

        //-- Обработчик нажатия на элемент ListBox
        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedRecipe = (ListBoxItem)sender;
            if (selectedRecipe != null)
            {
                if(int.TryParse(selectedRecipe.Tag.ToString(), out int recipeId))
                {
                    //-- Вызываем событие, чтобы главное окно знало о выборе
                    RecipeSelected?.Invoke(this, recipeId);
                }            
            }
        }
    }
}