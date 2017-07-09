using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DefaultImporter : MonoBehaviour
{

	private static readonly char[] CSV_SEPARATOR = new char[] {','};

    public static void ImportPart()
    {
        try
        {
            var commandLineArgsMap = makeArgsMap();
            if (!String.IsNullOrEmpty(commandLineArgsMap["-spritesheet"])) {
                importSpriteSheet(commandLineArgsMap);
            }

			var extraKeysCsv = commandLineArgsMap["-simpleassets"];
			foreach(var key in extraKeysCsv.Split(CSV_SEPARATOR)) {
				importSimpleAsset(commandLineArgsMap, ("-" + key));
			}
        } catch (Exception ex) {
			Debug.Log(ex);
			EditorApplication.Exit(1);
		}
    }

    private static void importSimpleAsset(Dictionary<string, string> commandLineArgsMap, String assetKey)
    {
        var assetPath = commandLineArgsMap[assetKey];
		Debug.Log(String.Format("importing asset at path {0} and deleteing existing meta...", assetPath));
		//delete previous entry
		File.Delete(assetPath + ".meta");
		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }

    private static void importSpriteSheet(Dictionary<string, string> commandLineArgsMap)
    {
        var spritePath = commandLineArgsMap["-spritesheet"];
        var spriteWidth = int.Parse(commandLineArgsMap["-width"]);
        var spriteHeight = int.Parse(commandLineArgsMap["-height"]);
        var rows = int.Parse(commandLineArgsMap["-rows"]);
        var cols = int.Parse(commandLineArgsMap["-cols"]);
        var paddingX = commandLineArgsMap.ContainsKey("-paddingX") ?
            int.Parse(commandLineArgsMap["-paddingX"]) : 0;
        var paddingY = commandLineArgsMap.ContainsKey("-paddingY") ?
            int.Parse(commandLineArgsMap["-paddingY"]) : 0;
        int spritesCount = rows * cols;

		Debug.Log(String.Format("Importing spritesheet at {0}", spritePath));
		var pathObj = Path.GetFileNameWithoutExtension(spritePath);
		Debug.Log("Deleting existing meta info");
		File.Delete(spritePath + ".meta");
		Debug.Log("Init importer...");
		AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate); // init importer
        var textureImporter = AssetImporter.GetAtPath(spritePath) as TextureImporter;
	
		Debug.Log("Got improter: " + textureImporter);
		textureImporter.spriteImportMode = SpriteImportMode.Multiple;
		textureImporter.name = pathObj;
		// textureImporter.spriteBorder = new Vector4(paddingX, paddingY, paddingX, paddingY);

        List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
        var sheetHeight = (rows - 1) * (spriteHeight + paddingY); 
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var metaData = new SpriteMetaData();
                metaData.rect = new Rect(
                    j * spriteWidth + j * paddingX,
                    sheetHeight - (i * spriteHeight + i * paddingY),
                    spriteWidth, 
                    spriteHeight);
				metaData.border = new Vector4(paddingX, paddingY, paddingX, paddingY);
                metaData.pivot = metaData.rect.center;
				metaData.name = String.Format("{0}_{1}_{2}", pathObj, (i), (j));
                metaDataList.Add(metaData);
				Debug.Log(String.Format(
                    "Created metadata #{0} ({1}) with rect: {2}", (1 + (i * cols + j)),
                    metaData.name,
                    metaData.rect));
            }
        }
        textureImporter.spritesheet = metaDataList.ToArray();
		textureImporter.SaveAndReimport();
    }

    private static Dictionary<string, string> makeArgsMap()
    {
        var argsArray = System.Environment.GetCommandLineArgs();
        var mapping = new Dictionary<string, string>(argsArray.Length / 2);
        for (int i = 0; i < argsArray.Length; i++)
        {
            //this is a parameter
            if (
                (i < argsArray.Length - 1) &&
             argsArray[i].StartsWith("-") &&
              !argsArray[i + 1].StartsWith("-")
              )
            {
                mapping.Add(argsArray[i], argsArray[i + 1]);
            }
            else
            {
                if (argsArray[i].StartsWith("-"))
                {
                    //add bool switches
                    mapping.Add(argsArray[i], true.ToString());
                }
            }

        }
		var cmdPrint = new StringBuilder("Commands:\n");
		foreach(var entry in mapping)
		{
			cmdPrint.Append(entry.Key + "=" + entry.Value + "\n");
		}
		Debug.Log(cmdPrint);
        return mapping;
    }
}
