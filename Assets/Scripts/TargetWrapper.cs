using System;

[Serializable] // necessary for converting the class to and from JSON format
public class TargetWrapper {
    public Target[] TargetList; // provides a container for an array of Target objects
}
