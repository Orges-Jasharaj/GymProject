using GymProject.NutritionService.Models;
using Microsoft.EntityFrameworkCore;

namespace GymProject.NutritionService.Data
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        {
        }

        public DbSet<Meals> Meals { get; set; }
        public DbSet<NutritionPlan> NutritionPlans { get; set; }
        public DbSet<PlanMeals> PlanMeals { get; set; }
    }
}