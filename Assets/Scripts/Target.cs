using System; // This namespace is required to use the [Serializable] attribute
using UnityEngine; // This namespace is required for the Vector3 structure and other Unity-specific classes and methods

[Serializable] // necessary for converting the class to and from JSON format
public class Target
{
    public string Name;
    public int FloorNumber;
    public Vector3 Position;
    public Vector3 Rotation;

    public Target(string name, Vector3 position, Vector3 rotation)
    {
        this.Name = name;
        this.Position = position;
        this.Rotation = rotation;
    }
}

