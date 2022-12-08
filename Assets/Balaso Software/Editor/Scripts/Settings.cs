using System;
using System.Collections.Generic;
using UnityEngine;

namespace Balaso
{
    public class Settings : ScriptableObject
    {
        public bool UseLocalizationValues;
        public LanguagesDictionary LocalizedPopupMessageDictionary;
        public List<string> SkAdNetworkIds;
    }
}
