using System;
using System.Runtime.Serialization;

[Serializable]
public class StatIntDict : SerializableDictionary<StatTypes, int>
{
    public StatIntDict() : base() { }
    public StatIntDict(SerializationInfo info, StreamingContext context) : base(info, context) { }

}
