using System.Collections.Generic;

public class ResourcePledge
{
    public readonly Dictionary<SharedGoalID, Dictionary<Resource, int>> goalPledges = new();
}