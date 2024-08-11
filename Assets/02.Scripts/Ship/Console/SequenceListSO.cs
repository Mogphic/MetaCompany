using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewSequenceList", menuName = "Sequence Console/Sequence List")]
public class SequenceListSO : ScriptableObject
{
    public List<Sequence> sequences = new List<Sequence>();

    public Sequence GetSequence(string name)
    {
        return sequences.Find(s => s.name == name);
    }

}