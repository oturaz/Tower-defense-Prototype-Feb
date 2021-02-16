using System.Runtime.Serialization;
using UnityEngine;

sealed class Vector3SerializationSurrogate : ISerializationSurrogate
{

    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    // Method called to deserialize a Vector3 object
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;
        return obj;
    }
}

sealed class TileDataSerializationSurrogate : ISerializationSurrogate
{
    // Method called to serialize a TileData object
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        TileData td = (TileData)obj;
        info.AddValue("Availability", td.Availability);
        info.AddValue("Height", td.Height);
    }

    // Method called to deserialize a TileData object
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        TileData td = (TileData)obj;
        td.Availability = (bool)info.GetValue("Availability", typeof(bool));
        td.Height = (float)info.GetValue("Height", typeof(float));
        obj = td;
        return obj;
    }
}

sealed class EnemyDataSerializationSurrogate : ISerializationSurrogate
{
    // Method called to serialize a EnemyData object
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        EnemyData ed = (EnemyData)obj;
        info.AddValue("enemyType", ed.enemyType);
        info.AddValue("waveIndex", ed.waveIndex);
        info.AddValue("pathIndex", ed.pathIndex);
        info.AddValue("quantity", ed.quantity);
    }

    // Method called to deserialize a EnemyData object
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        EnemyData ed = (EnemyData)obj;
        ed.enemyType = (int)info.GetValue("enemyType", typeof(int));
        ed.waveIndex = (int)info.GetValue("waveIndex", typeof(int));
        ed.pathIndex = (int)info.GetValue("pathIndex", typeof(int));
        ed.quantity = (int)info.GetValue("quantity", typeof(int));
        obj = ed;
        return obj;
    }
}