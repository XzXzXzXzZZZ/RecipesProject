using System.Windows;
using RecipesProject.UI.FavRecepts;
using RecipesProject.UI.NewRecepts;
using RecipesProject.UI.MainMenu;
namespace RecipesProject.UI.AllRecepts
{
    /// <summary>
    /// Логика взаимодействия для WindowAllRecept.xaml
    /// </summary>
    public partial class WindowAllRecept : Window
    {
        public WindowAllRecept()
        {
            InitializeComponent();
        }
        private void FavoriteBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowFavRecept windowFavRecept = new WindowFavRecept();
            windowFavRecept.Show();
            this.Close();
        }

        private void NewReceptBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowNewRecept windowNewRecept = new WindowNewRecept();
            windowNewRecept.Show();
            this.Close();
        }

        private void MainMenuBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowMain windowMain = new WindowMain();
            windowMain.Show();
            this.Close();
        }
    }
    
}
