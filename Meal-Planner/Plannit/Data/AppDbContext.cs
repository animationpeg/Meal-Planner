using Microsoft.EntityFrameworkCore;
using Plannit.Models;

namespace Plannit.Data
{
    /// <summary>
    /// Application Db Context - for EF building
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        #region Database Sets
        /// <summary>
        /// The Recipes table in the database
        /// </summary>
        public DbSet<Recipe> Recipes { get; set; }
        /// <summary>
        /// The Ingredients table in the database
        /// </summary>
        public DbSet<Ingredient> Ingredients { get; set; }
        /// <summary>
        /// The MethodSteps table in the database
        /// </summary>
        public DbSet<MethodStep> MethodSteps { get; set; }
        /// <summary>
        /// The MealPlans table in the database
        /// </summary>
        public DbSet<MealPlan> MealPlans { get; set; }
        /// <summary>
        /// The MealPlanEntries table in the database
        /// </summary>
        public DbSet<MealPlanEntry> MealPlanEntries { get; set; }
        /// <summary>
        /// The ShoppingLists table in the database
        /// </summary>
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        /// <summary>
        /// The ShoppingListItems table in the database
        /// </summary>
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
        #endregion
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Description).IsRequired(false);
                entity.Property(r => r.Servings).IsRequired(false);

                entity.HasMany(r => r.Ingredients)
                      .WithOne(i => i.Recipe)
                      .HasForeignKey(i => i.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(r => r.MethodSteps)
                      .WithOne(s => s.Recipe)
                      .HasForeignKey(s => s.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Name).IsRequired().HasMaxLength(200);
                entity.Property(i => i.Quantity).IsRequired(false);
                entity.Property(i => i.Unit).HasConversion<string>();

                entity.HasOne(i => i.Recipe)
                      .WithMany(r => r.Ingredients)
                      .HasForeignKey(i => i.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MethodStep>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.StepNumber).IsRequired();
                entity.Property(s => s.Instruction).IsRequired();

                entity.HasOne(s => s.Recipe)
                      .WithMany(r => r.MethodSteps)
                      .HasForeignKey(s => s.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MealPlan>(entity =>
            {
                entity.HasKey(mp => mp.Id);
                entity.Property(mp => mp.Name).IsRequired().HasMaxLength(200);
                entity.Property(mp => mp.WeekStarting).IsRequired();
                entity.Property(mp => mp.DurationDays).IsRequired();
                entity.Property(mp => mp.IsActive).IsRequired();

                entity.HasMany(mp => mp.Entries)
                      .WithOne(e => e.MealPlan)
                      .HasForeignKey(e => e.MealPlanId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MealPlanEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Day).IsRequired();
                entity.Property(e => e.MealSlot).HasConversion<string>();

                entity.HasOne(e => e.MealPlan)
                      .WithMany(mp => mp.Entries)
                      .HasForeignKey(e => e.MealPlanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Recipe)
                      .WithMany()
                      .HasForeignKey(e => e.RecipeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShoppingList>(entity =>
            {
                entity.HasKey(sl => sl.Id);

                entity.HasOne(sl => sl.MealPlan)
                      .WithOne(mp => mp.ShoppingList)
                      .HasForeignKey<ShoppingList>(sl => sl.MealPlanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(sl => sl.Items)
                      .WithOne(i => i.ShoppingList)
                      .HasForeignKey(i => i.ShoppingListId)
                      .OnDelete(DeleteBehavior.Cascade); ;
            });

            modelBuilder.Entity<ShoppingListItem>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Name).IsRequired().HasMaxLength(200);
                entity.Property(i => i.Quantity).IsRequired(false);
                entity.Property(i => i.Unit).HasConversion<string>();
                entity.Property(i => i.IsChecked).IsRequired();
                entity.Property(i => i.Order).IsRequired();
            });
        }
    }
}
