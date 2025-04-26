public enum PlayerFaction
{
    None,
    Bumbi,
    Lyki,
    Pigyn,
    Gumbi,
    Seltas,
    PomPom
}

public static class PlayerFactionExtension
{
    public static Resource StartingResource(this PlayerFaction faction)
    {
        switch (faction)
        {
            case PlayerFaction.None:
                return Resource.None;
            case PlayerFaction.Bumbi:
                return Resource.Pebbles;
            case PlayerFaction.Lyki:
                return Resource.Leaves;
            case PlayerFaction.Pigyn:
                return Resource.Mana;
            case PlayerFaction.Gumbi:
                return Resource.Wood;
            case PlayerFaction.Seltas:
                return Resource.Dewdrops;
            case PlayerFaction.PomPom:
                return Resource.Fireflies;
        }
        return Resource.None;
    }
}