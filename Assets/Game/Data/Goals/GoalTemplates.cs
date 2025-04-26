using System;
using System.Collections.Generic;
using System.Linq;

public static class GoalTemplates
{
    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } = ((Building[])Enum.GetValues(typeof(Building))).Select(building => new BuildingAmount($"Have at least 3 {building}'s.\n\nComplete this task to double all building production.", new() { { building, 5 } })).ToList();
}