using System;
using System.Collections.Generic;
using System.Linq;

public static class GoalTemplates
{
    public static IReadOnlyList<SecretTask> IndividualSecretTasks =>
        ((Building[])Enum.GetValues(typeof(Building)))
        .Where(building => building != Building.None)
        .Select(building => {
            int n = building >= Building.Composter ? 1 : 3;
            string s = n >= 2 ? "s" : "";
            return new BuildingAmount($"Have at least {n} {building}{s}.\\n\\nComplete this task to double all building production.\"", new() { { building, n } }); 
        })
        .ToList();
}