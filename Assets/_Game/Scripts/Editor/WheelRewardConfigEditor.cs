using UnityEngine;
using UnityEditor;
using WheelOfFortune.Data.Rewards;

namespace WheelOfFortune.Editor
{
    [CustomEditor(typeof(WheelRewardConfig))]
    public sealed class WheelRewardConfigEditor : UnityEditor.Editor
    {
        private const int SLICE_COUNT = 8;
        
        private SerializedProperty _bronzeSlicesProp;
        private SerializedProperty _silverSlicesProp;
        private SerializedProperty _superSlicesProp;
        private SerializedProperty _bombItemProp;

        private RewardItemDefinition[] _availableItems;
        private bool _itemsFoldout = true;

        private void OnEnable()
        {
            _bronzeSlicesProp = serializedObject.FindProperty("_bronzeSlices");
            _silverSlicesProp = serializedObject.FindProperty("_silverSlices");
            _superSlicesProp = serializedObject.FindProperty("_superSlices");
            _bombItemProp = serializedObject.FindProperty("_bombItem");
            
            LoadAvailableItems();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            DrawAvailableItemsSection();
            
            EditorGUILayout.Space(10);
            DrawZoneSection("Bronze Slices (Danger Zones)", _bronzeSlicesProp, 30, 100, true);
            
            EditorGUILayout.Space(10);
            DrawZoneSection("Silver Slices (Safe Zones)", _silverSlicesProp, 100, 300);
            
            EditorGUILayout.Space(10);
            DrawZoneSection("Super Slices (Super Zones)", _superSlicesProp, 300, 1000);
            
            EditorGUILayout.Space(10);
            DrawBombSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAvailableItemsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _itemsFoldout = EditorGUILayout.Foldout(_itemsFoldout, "Available Reward Items", true);
            
            if (_itemsFoldout)
            {
                if (_availableItems == null || _availableItems.Length == 0)
                {
                    EditorGUILayout.HelpBox("No RewardItemDefinition assets found. Create some in Assets/_Project/Data/RewardItems/", MessageType.Warning);
                    
                    if (GUILayout.Button("Refresh Item List"))
                    {
                        LoadAvailableItems();
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"Found {_availableItems.Length} item(s):", EditorStyles.boldLabel);
                    
                    foreach (RewardItemDefinition item in _availableItems)
                    {
                        if (item != null && !item.IsBomb)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.ObjectField(item, typeof(RewardItemDefinition), false);
                            EditorGUILayout.LabelField($"[{item.RewardType}]", GUILayout.Width(100));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawZoneSection(string label, SerializedProperty slicesProp, int minAmount, int maxAmount, bool forceBomb = false)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Generate Random Rewards", GUILayout.Height(30)))
            {
                GenerateRandomRewards(slicesProp, minAmount, maxAmount, forceBomb);
            }
            
            if (GUILayout.Button("Clear All", GUILayout.Width(100), GUILayout.Height(30)))
            {
                ClearSlices(slicesProp);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(slicesProp, true);
            
            EditorGUILayout.EndVertical();
        }

        private void DrawBombSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Bomb Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_bombItemProp, new GUIContent("Bomb Item"));
            EditorGUILayout.EndVertical();
        }

        private void GenerateRandomRewards(SerializedProperty slicesProp, int minAmount, int maxAmount, bool forceBomb)
        {
            if (_availableItems == null || _availableItems.Length == 0)
            {
                EditorUtility.DisplayDialog("No Items Available", 
                    "Please create RewardItemDefinition assets first in Assets/_Project/Data/RewardItems/", 
                    "OK");
                return;
            }
            
            if (forceBomb && _bombItemProp.objectReferenceValue == null)
            {
                EditorUtility.DisplayDialog("Bomb Item Missing",
                    "Assign a Bomb RewardItemDefinition in the Bomb Configuration section before generating bronze rewards.",
                    "OK");
                return;
            }

            RewardItemDefinition[] nonBombItems = System.Array.FindAll(_availableItems, item => item != null && !item.IsBomb);
            
            if (nonBombItems.Length == 0)
            {
                EditorUtility.DisplayDialog("No Reward Items", 
                    "No non-bomb reward items found. Please create some RewardItemDefinition assets.", 
                    "OK");
                return;
            }

            slicesProp.arraySize = SLICE_COUNT;

            for (int index = 0; index < SLICE_COUNT; index++)
            {
                SerializedProperty elementProp = slicesProp.GetArrayElementAtIndex(index);
                SerializedProperty rewardItemProp = elementProp.FindPropertyRelative("_rewardItem");
                SerializedProperty amountProp = elementProp.FindPropertyRelative("_amount");

                RewardItemDefinition randomItem = nonBombItems[Random.Range(0, nonBombItems.Length)];
                int randomAmount = Random.Range(minAmount, maxAmount + 1);

                rewardItemProp.objectReferenceValue = randomItem;
                amountProp.intValue = randomAmount;
            }

            if (forceBomb && _bombItemProp.objectReferenceValue != null)
            {
                SerializedProperty bombElement = slicesProp.GetArrayElementAtIndex(SLICE_COUNT - 1);
                SerializedProperty rewardItemProp = bombElement.FindPropertyRelative("_rewardItem");
                SerializedProperty amountProp = bombElement.FindPropertyRelative("_amount");
                
                rewardItemProp.objectReferenceValue = _bombItemProp.objectReferenceValue;
                amountProp.intValue = 0;
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void ClearSlices(SerializedProperty slicesProp)
        {
            if (EditorUtility.DisplayDialog("Clear Slices", 
                "Are you sure you want to clear all slices in this zone?", 
                "Yes", "Cancel"))
            {
                slicesProp.arraySize = 0;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void LoadAvailableItems()
        {
            string[] guids = AssetDatabase.FindAssets("t:RewardItemDefinition");
            _availableItems = new RewardItemDefinition[guids.Length];

            for (int index = 0; index < guids.Length; index++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[index]);
                _availableItems[index] = AssetDatabase.LoadAssetAtPath<RewardItemDefinition>(path);
            }
        }
    }
}
