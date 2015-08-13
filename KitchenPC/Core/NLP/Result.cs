namespace KitchenPC.NLP
{
    using Enums;
    using Ingredients;

    public abstract class Result
    {

        protected Result(string input)
        {
            Input = input;
        }

        public string Input { get; set; }

        public virtual IngredientUsage Usage
        {
            get
            {
                return null;
            }
        }

        public abstract MatchResult Status { get; }

        /// <summary>Assembles a Result baesd on NLP match data for a given template.</summary>
        /// <param name="template">A passing NLP template that yieled match data</param>
        /// <param name="input">The original input string that was parsed</param>
        /// <param name="matchdata">Match data generated from the specified NLP template</param>
        /// <returns>Result describing the generated KitchenPC IngredientUsage or error code if usage could not be generated</returns>
        public static Result BuildResult(Template template, string input, MatchData matchdata)
        {
            var result = new IngredientUsage();
            var ingName = (matchdata.Ingredient.Parent == null) ? matchdata.Ingredient.IngredientName : matchdata.Ingredient.Parent.IngredientName;
            result.Ingredient = new Ingredient(matchdata.Ingredient.Id, ingName);
            result.Ingredient.ConversionType = matchdata.Ingredient.ConversionType;
            result.Ingredient.UnitWeight = matchdata.Ingredient.UnitWeight;
            result.Amount = matchdata.Amount;
            result.PrepNote = matchdata.Preps.HasValue ? matchdata.Preps.ToString() : template.DefaultPrep;
            var pairings = matchdata.Ingredient.Pairings;

            NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Ingredient: {0}", matchdata.Ingredient.IngredientName);
            if (matchdata.Ingredient.Parent != null)
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Re-Link to Root Ingredient: {0}", matchdata.Ingredient.Parent.IngredientName);
            }

            if (template.AllowPartial && matchdata.Amount == null)
            {
                return new PartialMatch(input, result.Ingredient, result.PrepNote);
            }

            if (matchdata.Unit is CustomUnitNode)
            {
                IngredientForm form;
                if (UnitSynonyms.TryGetFormForIngredient(matchdata.Unit.Name, matchdata.Ingredient.Id, out form))
                {
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Based on unit name {0}, linking to form id {1}", matchdata.Unit.Name, form.FormId);
                    result.Form = form;
                }
                else
                {
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] ERROR: Unable to find link between unit '{0}' and ingredient '{1}'.", matchdata.Unit.Name, result.Ingredient.Name);
                    return new NoMatch(input, MatchResult.UnknownUnit);
                }
            }
            else
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] No custom unit found, so cannot get form based on unit.");
            }

            if (matchdata.Form != null)
            {
                IngredientForm form;
                if (FormSynonyms.TryGetFormForIngredient(matchdata.Form.FormName, matchdata.Ingredient.Id, out form))
                {
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Based on reference to form {0}, linking to form id {1}", matchdata.Form.FormName, form.FormId);
                    result.Form = form;
                }
                else
                {
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] ERROR: Unable to find link between form '{0}' and ingredient '{1}.", matchdata.Form.FormName, result.Ingredient.Name);
                    return new NoMatch(input, MatchResult.UnknownForm);
                }
            }
            else
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] No known form found, so cannot get form based on form synonym.");
            }

            if (result.Form == null)
            {
                if (matchdata.Unit == null || matchdata.Unit.Unit == Units.Unit) // TODO: Is second part necessary? Only Units.Unit would be custom form types, and we'd have errored out already if that didn't match
                {
                    result.Form = pairings.Unit;
                    NlpTracer.ConditionalTrace(pairings.HasUnit, TraceLevel.Debug, "[BuildResult] Linking to default Unit paired form {0}", pairings.Unit);
                }
                else
                {
                    switch (Unit.GetConvType(matchdata.Unit.Unit))
                    {
                        case UnitType.Volume:
                            result.Form = pairings.Volume;
                            NlpTracer.ConditionalTrace(pairings.HasVolume, TraceLevel.Debug, "[BuildResult] Linking to default paired Volume form {0}", pairings.Volume);
                            break;
                        case UnitType.Weight:
                            result.Form = pairings.Weight;
                            NlpTracer.ConditionalTrace(pairings.HasWeight, TraceLevel.Debug, "[BuildResult] Linking to default paired Weight form {0}", pairings.Weight);
                            break;
                    }
                }

                if (result.Form == null && result.Amount.Unit == Units.Ounce && pairings.HasVolume)
                {
                    result.Form = pairings.Volume;
                    result.Amount.Unit = Units.FluidOunce;
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Interpretting reference to Ounces as Fluid Ounces and linking to volumetric form {0}", pairings.Volume);
                }

                if (result.Form == null)
                {
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Could not find any default pairing for the unit type: {0}", result.Amount.Unit);
                }
            }

            var parsedType = Unit.GetConvType(result.Amount.Unit);
            if (result.Form != null && parsedType == Unit.GetConvType(result.Form.FormUnitType))
            {
                NlpTracer.Trace(TraceLevel.Info, "[BuildResult] SUCCESS: Linked form is compatible with usage reference.");
                return new Match(input, result);
            }

            NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Running anomalous parsing.");

            // TODO: If matchdata has multiple prep notes, we either need to only parse the user entered one or avoid duplicate matches
            if (parsedType == UnitType.Volume && matchdata.Preps.HasValue)
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Checking for form matching prep note: {0}", matchdata.Preps);

                IngredientForm form;
                if (FormSynonyms.TryGetFormForPrep(matchdata.Preps, matchdata.Ingredient, true, out form))
                {
                    result.Form = form;

                    if (parsedType == Unit.GetConvType(result.Form.FormUnitType))
                    {
                        NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] SUCCESS: Found matching volumetric form, allowing prep to form fall-through.");
                        result.PrepNote = matchdata.Preps.ToString();
                        return new AnomalousMatch(input, AnomalousResult.Fallthrough, result);
                    }
                    else
                    {
                        NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Found matching form, but form is not compatible with volumetric usage.");
                    }
                }
            }
            else
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Could not clarify form through prep note, since unit type is not volumetric or there is no prep note.");
            }

            if (result.Form != null)
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Form and unit incompatible - attempting to auto-convert form {0}", result.Form);
                var formType = Unit.GetConvType(result.Form.FormUnitType);

                if (parsedType == UnitType.Weight && formType == UnitType.Volume && pairings.HasWeight)
                {
                    NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] SUCCESS: Converting to default weight pairing, and setting prep note to: {0}", result.Form.FormDisplayName);
                    result.PrepNote = result.Form.FormDisplayName;
                    result.Form = pairings.Weight;
                    return new AnomalousMatch(input, AnomalousResult.AutoConvert, result);
                }
                else if (parsedType == UnitType.Unit && formType == UnitType.Volume)
                {
                    if (pairings.HasUnit && (matchdata.Unit == null || string.IsNullOrEmpty(matchdata.Unit.Name)))
                    {
                        NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] SUCCESS: Converting to default unit pairing, and setting prep note to: {0}", result.Form.FormDisplayName);
                        result.PrepNote = result.Form.FormDisplayName;
                        result.Form = pairings.Unit;
                        return new AnomalousMatch(input, AnomalousResult.AutoConvert, result);
                    }

                    if (matchdata.Unit != null && false == string.IsNullOrEmpty(matchdata.Unit.Name))
                    {
                        IngredientForm form;
                        NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Attempting to convert volumetric usage to unit form for custom unit: {0}", matchdata.Unit.Name);
                        if (UnitSynonyms.TryGetFormForIngredient(matchdata.Unit.Name, matchdata.Ingredient.Id, out form))
                        {
                            NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] SUCCESS: Converting to custom unit pairing, and setting prep note to: {0}", result.Form.FormDisplayName);
                            result.PrepNote = result.Form.FormDisplayName;
                            result.Form = form;
                            return new AnomalousMatch(input, AnomalousResult.AutoConvert, result);
                        }
                    }
                }
            }
            else
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] Could not auto-convert form since there is no form to convert.");
            }

            if (result.Form == null)
            {
                NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] ERROR: Anomalous parsing could still not find a form for this usage.");
                return new NoMatch(input, MatchResult.NoForm);
            }

            NlpTracer.Trace(TraceLevel.Debug, "[BuildResult] ERROR: Anomalous parsing could not fix form/unit incompatibility.");

            return new NoMatch(input, MatchResult.IncompatibleForm);
        }
    }
}