namespace KitchenPC.NLP
{
    using System.Collections.Generic;

    public interface ISynonymLoader<T>
    {
        IEnumerable<T> LoadSynonyms();

        Pairings LoadFormPairings(); //Default pairing data for forms of certain ingredients
    }
}