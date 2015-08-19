namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class ShoppingListsMap : ClassMap<ShoppingLists>
    {
        public ShoppingListsMap()
        {
            this.Id(x => x.ShoppingListId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.UserId).Not.Nullable().Index("IDX_ShoppingLists_UserId").UniqueKey("UniqueTitle");
            this.Map(x => x.Title).Not.Nullable().UniqueKey("UniqueTitle");

            this.HasMany(x => x.Items)
                .KeyColumn("ShoppingListId")
                .Inverse()
                .Cascade.All();
        }
    }
}