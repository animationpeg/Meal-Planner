using Microsoft.EntityFrameworkCore;
using Plannit.Data;
using Plannit.Models;

namespace Plannit.Services
{
    /// <summary>
    /// Service for recipe-related database queries
    /// </summary>
    public class RecipeService(IDbContextFactory<AppDbContext> contextFactory)
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory = contextFactory;

        #region Get Methods
        /// <summary>
        /// Retrieve all recipes, including their Ingredients and MethodSteps
        /// </summary>
        /// <returns>A list of all recipes found</returns>
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.MethodSteps.OrderBy(s => s.StepNumber))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieve a single recipe by it's Id, including it's Ingredients and MethodSteps
        /// </summary>
        /// <param name="id">The unique id of the Recipe</param>
        /// <returns>The first recipe found with the given id</returns>
        public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.MethodSteps.OrderBy(s => s.StepNumber))
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Add a recipe to the database
        /// </summary>
        /// <param name="recipe">The recipe entity to add</param>
        public async Task AddRecipeAsync(Recipe recipe)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();

            foreach (Ingredient ingredient in recipe.Ingredients)
            {
                ingredient.RecipeId = recipe.Id;
            }

            foreach (MethodStep step in recipe.MethodSteps)
            {
                step.RecipeId = recipe.Id;
            }

            RenumberSteps(recipe);
            context.Recipes.Add(recipe);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Update the record of a recipe already in the database
        /// </summary>
        /// <param name="recipe">The recipe entity to update</param>
        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();

            Recipe? existing = await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.MethodSteps)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            if (existing is null)
            {
                return;
            }

            context.Entry(existing).CurrentValues.SetValues(recipe);

            foreach (Ingredient existingIngredient in existing.Ingredients.ToList())
            {
                if (!recipe.Ingredients.Any(i => i.Id == existingIngredient.Id))
                {
                    context.Ingredients.Remove(existingIngredient);
                }
            }

            foreach (Ingredient ingredient in recipe.Ingredients)
            {
                Ingredient? existingIngredient = existing.Ingredients
                    .FirstOrDefault(i => i.Id == ingredient.Id);

                if (existingIngredient is null)
                {
                    ingredient.RecipeId = recipe.Id;
                    existing.Ingredients.Add(ingredient);
                    context.Entry(ingredient).State = EntityState.Added;
                }
                else
                {
                    context.Entry(existingIngredient).CurrentValues.SetValues(ingredient);
                }
            }

            foreach (MethodStep existingStep in existing.MethodSteps.ToList())
            {
                if (!recipe.MethodSteps.Any(s => s.Id == existingStep.Id))
                {
                    context.MethodSteps.Remove(existingStep);
                }
            }

            foreach (MethodStep step in recipe.MethodSteps)
            {
                MethodStep? existingStep = existing.MethodSteps
                    .FirstOrDefault(s => s.Id == step.Id);

                if (existingStep is null)
                {
                    step.RecipeId = recipe.Id;
                    existing.MethodSteps.Add(step);
                    context.Entry(step).State = EntityState.Added;
                }
                else
                {
                    context.Entry(existingStep).CurrentValues.SetValues(step);
                }
            }

            RenumberSteps(existing);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete a recipe record from the database by id
        /// </summary>
        /// <param name="id">The id of the recipe to be deleted</param>
        public async Task DeleteRecipeAsync(Guid id)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            Recipe? recipe = await context.Recipes.FindAsync(id);
            if (recipe is not null)
            {
                context.Recipes.Remove(recipe);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Add a new ingredient to a given recipe
        /// </summary>
        /// <param name="recipe">The Recipe entity</param>
        public void AddIngredient(Recipe recipe)
        {
            recipe.Ingredients.Add(new Ingredient());
        }

        /// <summary>
        /// Remove an ingredient from a given recipe
        /// </summary>
        /// <param name="recipe">The Recipe entity</param>
        /// <param name="ingredient">The Ingredient entity</param>
        public void RemoveIngredient(Recipe recipe, Ingredient ingredient)
        {
            recipe.Ingredients.Remove(ingredient);
        }

        /// <summary>
        /// Add a new method step to a given recipe
        /// </summary>
        /// <param name="recipe">The Recipe entity</param>
        public void AddStep(Recipe recipe)
        {
            int nextStepNumber = recipe.MethodSteps.Count + 1;
            recipe.MethodSteps.Add(new MethodStep { StepNumber = nextStepNumber });
        }

        /// <summary>
        /// Remove method step from a given recipe
        /// </summary>
        /// <param name="recipe">The Recipe entity</param>
        /// <param name="step">The MethodStep entity to remove</param>
        public void RemoveStep(Recipe recipe, MethodStep step)
        {
            recipe.MethodSteps.Remove(step);
        }
        #endregion

        /// <summary>
        /// Private method for updating the order of method steps
        /// </summary>
        /// <param name="recipe">The recipe entity whose method steps need renumbering</param>
        private static void RenumberSteps(Recipe recipe)
        {
            int stepNumber = 1;
            foreach (MethodStep step in recipe.MethodSteps)
            {
                step.StepNumber = stepNumber++;
            }
        }
    }
}
