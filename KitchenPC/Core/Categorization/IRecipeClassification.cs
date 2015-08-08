namespace KitchenPC.Categorization
{
    using KitchenPC.Recipes;

    public interface IRecipeClassification
    {
        Recipe Recipe { get; }

        bool IsBreakfast { get; }

        bool IsLunch { get; }

        bool IsDinner { get; }

        bool IsDessert { get; }
    }
}