// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules.PropertyConditions;
using AccessibilityInsights.Rules.Resources;
using static AccessibilityInsights.Rules.PropertyConditions.ControlType;

namespace AccessibilityInsights.Rules.Library
{
    [RuleInfo(ID = RuleId.ControlShouldSupportInvokePattern)]
    class ControlShouldSupportInvokePattern : Rule
    {
        public ControlShouldSupportInvokePattern()
        {
            this.Info.Description = Descriptions.ControlShouldSupportInvokePattern;
            this.Info.HowToFix = HowToFix.ControlShouldSupportInvokePattern;
            this.Info.Standard = A11yCriteriaId.AvailableActions;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.Invoke.Matches(e) ? EvaluationCode.Pass : EvaluationCode.Error;
        }

        protected override Condition CreateCondition()
        {
            // Button is handled separately.
            return Hyperlink;
        }
    } // class
} // namespace
