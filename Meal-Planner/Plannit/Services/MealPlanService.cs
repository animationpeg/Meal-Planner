using Microsoft.EntityFrameworkCore;
using Plannit.Data;
using Plannit.Enums;
using Plannit.Models;

namespace Plannit.Services
{
    /// <summary>
    /// Service for Meal Plan database operations
    /// </summary>
    public class MealPlanService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        #region Get Methods
        /// <summary>
        /// Retrieve all Meal Plans
        /// </summary>
        /// <returns>A list of MealPlans</returns>
        public async Task<List<MealPlan>> GetAllMealPlansAsync()
        {
            return await _context.MealPlans
                .OrderByDescending(mp => mp.IsActive)
                .ThenByDescending(mp => mp.WeekStarting)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieve the active meal plan
        /// </summary>
        /// <returns>The first active meal plan found</returns>
        public async Task<MealPlan?> GetActiveMealPlanAsync()
        {
            return await _context.MealPlans
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .FirstOrDefaultAsync(mp => mp.IsActive);
        }

        /// <summary>
        /// Retrieve a single meal plan based on a given Guid id
        /// </summary>
        /// <param name="id">The id of the meal plan</param>
        /// <returns>The first meal plan found matching the given id</returns>
        public async Task<MealPlan?> GetMealPlanByIdAsync(Guid id)
        {
            return await _context.MealPlans
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }

        /// <summary>
        /// Retrieve the active meal plan and ingredients details for shopping list generation
        /// </summary>
        /// <returns>The first active meal plan found</returns>
        public async Task<MealPlan?> GetActiveMealPlanWithIngredientsAsync()
        {
            return await _context.MealPlans
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .ThenInclude(r => r.Ingredients)
                .FirstOrDefaultAsync(mp => mp.IsActive);
        }

        #endregion

        #region Command methods
        /// <summary>
        /// Adds a new meal plan to the database
        /// </summary>
        /// <param name="mealPlan">The meal plan to save</param>
        public async Task CreateMealPlanAsync(MealPlan mealPlan)
        {
            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update data on an already existing meal plan
        /// </summary>
        /// <param name="mealPlan">The meal plan entity to update</param>
        public async Task UpdateMealPlanAsync(MealPlan mealPlan)
        {
            _context.MealPlans.Update(mealPlan);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete a meal plan from the database
        /// </summary>
        /// <param name="id">The Guid id of the meal plan to delete</param>
        public async Task DeleteMealPlanAsync(Guid id)
        {
            MealPlan? mealPlan = await _context.MealPlans.FindAsync(id);
            if (mealPlan is not null)
            {
                _context.MealPlans.Remove(mealPlan);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Set the given meal plan as active, and set all others to inactive
        /// </summary>
        /// <param name="id">The meal plan id to set active</param>
        public async Task SetActivePlanAsync(Guid id)
        {
            List<MealPlan> allPlans = await _context.MealPlans.ToListAsync();
            foreach (MealPlan plan in allPlans)
            {
                plan.IsActive = plan.Id == id;
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Add a recipe to a slot in a meal plan
        /// </summary>
        /// <param name="mealPlanId">The meal plan id to add to</param>
        /// <param name="day">The day for the entry</param>
        /// <param name="mealSlot">The meal slot for the entry</param>
        /// <param name="recipeId">The recipe id to add</param>
        public async Task AddEntryAsync(Guid mealPlanId, int day, MealSlot mealSlot, Guid recipeId)
        {
            MealPlanEntry entry = new MealPlanEntry
            {
                MealPlanId = mealPlanId,
                Day = day,
                MealSlot = mealSlot,
                RecipeId = recipeId
            };
            _context.MealPlanEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove an entry from a meal plan
        /// </summary>
        /// <param name="entryId">The entry guid id to remove</param>
        public async Task RemoveEntryAsync(Guid entryId)
        {
            MealPlanEntry? entry = await _context.MealPlanEntries.FindAsync(entryId);
            if (entry is not null)
            {
                _context.MealPlanEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
