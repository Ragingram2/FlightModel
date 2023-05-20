using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "air")]
public class AirFoilImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var fileData = System.IO.File.ReadAllText(ctx.assetPath);
        var lines = fileData.Split("\n"[0]);
        bool startReading = false;
        var so = ScriptableObject.CreateInstance<AirFoil>();

        foreach (var line in lines)
        {
            var lineData = (line.Trim()).Split(","[0]);
            if (lineData[0].Equals("Alpha") && !startReading)
            {
                startReading = true;
                continue;
            }
            if (startReading && line.Length > 0.0f)
            {
                Vector3 v;
                float.TryParse(lineData[0], out v.x);
                float.TryParse(lineData[1], out v.y);
                float.TryParse(lineData[2], out v.z);
                so.data.Add(v);
            }
        }
        so.initialize();

        string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{ctx.assetPath}.asset");
        AssetDatabase.CreateAsset(so, name);
    }
}
