using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Credits")]
public class CreditsInformation : ScriptableObject
{
    public CreditsCategory[] CreditsCategories;

    [Serializable]
    public struct CreditsCategory
    {
        public string Category;
        public string[] Lines;
    }
}
