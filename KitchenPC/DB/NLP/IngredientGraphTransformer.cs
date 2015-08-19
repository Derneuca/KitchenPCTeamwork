namespace KitchenPC.DB
{
    using System;
    using System.Collections;
    using Modeler;
    using NHibernate.Transform;

    public class IngredientGraphTransformer : IResultTransformer
    {
        public IngredientGraphTransformer()
        {
        }

        public static IngredientGraphTransformer Create()
        {
            return new IngredientGraphTransformer();
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return IngredientBinding.Create(
                (Guid)tuple[0], // R.IngredientId
                (Guid)tuple[1], // R.RecipeId
                (float?)tuple[2], // R.Qty
                (Units)tuple[3], // R.Unit
                (UnitType)tuple[4], // I.ConversionType
                (int)tuple[5], // I.UnitWeight
                (Units)tuple[6], // F.UnitType
                (float)tuple[7], // F.FormAmount
                (Units)tuple[8]); // F.FormUnit
        }
    }
}