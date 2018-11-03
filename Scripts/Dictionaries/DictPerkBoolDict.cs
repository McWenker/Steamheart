using System;
using System.Runtime.Serialization;

[Serializable]
public class DictPerkBoolDict : SerializableDictionary<DictPerk, bool>
{
    public DictPerkBoolDict() : base() { }
    public DictPerkBoolDict(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
