namespace KitchenPC.Modeler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #region Support classes for RatingGraph

    internal partial class RatingGraph
    {
        private class RecipeNode
        {
            internal Guid Key { get; set; }

            internal List<Rating> Ratings { get; set; }
        }

        private class UserNode
        {
            internal Guid Key { get; set; }

            internal List<Rating> Ratings { get; set; }
        }

        private class Rating
        {
            internal RecipeNode Recipe { get; set; }

            internal UserNode User { get; set; }

            internal short Weight { get; set; }
        }
    }

    #endregion

    internal partial class RatingGraph
    {
        private readonly Dictionary<object, RecipeNode> recipeIndex;
        private readonly Dictionary<object, UserNode> userIndex;

        public RatingGraph()
        {
            this.recipeIndex = new Dictionary<object, RecipeNode>();
            this.userIndex = new Dictionary<object, UserNode>();
        }

        public void AddRating(short rating, Guid userId, Guid recipeId)
        {
            RecipeNode recipeNode;
            UserNode userNode;

            if (this.recipeIndex.ContainsKey(recipeId) == false)
            {
                recipeNode = new RecipeNode();
                recipeNode.Key = recipeId;
                recipeNode.Ratings = new List<Rating>();
                this.recipeIndex.Add(recipeId, recipeNode);
            }
            else
            {
                recipeNode = this.recipeIndex[recipeId];
            }

            if (this.userIndex.ContainsKey(userId) == false)
            {
                userNode = new UserNode();
                userNode.Key = userId;
                userNode.Ratings = new List<Rating>();
                this.userIndex.Add(userId, userNode);
            }
            else
            {
                userNode = this.userIndex[userId];
            }

            var newRating = new Rating();
            newRating.Weight = rating;
            newRating.Recipe = recipeNode;
            newRating.User = userNode;

            recipeNode.Ratings.Add(newRating);
            userNode.Ratings.Add(newRating);
        }

        public Guid[] GetSimilarRecipes(Guid recipeId)
        {
            if (this.recipeIndex.ContainsKey(recipeId) == false)
            {
                return new Guid[] { };
            }

            var recipeNode = this.recipeIndex[recipeId];
            var results = new Dictionary<Guid, int>();
            foreach (var rating1 in recipeNode.Ratings)
            {
                var userNode = rating1.User;
                foreach (var rating2 in userNode.Ratings)
                {
                    if (rating2.Recipe.Key == recipeId)
                    {
                        continue;
                    }

                    if (results.ContainsKey(rating2.Recipe.Key) == false)
                    {
                        results.Add(rating2.Recipe.Key, 1);
                    }
                    else
                    {
                        results[rating2.Recipe.Key]++;
                    }
                }
            }

            // For every pair in graph, calculate the Jaccard similarity coefficient 
            // (Number of overlapping ingredients divided by total distinct ingredients in both)
            var result = from k in results.Keys
                    orderby results[k] descending
                    select k;

            return result.ToArray();
        }
    }
}