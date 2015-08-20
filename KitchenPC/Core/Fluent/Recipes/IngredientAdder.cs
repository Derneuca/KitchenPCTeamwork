namespace KitchenPC.Fluent.Recipes
{
    using System;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Exceptions;
    using KitchenPC.Ingredients;
    using KitchenPC.NLP;
    using KitchenPC.Recipes;
    using KitchenPC.Recipes.Enums;

    public class IngredientAdder
    {
        private readonly IKPCContext context;
        private readonly Recipe recipe;
        private readonly string section;

        public IngredientAdder(IKPCContext context, Recipe recipe)
        {
            this.context = context;
            this.recipe = recipe;
        }

        public IngredientAdder(IKPCContext context, Recipe recipe, string section)
            : this(context, recipe)
        {
            this.section = section;
        }

        public IngredientAdder AddIngredientUsage(IngredientUsage usage)
        {
            if (!string.IsNullOrWhiteSpace(this.section))
            {
                usage.Section = this.section;
            }

            this.recipe.AddIngredient(usage);
            return this;
        }

        public IngredientAdder AddIngredientUsage(Func<IngredientUsageCreator, IngredientUsageCreator> createAction)
        {
            var creator = createAction(IngredientUsage.Create);
            var usage = creator.Usage;

            // Validate Ingredient
            usage.Ingredient = this.context.ReadIngredient(usage.Ingredient.Id);

            // Verify form
            if (usage.Form != null)
            {
                var forms = this.context.ReadFormsForIngredient(usage.Ingredient.Id).Forms;
                var validatedForm = forms.FirstOrDefault(f => f.FormId == usage.Form.FormId);
                if (validatedForm == null)
                {
                    throw new InvalidFormException(usage.Ingredient, usage.Form);
                }

                usage.Form = validatedForm;
            }

            var result = this.AddIngredientUsage(usage);
            return result;
        }

        public IngredientAdder AddIngredient(Ingredient ingredient, Amount amount, string prepNote = null)
        {
            var usage = new IngredientUsage(ingredient, null, amount, prepNote);
            var result = this.AddIngredientUsage(usage);
            return result;
        }

        public IngredientAdder AddIngredient(Ingredient ingredient, string prepNote = null)
        {
            var result = this.AddIngredient(ingredient, null, prepNote);
            return result;
        }

        public IngredientAdder AddRaw(string raw)
        {
            var result = this.context.ParseIngredientUsage(raw);
            if (!(result is Match))
            {
                throw new CouldNotParseUsageException(result, raw);
            }

            return this.AddIngredientUsage(result.Usage);
        }
    }
}
