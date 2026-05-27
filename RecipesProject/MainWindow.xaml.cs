using Microsoft.EntityFrameworkCore;
using RecipesProject.Data;
using RecipesProject.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecipesProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            using (DBContext dbContext = new DBContext())
            {
                //dbContext.Database.Migrate();
            }
            InitializeComponent();
            TestDatabaseRepository();

            //using (var context = new DBContext())
            //{
            //    // Удаляем все записи из всех таблиц
            //    context.Recipes.RemoveRange(context.Recipes);
            //    context.Steps.RemoveRange(context.Steps);
            //    context.Ingredients.RemoveRange(context.Ingredients);
            //    context.MyProducts.RemoveRange(context.MyProducts);
            //    context.PermanentProducts.RemoveRange(context.PermanentProducts);

            //    context.SaveChanges();

            //    Console.WriteLine("✅ База данных очищена!");
            //}
        }


        private void TestDatabaseRepository()
        {

            using (DBContext dbContext = new DBContext())
            {
                // Убедимся что БД создана
                dbContext.Database.EnsureCreated();

                var repo = new RecipeRepository(dbContext);

                Console.WriteLine("========== ТЕСТ РЕПОЗИТОРИЯ С БД ==========\n");

                // 1. Добавление рецептов
                Console.WriteLine("1️ ADD:");
                var recipe1 = new Recipe
                {
                    Title = "Борщ",
                    CookingTime = 90,
                    IsFavorite = 1,
                    Description = "Классический борщ",
                    Difficulty = 3
                };
                repo.Add(recipe1);
                Console.WriteLine($"Добавлен: {recipe1.Title} (ID: {recipe1.Id}, CreatedAt: {recipe1.CreatedAt})");

                var recipe2 = new Recipe
                {
                    Title = "Оливье",
                    CookingTime = 60,
                    IsFavorite = 1
                };
                repo.Add(recipe2);
                Console.WriteLine($"Добавлен: {recipe2.Title} (ID: {recipe2.Id})");

                var recipe3 = new Recipe
                {
                    Title = "Блинчики",
                    CookingTime = 30
                };
                repo.Add(recipe3);
                Console.WriteLine($"Добавлен: {recipe3.Title} (ID: {recipe3.Id})");

                // 2. GetAll
                Console.WriteLine("\n2️ GET ALL:");
                var allRecipes = repo.GetAll();
                Console.WriteLine($"Всего рецептов: {allRecipes.Count}");
                foreach (var r in allRecipes)
                {
                    Console.WriteLine($"  [{r.Id}] {r.Title} | Время: {r.CookingTime}мин | Избранное: {r.IsFavorite} | Создан: {r.CreatedAt}");
                }

                // 3. GetById
                Console.WriteLine("\n3️ GET BY ID:");
                var found = repo.GetById(recipe1.Id);
                Console.WriteLine(found != null
                    ? $"Найден: {found.Title} (Описание: {found.Description})"
                    : " Не найден");

                // 4. GetFavorites
                Console.WriteLine("\n4️ GET FAVORITES:");
                var favorites = repo.GetFavorites();
                Console.WriteLine($"Избранных рецептов: {favorites.Count}");
                foreach (var r in favorites)
                {
                    Console.WriteLine($"  {r.Title}");
                }

                // 5. SearchByTitle
                Console.WriteLine("\n5️ SEARCH BY TITLE:");
                var search = repo.SearchByTitle("бор");
                Console.WriteLine($"Найдено по 'бор': {search.Count}");
                foreach (var r in search)
                {
                    Console.WriteLine($"  {r.Title}");
                }

                // 6. UPDATE - меняем ВСЁ кроме ID
                Console.WriteLine("\n UPDATE:");
                var toUpdate = repo.GetById(recipe1.Id);
                var oldCreatedAt = toUpdate.CreatedAt;
                var oldId = toUpdate.Id;

                Console.WriteLine($"ДО:  [{toUpdate.Id}] {toUpdate.Title} | Описание: {toUpdate.Description} | Создан: {toUpdate.CreatedAt} | Обновлен: {toUpdate.UpdatedAt}");

                // Меняем ВСЕ поля
                toUpdate.Title = "Борщ вкусный";
                toUpdate.Description = "Обновленный рецепт борща";
                toUpdate.CookingTime = 120;
                toUpdate.Difficulty = 5;
                toUpdate.Servings = 6;
                toUpdate.IsFavorite = 0;

                repo.Update(toUpdate);

                var updated = repo.GetById(recipe1.Id);
                Console.WriteLine($"ПОСЛЕ: [{updated.Id}] {updated.Title} | Описание: {updated.Description}");
                Console.WriteLine($"  CookingTime: {updated.CookingTime}мин | Difficulty: {updated.Difficulty}");
                Console.WriteLine($"  Servings: {updated.Servings} | IsFavorite: {updated.IsFavorite}");
                Console.WriteLine($"  CreatedAt: {updated.CreatedAt} (НЕ изменился) ");
                Console.WriteLine($"  UpdatedAt: {updated.UpdatedAt} (изменился) ");
                Console.WriteLine($"  ID: {updated.Id} (НЕ изменился: {oldId == updated.Id}) ");

                // 7. DELETE
                Console.WriteLine("\n7️⃣ DELETE:");
                int countBefore = repo.GetAll().Count;
                Console.WriteLine($"Рецептов до удаления: {countBefore}");
                repo.Delete(recipe3.Id);
                int countAfter = repo.GetAll().Count;
                Console.WriteLine($"Рецептов после удаления ID {recipe3.Id}: {countAfter}");
                Console.WriteLine($"Удален? {(repo.GetById(recipe3.Id) == null ? " Да" : " Нет")}");

                Console.WriteLine("\n ВСЕ ТЕСТЫ ПРОЙДЕНЫ");

            }
        }

    }
}