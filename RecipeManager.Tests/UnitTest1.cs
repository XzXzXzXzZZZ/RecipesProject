using Microsoft.EntityFrameworkCore;
using RecipesProject.Data;
using RecipesProject.Models;

namespace RecipeManager.Tests
{
    [TestFixture]
    public class Tests
    {
        private DBContext _dBContext;
        private RecipeRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                        .UseSqlServer("Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False")
                        .Options;

            _dBContext = new DBContext(options);
            _dBContext.Database.EnsureDeleted();
            _dBContext.Database.EnsureCreated();
            _repo = new RecipeRepository(_dBContext);
        }

        [Test]
        public void AddTestRecipe()
        {
            var recipe = new Recipe()
            {
                Title = "Test",
                CookingTime = 5
            };

            _repo.Add(recipe);
            var result = _repo.GetById(recipe.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(recipe.Title, result.Title);
        }

        [Test]
        public void SearchByTitleRecipe()
        {
            _repo.Add(new Recipe()
            {
                Title = "Борщ",
                CookingTime = 65
            });
           _repo.Add(new Recipe()
            {
                Title = "Блинчики",
                CookingTime = 105
            });

            var results = _repo.SearchByTitle("борщ");

            Assert.IsTrue(results.Any(r => r.Title.ToLower().Contains("борщ")));
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void DeleteRecipe()
        {
            var recipe = new Recipe()
            {
                Title = "Удаляемый",
                CookingTime = 65
            };

            _repo.Add(recipe);
            _repo.Delete(recipe.Id);
            var result = _repo.GetById(recipe.Id);

            Assert.IsNull(result);
        }

        [Test]
        public void FridgeSearch_ShouldFindRecipeByIngredients()
        {
            var productRepo = new ProductRepository(_dBContext);

            var recipe = new Recipe { Title = "Омлет", CookingTime = 15 };
            recipe.Ingredients.Add(new Ingredient { Text = "Яйца" });
            recipe.Ingredients.Add(new Ingredient { Text = "Молоко" });
            _repo.Add(recipe);

            productRepo.AddMyProduct("Яйца", false);
            productRepo.AddMyProduct("Молоко", false);

            var fridgeProducts = productRepo.GetAllMyProducts();
            var results = _repo.FindByIngredients(fridgeProducts.Select(p=>p.Name).ToList());

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Омлет", results[0].Title);
        }

        [TearDown]
        public void TearDown()
        {
            _dBContext?.Dispose();
        }
    }
}