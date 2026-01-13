using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loading/Loading Text Database", fileName = "LoadingTextDatabase")]
public class LoadingTextDatabase : ScriptableObject
{
    [TextArea(2, 4)]
    public List<string> texts = new List<string>();
}
