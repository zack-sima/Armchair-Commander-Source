using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http.Headers;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

//*partially from Game Dev Guide's YouTube tutorial
public class CSVLoader {
    private TextAsset csvFile;
    private readonly char lineSeparator = '\n';
    private readonly char surround = '"';
    private readonly string[] fieldSeparator = { "\",\"" };

    public void LoadCSV(string fileName) {
        csvFile = Resources.Load<TextAsset>(fileName);
    }
    public Dictionary<string, string> GetDictionaryValuies(string attributeId) {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] lines = csvFile.text.Split(lineSeparator);
        int attributeIndex = -1;
        string[] headers = lines[0].Split(fieldSeparator, StringSplitOptions.None);
        for (int i = 0; i < headers.Length; i++) {
            if (headers[i].Contains(attributeId)) {
                attributeIndex = i;
                break;
            }
        }
        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i];
            string[] fields = CSVParser.Split(line);
            for (int f = 0; f < fields.Length; f++) {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].TrimEnd(surround);

            }
            if (fields.Length > attributeIndex) {
                var key = fields[0];
                if (dictionary.ContainsKey(key)) {
                    continue;
                }
                var value = fields[attributeIndex];
                dictionary.Add(key, value);

            }
        }
        return dictionary;
    }
}
