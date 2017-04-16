
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;
using SimpleJSON;
using TankArena.Models.Characters;
using MovementEffects;

namespace TankArena.Models.Dialogue
{
    public class DialogueScene : FileLoadedEntityModel
    {

        public Sprite SceneBackground
        {
            get
            {
                return (Sprite)properties[EK.EK_BACKGROUND_IMAGE];
            }
        }

        public Sprite LeftModel
        {
            get
            {
                return (Sprite)properties[EK.EK_MODEL_LEFT];
            }
        }
        public Sprite RightModel
        {
            get
            {
                return (Sprite)properties[EK.EK_MODEL_RIGHT];
            }
        }
        public string LeftName
        {
            get
            {
                return (string)properties[EK.EK_NAME_LEFT];
            }
        }
        public string RightName
        {
            get
            {
                return (string)properties[EK.EK_NAME_RIGHT];
            }
        }
        public List<DialogueBeat> dialogueBeats;

        //quick access to specific scene beat
        public DialogueBeat this[int i]
        {
            get { return dialogueBeats[i]; }
        }


        public DialogueScene(string jsonPath) : base(jsonPath) { }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            //GET DIALOGUE SCENE MODEL META INFO
            JSONClass scene = json[EK.EK_SCENE].AsObject;
            var bg = ResolveSpecialContent(scene[EK.EK_BACKGROUND_IMAGE].Value);

            properties[EK.EK_BACKGROUND_IMAGE] = (bg is PlayableCharacter) ?
                ((PlayableCharacter)bg).Background
                : bg;

            foreach (string key in new string[] { EK.EK_MODEL_LEFT, EK.EK_MODEL_RIGHT })
            {
                var model = ResolveSpecialContent(scene[key].Value);

                if (model is PlayableCharacter)
                {
                    PlayableCharacter character = ((PlayableCharacter)model);
                    properties[key] = character.CharacterModel;
                    var nameKey = "name_" + key.Split(new char[] { '_' }, 2).Last();
                    properties[nameKey] = character.Name;
                }
                else
                {
                    properties[key] = model;
                }
            }
            foreach (string nameKey in new string[] { EK.EK_NAME_LEFT, EK.EK_NAME_RIGHT })
            {
                if (json[nameKey] != null)
                {
                    properties[nameKey] = json[nameKey].Value;
                }
            }

            dialogueBeats = new List<DialogueBeat>();
            // GET INDIVIDUAL DIALOGUE BEATS
            foreach (JSONNode beatObj in json[EK.EK_BEATS].AsArray)
            {
                dialogueBeats.Add(DialogueBeat.parseJSON(beatObj.AsObject));
            }

            yield return 0.0f;
        }

    }
}
