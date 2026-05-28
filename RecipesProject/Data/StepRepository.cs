using RecipesProject.Models;

namespace RecipesProject.Data
{
    public class StepRepository
    {
        private readonly DBContext _context;

        public StepRepository(DBContext context)
        {
            _context = context;
        }

        public void Add(Step step)
        {
            _context.Steps.Add(step);
            _context.SaveChanges();
        }

        public void AddRange(List<Step> steps)
        {
            _context.Steps.AddRange(steps);
            _context.SaveChanges();
        }
    }
}
