namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class NlpFormSynonyms
   {
      public virtual Guid FormSynonymId { get; set; }

      public virtual Ingredients Ingredient { get; set; }

      public virtual IngredientForms Form { get; set; }

      public virtual string Name { get; set; }
   }

   public class NlpFormSynonymsMap : ClassMap<NlpFormSynonyms>
   {
      public NlpFormSynonymsMap()
      {
          this.Id(x => x.FormSynonymId)
            .GeneratedBy.GuidComb()
            .UnsavedValue(Guid.Empty);

          this.Map(x => x.Name).Length(50).UniqueKey("FormName");

          this.References(x => x.Ingredient).Not.Nullable().UniqueKey("FormName");
          this.References(x => x.Form).Not.Nullable();
      }
   }
}