// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.IsKeyboardFocusableDescendantTextPattern)]
    class IsKeyboardFocusableDescendantTextPattern : Rule
    {
        public IsKeyboardFocusableDescendantTextPattern()
        {
            this.Info.Description = Descriptions.IsKeyboardFocusableDescendantTextPattern;
            this.Info.HowToFix = HowToFix.IsKeyboardFocusableDescendantTextPattern;
            this.Info.Standard = A11yCriteriaId.Keyboard;
            this.Info.PropertyID = PropertyType.UIA_IsKeyboardFocusablePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            return EvaluationCode.Note;
        }

        protected override Condition CreateCondition()
        {
            return IsContentOrControlElement & IsNotKeyboardFocusable & IsNotOffScreen & BoundingRectangle.Valid
                & Patterns.Text & AnyAncestor(Patterns.Text);
        }
    } // class
} // namespace
