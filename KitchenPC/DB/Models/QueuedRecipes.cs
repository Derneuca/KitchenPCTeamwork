namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class QueuedRecipes
    {
        public virtual Guid QueueId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual DateTime QueuedDate { get; set; }
    }

    public class QueuedRecipesMap : ClassMap<QueuedRecipes>
    {
        public QueuedRecipesMap()
        {
            this.Id(x => x.QueueId)
              .GeneratedBy.GuidComb()
              .UnsavedValue(Guid.Empty);

            this.Map(x => x.UserId).Not.Nullable().Index("IDX_QueuedRecipes_UserId").UniqueKey("UniqueRecipe");
            this.Map(x => x.QueuedDate).Not.Nullable();

            this.References(x => x.Recipe).Not.Nullable().UniqueKey("UniqueRecipe");
        }
    }
}