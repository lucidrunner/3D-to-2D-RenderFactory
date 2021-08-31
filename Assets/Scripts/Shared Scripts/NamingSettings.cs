using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Shared_Scripts
{
    public class NamingSettings : SettingsBase
    {
        public const string SettingsPath = SettingsBasePath + "NamingSettings.asset";

        public string ExampleOutput
        {
            get
            {
                StringBuilder _stringBuilder = new StringBuilder();
                foreach (string _element in renderNameFormat)
                {
                    //Slightly ugly label text matching but w/e
                    switch (_element)
                    {
                        case "Model Name":
                            _stringBuilder.Append("BuddyModels_");
                            break;
                        case "Rig Tag":
                            _stringBuilder.Append("ISO4_");
                            break;
                        case "Camera Index":
                            _stringBuilder.Append((includeFormatIdentifier ? "c" : "") + "0_");
                            break;
                        case "Animation Index":
                            _stringBuilder.Append(UseAnimationName ? "Walk_" : (includeFormatIdentifier ? "a" : "") + "3_");
                            break;
                        case "Frame Index":
                            _stringBuilder.Append((includeFormatIdentifier ? "f" : "") + "31_");
                            break;
                        case "Static":
                            _stringBuilder.Append("Static_");
                            break;
                    }
                }

                //Remove the last _
                if(_stringBuilder.Length > 0)
                    _stringBuilder.Remove(_stringBuilder.Length - 1, 1);
                else
                {
                    //Otherwise, If we've done any changes to this class there's a chance we've cleared the format list, requiring a reset
                    ResetFormat();
                }
                return _stringBuilder.ToString();
            }
        }

        public List<char> RenderNameFormat => renderNameFormat.Select(aElement => aElement[0]).ToList();

        public bool UseAnimationName => useAnimationName;
        [SerializeField] protected bool useAnimationName = false;

        [SerializeField] protected bool includeRigTag = true;
        [SerializeField] protected bool includeStaticTag = false;

        public bool IncludeFormatIdentifier => includeFormatIdentifier;
        [SerializeField] protected bool includeFormatIdentifier = false;

        [SerializeField]
        private List<string> renderNameFormat = new List<string>
        {
            "Model Name", "Rig Tag", "Camera Index", "Animation Index", "Frame Index"
        };

        public void ResetFormat()
        {
            renderNameFormat = new List<string>
            {
                "Model Name", "Camera Index", "Animation Index", "Frame Index"
            };
            if (includeRigTag)
            {
                renderNameFormat.Insert(1, "Rig Tag");
            }
            if(includeStaticTag)
            {
                renderNameFormat.Add("Static");
            }
        }

        private void ApplyRigTagChange()
        {
            if(includeRigTag == (renderNameFormat.FirstOrDefault(aElement => aElement.Equals("Rig Tag")) == null))
                ApplyTagChange(includeRigTag, "Rig Tag", renderNameFormat.IndexOf("Model Name") + 1); //We add the rig tag after the model name by default
        }


        private void ApplyStaticTagChange()
        {
            if(includeStaticTag == (renderNameFormat.FirstOrDefault(aElement => aElement.Equals("Static")) == null))
                ApplyTagChange(includeStaticTag, "Static", renderNameFormat.Count); //We add the static tag at the end of the name
        }

        private void ApplyTagChange(bool aToggleState, string aLabelText, int aInsertIndex)
        {
            if (aToggleState == false)
            {
                var _element = renderNameFormat.FirstOrDefault(aElement => aElement.Equals(aLabelText));
                if (_element != null)
                    renderNameFormat.Remove(_element);
            }

            else
            {
                renderNameFormat.Insert(aInsertIndex, aLabelText);
            }
        }
        
        public void MoveFormatLeft(int aIndex)
        {
            if (aIndex > 0)
            {
                var _element = renderNameFormat[aIndex];
                renderNameFormat.Insert(aIndex - 1, _element);
                renderNameFormat.RemoveAt(aIndex + 1);
            }
        }

        public void MoveFormatRight(int aIndex)
        {
            if (aIndex < renderNameFormat.Count - 1)
            {
                var _element = renderNameFormat[aIndex];
                renderNameFormat.Remove(_element);
                renderNameFormat.Insert(aIndex + 1, _element);
            }
        }
        
        public static NamingSettings GetOrCreateSettings()
        {
            var _settings = AssetDatabase.LoadAssetAtPath<NamingSettings>(SettingsPath);
            if (_settings != null) return _settings;
            _settings = ScriptableObject.CreateInstance<NamingSettings>();
            AssetDatabase.CreateAsset(_settings, SettingsPath);
            AssetDatabase.SaveAssets();
            return _settings;
        }
        
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}