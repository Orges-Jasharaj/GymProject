using GymProject.SubscriptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace GymProject.SubscriptionService.Data
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
        }
        
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<SubscriptionPlan> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }

    }
}
