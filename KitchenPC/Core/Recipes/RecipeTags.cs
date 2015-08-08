namespace KitchenPC.Recipes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class RecipeTags : IEnumerable<RecipeTag>
    {
        private readonly int mask; // Bitmask value, where each bit indicates the tag in that ordinal position
        private readonly List<RecipeTag> tags; // Enumerable list of RecipeTag objects
         
        public RecipeTags()
        {
            this.tags = new List<RecipeTag>(13);
            this.mask = 0;
        }

        private RecipeTags(int mask)
        {
            this.mask = mask;
            this.tags = new List<RecipeTag>(13);

            this.AddTags();
        }

        public static RecipeTags None
        {
            get
            {
                return 0;
            }
        }

        public int Length
        {
            get
            {
                return this.tags.Count;
            }
        }

        public RecipeTag this[int index]
        {
            get
            {
                return this.tags[index];
            }
        }
        
        public static RecipeTags Parse(string list)
        {
            var tags = from t in list.Split(',') 
                       select t.Trim();
            var result = new RecipeTags();
            result = tags.Aggregate(result, (current, tag) => current | (RecipeTag)tag);

            return result;
        }
        
        public static RecipeTags From(params RecipeTag[] tags)
        {
            var result = new RecipeTags();
            result.tags.AddRange(tags);

            return result;
        }
        
        public static RecipeTags operator |(RecipeTags firstTag, RecipeTag secondTag)
        {
            var result = firstTag.mask | secondTag.BitFlag;
            return result;
        }

        public static int operator &(RecipeTags firstTag, RecipeTag secondTag)
        {
            int result = firstTag.mask & secondTag.BitFlag;
            return result;
        }

        public static bool operator !=(RecipeTags firstTag, RecipeTags secondTag)
        {
            bool result = !(firstTag == secondTag);
            return result;
        }

        public static bool operator ==(RecipeTags firstTag, RecipeTags secondTag)
        {
            if (ReferenceEquals(firstTag, secondTag))
            {
                return true;
            }

            if ((object)firstTag == null || ((object)secondTag == null))
            {
                return false;
            }

            bool result = firstTag.mask == secondTag.mask;
            return result;
        }

        public static implicit operator int(RecipeTags tags)
        {
            int result = tags.mask;
            return result;
        }

        public static implicit operator RecipeTags(int value)
        {
            var result = new RecipeTags(value);
            return result;
        }

        public bool HasTag(RecipeTag tag)
        {
            bool result = (this & tag) > 0;
            return result;
        }

        public override bool Equals(object obj)
        {
            var tags = obj as RecipeTags;
            bool result = tags != null && this.mask == tags.mask;
            return result;
        }
        
        public override string ToString()
        {
            var labels = (from n in this.tags select n.ToString()).ToArray();
            return string.Join(", ", labels);
        }

        public override int GetHashCode()
        {
            return this.mask;
        }

        public IEnumerator<RecipeTag> GetEnumerator()
        {
            return this.tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tags.GetEnumerator();
        }

        //Required to serialize class
        public void Add(object obj)
        {
            throw new NotImplementedException();
        }

        private void AddTags()
        {
            if ((this & RecipeTag.GlutenFree) > 0)
            {
                this.tags.Add(RecipeTag.GlutenFree);
            }

            if ((this & RecipeTag.NoAnimals) > 0)
            {
                this.tags.Add(RecipeTag.NoAnimals);
            }

            if ((this & RecipeTag.NoMeat) > 0)
            {
                this.tags.Add(RecipeTag.NoMeat);
            }

            if ((this & RecipeTag.NoPork) > 0)
            {
                this.tags.Add(RecipeTag.NoPork);
            }

            if ((this & RecipeTag.NoRedMeat) > 0)
            {
                this.tags.Add(RecipeTag.NoRedMeat);
            }

            if ((this & RecipeTag.Breakfast) > 0)
            {
                this.tags.Add(RecipeTag.Breakfast);
            }

            if ((this & RecipeTag.Dessert) > 0)
            {
                this.tags.Add(RecipeTag.Dessert);
            }

            if ((this & RecipeTag.Dinner) > 0)
            {
                this.tags.Add(RecipeTag.Dinner);
            }

            if ((this & RecipeTag.Lunch) > 0)
            {
                this.tags.Add(RecipeTag.Lunch);
            }

            if ((this & RecipeTag.LowCalorie) > 0)
            {
                this.tags.Add(RecipeTag.LowCalorie);
            }

            if ((this & RecipeTag.LowCarb) > 0)
            {
                this.tags.Add(RecipeTag.LowCarb);
            }

            if ((this & RecipeTag.LowFat) > 0)
            {
                this.tags.Add(RecipeTag.LowFat);
            }

            if ((this & RecipeTag.LowSodium) > 0)
            {
                this.tags.Add(RecipeTag.LowSodium);
            }

            if ((this & RecipeTag.LowSugar) > 0)
            {
                this.tags.Add(RecipeTag.LowSugar);
            }

            if ((this & RecipeTag.Common) > 0)
            {
                this.tags.Add(RecipeTag.Common);
            }

            if ((this & RecipeTag.Easy) > 0)
            {
                this.tags.Add(RecipeTag.Easy);
            }

            if ((this & RecipeTag.Quick) > 0)
            {
                this.tags.Add(RecipeTag.Quick);
            }
        }
    }
}