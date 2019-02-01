// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedLandmarkTypeExcludesSpecialCharacters)]
    class LocalizedLandmarkTypeExcludesSpecialCharacters : Rule
    {
        public LocalizedLandmarkTypeExcludesSpecialCharacters()
        {
            this.Info.Description = Descriptions.LocalizedLandmarkTypeExcludesSpecialCharacters;
            this.Info.HowToFix = HowToFix.LocalizedLandmarkTypeExcludesSpecialCharacters;
            this.Info.Standard = A11yCriteriaId.InfoAndRelationships;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            return LocalizedLandmarkType.ExcludesSpecialCharacters.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return LocalizedLandmarkType.NotNullOrEmpty;
        }
    } // class
} // namespace
