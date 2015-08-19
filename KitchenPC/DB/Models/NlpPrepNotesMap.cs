namespace KitchenPC.DB.Models
{
    using FluentNHibernate.Mapping;

    public class NlpPrepNotesMap : ClassMap<NlpPrepNotes>
    {
        public NlpPrepNotesMap()
        {
            this.Id(x => x.Name).Length(50);
        }
    }
}