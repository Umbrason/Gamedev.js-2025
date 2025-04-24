using System.Collections.Generic;


public class ResourcesUtilities
{
    /// <summary>
    /// How much is valued owning one unit of resource
    /// </summary>
    public Dictionary<Resource, float> Stock = new();

    /// <summary>
    /// How much is valued increasing the production by one
    /// </summary>
    public Dictionary<Resource, float> Prodution = new();
}

