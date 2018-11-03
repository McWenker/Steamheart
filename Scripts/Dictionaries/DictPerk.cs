using System;

[Serializable]
public class DictPerk
{
    public string PerkName;
    public string CardinalName;

    public DictPerk(string perkName, string cardinalName)
    {
        PerkName = perkName;
        CardinalName = cardinalName;
    }

    public bool Equals(DictPerk other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return other.PerkName == PerkName && other.CardinalName == CardinalName;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != typeof(DictPerk)) return false;
        return Equals((DictPerk)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return PerkName.GetHashCode() ^ CardinalName.GetHashCode();
        }
    }
}
