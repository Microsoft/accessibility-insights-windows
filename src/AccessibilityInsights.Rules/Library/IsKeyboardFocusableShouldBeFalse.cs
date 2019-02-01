// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableShouldBeFalse)]
    class IsKeyboardFocusableShouldBeFalse : Rule
    {
        public IsKeyboardFocusableShouldBeFalse()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableShouldBeFalse;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableShouldBeFalse;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return IsNotKeyboardFocusable.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Warning;
        }

        protected override Condition CreateCondition()
        {
            return ElementGroups.ExpectedNotToBeFocusable;
        }
    } // class
} // namespace
