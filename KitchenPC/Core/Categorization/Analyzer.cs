namespace KitchenPC.Categorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KitchenPC.Recipes;

    public class Analyzer
    {
        private const float Tolerance = .05f;

        private float i;  // TODO: Don't know correct name 
        private float invI; // TODO: Don't know correct name 

        private RecipeIndex breakfastIndex;
        private RecipeIndex lunchIndex;
        private RecipeIndex dinnerIndex;
        private RecipeIndex dessertIndex;
        private Dictionary<Guid, IRecipeClassification> trainingData;

        public void LoadTrainingData(IDBLoader loader)
        {
            this.trainingData = new Dictionary<Guid, IRecipeClassification>();
            this.breakfastIndex = new RecipeIndex();
            this.lunchIndex = new RecipeIndex();
            this.dinnerIndex = new RecipeIndex();
            this.dessertIndex = new RecipeIndex();
            var data = loader.LoadTrainingData();

            foreach (var recipe in data)
            {
                this.trainingData.Add(recipe.Recipe.Id, recipe);

                if (recipe.IsBreakfast)
                {
                    this.breakfastIndex.Add(recipe.Recipe);
                }

                if (recipe.IsLunch)
                {
                    this.lunchIndex.Add(recipe.Recipe);
                }

                if (recipe.IsDinner)
                {
                    this.dinnerIndex.Add(recipe.Recipe);
                }

                if (recipe.IsDessert)
                {
                    this.dessertIndex.Add(recipe.Recipe);
                }
            }
        }

        public bool CheckIfTrained(Guid recipeId, out IRecipeClassification trainedRecipe)
        {
            return this.trainingData.TryGetValue(recipeId, out trainedRecipe);
        }

        public AnalyzerResult GetPrediction(Recipe recipe)
        {
            var winsBreakfast = new Ranking(Category.Breakfast);
            var winsLunch = new Ranking(Category.Lunch);
            var winsDinner = new Ranking(Category.Dinner);
            var winsDessert = new Ranking(Category.Dessert);

            // Setup Tournament
            this.Compete(recipe, this.breakfastIndex, this.lunchIndex, winsBreakfast, winsLunch, winsDinner, winsDessert);
            this.Compete(recipe, this.breakfastIndex, this.dinnerIndex, winsBreakfast, winsLunch, winsDinner, winsDessert);
            this.Compete(recipe, this.breakfastIndex, this.dessertIndex, winsBreakfast, winsLunch, winsDinner, winsDessert);
            this.Compete(recipe, this.lunchIndex, this.dinnerIndex, winsBreakfast, winsLunch, winsDinner, winsDessert);
            this.Compete(recipe, this.lunchIndex, this.dessertIndex, winsBreakfast, winsLunch, winsDinner, winsDessert);
            this.Compete(recipe, this.dinnerIndex, this.dessertIndex, winsBreakfast, winsLunch, winsDinner, winsDessert);

            // Choose winner
            var result = GetWinner(winsBreakfast, winsLunch, winsDinner, winsDessert);
            return result;
        }

        private static AnalyzerResult GetWinner(Ranking winsBreakfast, Ranking winsLunch, Ranking winsDinner, Ranking winsDessert)
        {
            var meals = new Ranking[] { winsBreakfast, winsLunch, winsDinner, winsDessert };
            var sortedMealsByScore = meals.OrderByDescending(meal => meal.Score).ToArray();
            var firstPlace = sortedMealsByScore[0];
            var secondPlace = (sortedMealsByScore[1].Score / sortedMealsByScore[0].Score > 0.8f) ? sortedMealsByScore[1] : null;
            var result = new AnalyzerResult(firstPlace.Type, (secondPlace != null ? secondPlace.Type : Category.None));
            return result;
        }

        private void Compete(Recipe entryRecipe, RecipeIndex firstIndex, RecipeIndex secondIndex, Ranking winsBreakfast, Ranking winsLunch, Ranking winsDinner, Ranking winsDessert)
        {
            var prediction = this.GetPrediction(entryRecipe, firstIndex, secondIndex);
            if (.5f - Tolerance < prediction && prediction < .5f + Tolerance)
            {
                return; // No winner
            }

            float difference = (float)Math.Abs(prediction - 0.5);
            var winner = prediction >= 0.5 ? firstIndex : secondIndex;

            if (winner == this.breakfastIndex)
            {
                winsBreakfast.Score += difference;
            }

            if (winner == this.lunchIndex)
            {
                winsLunch.Score += difference;
            }

            if (winner == this.dinnerIndex)
            {
                winsDinner.Score += difference;
            }

            if (winner == this.dessertIndex)
            {
                winsDessert.Score += difference;
            }
        }
        
        private float GetPrediction(Recipe recipe, RecipeIndex firstIndex, RecipeIndex secondIndex)
        {
            // Reset I and invI
            this.invI = 0;
            this.i = 0; 
            var tokens = Tokenizer.Tokenize(recipe);

            foreach (var token in tokens)
            {
                var firstCount = firstIndex.GetTokenCount(token);
                var secondCount = secondIndex.GetTokenCount(token);
                this.CalculateProbability(firstCount, firstIndex.EntryCount, secondCount, secondIndex.EntryCount);
            }

            var prediction = this.CombineProbability();
            return prediction;
        }

        private void CalculateProbability(float firstCategoryCount, float firstCategoryTotal, float secondCategoryCount, float secondCategoryTotal)
        {
            const float S = 1f;
            const float X = .5f;

            var bw = firstCategoryCount / firstCategoryTotal;
            var gw = secondCategoryCount / secondCategoryTotal;
            var pw = bw / (bw + gw);
            var n = firstCategoryCount + secondCategoryCount;
            var fw = ((S * X) + (n * pw)) / (S + n);

            this.LogProbability(fw);
        }

        private void LogProbability(float probability)
        {
            if (float.IsNaN(probability))
            {
                return;
            }

            this.i = this.i == 0 ? probability : this.i * probability;
            this.invI = this.invI == 0 ? (1 - probability) : this.invI * (1 - probability);
        }

        private float CombineProbability()
        {
            return this.i / (this.i + this.invI);
        }
    }
}