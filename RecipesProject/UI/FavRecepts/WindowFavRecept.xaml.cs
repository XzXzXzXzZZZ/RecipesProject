using System.Windows;
using RecipesProject.UI.NewRecepts;
using RecipesProject.UI.MainMenu;
using RecipesProject.UI.AllRecepts;
namespace RecipesProject.UI.FavRecepts
{
    public partial class WindowFavRecept : Window
    {
        public WindowFavRecept()
        {
            InitializeComponent();
        }
        private void MainMenuBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowMain windowMain = new WindowMain();
            windowMain.Show();
            this.Close();
        }
        private void NewReceptBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowNewRecept windowNewRecept = new WindowNewRecept();
            windowNewRecept.Show();
            this.Close();
        }

        private void AllReceptBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowAllRecept windowAllRecept= new WindowAllRecept();
            windowAllRecept.Show();
            this.Close();
        }
    }
}
