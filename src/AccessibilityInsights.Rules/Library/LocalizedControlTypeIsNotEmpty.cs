// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.StringProperties;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;


namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.LocalizedControlTypeNotEmpty)]
    class LocalizedControlTypeIsNotEmpty : Rule
    {
        public LocalizedControlTypeIsNotEmpty()
        {
            this.Info.Description = Descriptions.LocalizedControlTypeNotEmpty;
            this.Info.HowToFix = HowToFix.LocalizedControlTypeNotEmpty;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
            this.Info.PropertyID = PropertyType.UIA_LocalizedControlTypePropertyId;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException("The element is null");

            return LocalizedControlType.NotEmpty.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            return IsKeyboardFocusable & LocalizedControlType.NotNull;
        }
    } // class
} // namespace
