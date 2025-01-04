using UnityEngine;

public class NamedArrayAttribute : PropertyAttribute
{
    public string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}