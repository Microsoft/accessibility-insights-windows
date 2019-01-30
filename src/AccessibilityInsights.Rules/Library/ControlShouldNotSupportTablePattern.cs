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
    [RuleInfo(ID = RuleId.ControlShouldNotSupportTablePattern)]
    class ControlShouldNotSupportTablePattern : Rule
    {
        public ControlShouldNotSupportTablePattern()
        {
            this.Info.Description = Descriptions.ControlShouldNotSupportTablePattern;
            this.Info.HowToFix = HowToFix.ControlShouldNotSupportTablePattern;
            this.Info.Standard = A11yCriteriaId.NameRoleValue;
        }

        public override EvaluationCode Evaluate(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return Patterns.Table.Matches(e) ? EvaluationCode.Error : EvaluationCode.Pass;
        }

        protected override Condition CreateCondition()
        {
            return List;
        }
    } // class
} // namespace
