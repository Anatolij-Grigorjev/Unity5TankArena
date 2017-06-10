using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteImporter : MonoBehaviour
{

    public static void ImportSprite()
    {
        var commandLineArgsMap = makeArgsMap();

        importSpriteSheet(commandLineArgsMap);
    }

    private static void importSpriteSheet(Dictionary<string, string> commandLineArgsMap)
    {
        var spritePath = commandLineArgsMap["-spritesheet"];
        var spriteWidth = int.Parse(commandLineArgsMap["-width"]);
        var spriteHeight = int.Parse(commandLineArgsMap["-height"]);
        var rows = int.Parse(commandLineArgsMap["-rows"]);
        var cols = int.Parse(commandLineArgsMap["-cols"]);
        var paddingX = commandLineArgsMap.ContainsKey("paddingX") ?
            int.Parse(commandLineArgsMap["-paddingX"]) : 0;
        var paddingY = commandLineArgsMap.ContainsKey("paddingY") ?
            int.Parse(commandLineArgsMap["paddingY"]) : 0;
        int spritesCount = rows * cols;

        var textureImporter = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();


        for (int i = 0; i < rows; i++)
		{
            for (int j = 0; j < cols; j++)
            {
                var metaData = new SpriteMetaData();
                metaData.rect = new Rect(
					j * spriteWidth + j * paddingX,
					i * spriteHeight + i * paddingY,
					spriteWidth,
					spriteHeight
                );
                metaData.pivot = metaData.rect.center;
                metaDataList.Add(metaData);
            }
		}

        textureImporter.spritesheet = metaDataList.ToArray();
        AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
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

        return mapping;
    }
}
