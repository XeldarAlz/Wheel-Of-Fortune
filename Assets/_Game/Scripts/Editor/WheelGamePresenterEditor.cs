using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Presentation.Installers;

namespace WheelOfFortune.Editor
{
    [CustomEditor(typeof(WheelGamePresenterInstaller))]
    public sealed class WheelGamePresenterEditor : UnityEditor.Editor
    {
        private const float BUTTON_HEIGHT = 30.0f;
        private const float SECTION_SPACING = 15.0f;
        private const float MINIMUM_BOMB_CHANCE = 0.0f;
        private const float MAXIMUM_BOMB_CHANCE = 1.0f;
        private const float DEFAULT_CHEAT_BOMB_CHANCE = 0.3f;
        private const int DEFAULT_SEED = 12345;
        private const int INVALID_SLICE = -1;

        private float _cheatBombChance = DEFAULT_CHEAT_BOMB_CHANCE;
        private int _customSeed = DEFAULT_SEED;
        private int _specificSliceIndex;
        private bool _useCustomSeed;
        private bool _showSliceInfo;
        private bool _continuousForceBomb;
        private bool _continuousAvoidBomb;
        private bool _stylesInitialized;

        private GUIStyle _headerStyle;
        private GUIStyle _subHeaderStyle;
        private GUIStyle _successButtonStyle;
        private GUIStyle _dangerButtonStyle;
        private GUIStyle _warningButtonStyle;
        
        public override void OnInspectorGUI()
        {
            if (!_stylesInitialized)
            {
                InitializeStyles();
                _stylesInitialized = true;
            }

            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                EditorGUILayout.Space(SECTION_SPACING);
                EditorGUILayout.HelpBox("Enter Play Mode to use the Cheat Menu.", MessageType.Info);
                return;
            }

            IWheelGameService gameService = ServiceLocator.Instance.Get<IWheelGameService>();
            ICheatService cheatService = ServiceLocator.Instance.Get<ICheatService>();

            if (gameService == null)
            {
                EditorGUILayout.Space(SECTION_SPACING);
                EditorGUILayout.HelpBox("Game Service not found! Ensure the game is initialized.", MessageType.Error);
                return;
            }

            if (cheatService == null)
            {
                EditorGUILayout.Space(SECTION_SPACING);
                EditorGUILayout.HelpBox("Cheat Service not found! Ensure CheatService component is in the scene.", MessageType.Error);
                return;
            }

            EditorGUILayout.Space(SECTION_SPACING);
            DrawCheatMenuHeader();

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                DrawSliceInfoSection(gameService);
                DrawQuickActionsSection(gameService, cheatService);
                DrawSpecificControlSection(gameService, cheatService);
                DrawBombChanceSection(cheatService);
                DrawSeedControlSection(cheatService);
                DrawZoneControlSection(gameService);
                DrawGameStateSection(gameService);
                DrawContinuousModeSection(gameService, cheatService);
            }
        }

        private void DrawCheatMenuHeader()
        {
            GUIStyle headerStyle = _headerStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("WHEEL CHEAT MENU", headerStyle);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox("Full control over wheel outcomes for testing and debugging.", MessageType.None);
            EditorGUILayout.Space(10);
        }

        private void DrawSliceInfoSection(IWheelGameService gameService)
        {
            _showSliceInfo = EditorGUILayout.Foldout(_showSliceInfo, "Current Slice Information", true);
            
            if (_showSliceInfo)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    WheelSliceConfig[] slices = gameService.GetWorkingSlices();
                    
                    if (slices != null && slices.Length > 0)
                    {
                        EditorGUILayout.LabelField($"Total Slices: {slices.Length}");
                        
                        for (int index = 0; index < slices.Length; index++)
                        {
                            WheelSliceConfig slice = slices[index];
                            string sliceInfo = slice.IsBomb 
                                ? $"[{index}] BOMB" 
                                : $"[{index}] {slice.RewardType}: {slice.RewardValue}";
                            
                            EditorGUILayout.LabelField(sliceInfo);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No slice data available.", MessageType.Warning);
                    }
                }
            }
            
            EditorGUILayout.Space(10);
        }

        private void DrawQuickActionsSection(IWheelGameService gameService, ICheatService cheatService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Quick Actions", subHeaderStyle);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Force Bomb", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    ExecuteForceBomb(gameService, cheatService);
                }

                if (GUILayout.Button("Avoid Bomb", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    ExecuteAvoidBomb(gameService, cheatService);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Random Spin", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    ExecuteRandomSpin(gameService, cheatService);
                }

                if (GUILayout.Button("Clear Force", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    cheatService.ClearForcedSlice();
                    _continuousForceBomb = false;
                    _continuousAvoidBomb = false;
                }
            }
            
            EditorGUILayout.Space(10);
        }

        private void DrawSpecificControlSection(IWheelGameService gameService, ICheatService cheatService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Specific Slice Control", subHeaderStyle);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Slice Index:", GUILayout.Width(100));
                _specificSliceIndex = EditorGUILayout.IntField(_specificSliceIndex, GUILayout.Width(60));
                
                WheelSliceConfig[] slices = gameService.GetWorkingSlices();
                if (slices != null && slices.Length > 0)
                {
                    _specificSliceIndex = Mathf.Clamp(_specificSliceIndex, 0, slices.Length - 1);
                    
                    WheelSliceConfig targetSlice = slices[_specificSliceIndex];
                    string slicePreview = targetSlice.IsBomb ? "BOMB" : $"{targetSlice.RewardType}: {targetSlice.RewardValue}";
                    EditorGUILayout.LabelField(slicePreview, EditorStyles.boldLabel);
                }
            }

            if (GUILayout.Button($"Force Slice [{_specificSliceIndex}]", GUILayout.Height(BUTTON_HEIGHT)))
            {
                ExecuteForceSpecificSlice(gameService, cheatService, _specificSliceIndex);
            }
            
            EditorGUILayout.Space(10);
        }

        private void DrawBombChanceSection(ICheatService cheatService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Bomb Chance Control", subHeaderStyle);
            
            _cheatBombChance = EditorGUILayout.Slider("💥 Bomb Chance", _cheatBombChance, 
                MINIMUM_BOMB_CHANCE, MAXIMUM_BOMB_CHANCE);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Apply Chance", GUILayout.Height(25)))
                {
                    cheatService.SetBombChance(_cheatBombChance);
                }

                if (GUILayout.Button("0%", GUILayout.Width(40)))
                {
                    _cheatBombChance = 0.0f;
                    cheatService.SetBombChance(_cheatBombChance);
                }

                if (GUILayout.Button("50%", GUILayout.Width(40)))
                {
                    _cheatBombChance = 0.5f;
                    cheatService.SetBombChance(_cheatBombChance);
                }

                if (GUILayout.Button("100%", GUILayout.Width(40)))
                {
                    _cheatBombChance = 1.0f;
                    cheatService.SetBombChance(_cheatBombChance);
                }
            }
            
            EditorGUILayout.Space(10);
        }

        private void DrawSeedControlSection(ICheatService cheatService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Seed Control", subHeaderStyle);
            
            _useCustomSeed = EditorGUILayout.Toggle("Use Custom Seed", _useCustomSeed);
            
            if (_useCustomSeed)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _customSeed = EditorGUILayout.IntField("Seed Value", _customSeed);
                    
                    if (GUILayout.Button("Apply", GUILayout.Width(70)))
                    {
                        cheatService.SetSeed(_customSeed);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Use Random Seed", GUILayout.Height(25)))
                {
                    cheatService.UseRandomSeed();
                }
            }
            
            EditorGUILayout.Space(10);
        }

        private void DrawZoneControlSection(IWheelGameService gameService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Zone Control", subHeaderStyle);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Bronze", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    gameService.SetZoneToBronze();
                }

                if (GUILayout.Button("Silver", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    gameService.SetZoneToSilver();
                }

                if (GUILayout.Button("Gold", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    gameService.SetZoneToGold();
                }
            }
            
            EditorGUILayout.Space(10);
        }

        private void DrawGameStateSection(IWheelGameService gameService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Game State Control", subHeaderStyle);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Reset Game", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    gameService.ResetToInitialState();
                }

                if (GUILayout.Button("Clear Rewards", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    gameService.ClearCollectedRewards();
                }
            }

            int pendingCurrency = gameService.GetPendingCurrency();
            EditorGUILayout.LabelField($"Pending Currency: {pendingCurrency}");
            
            EditorGUILayout.Space(10);
        }

        private void DrawContinuousModeSection(IWheelGameService gameService, ICheatService cheatService)
        {
            GUIStyle subHeaderStyle = _subHeaderStyle ?? EditorStyles.boldLabel ?? new GUIStyle();
            EditorGUILayout.LabelField("Continuous Mode", subHeaderStyle);
            
            Color originalColor = GUI.backgroundColor;
            
            GUI.backgroundColor = _continuousForceBomb ? Color.red : originalColor;
            if (GUILayout.Button(_continuousForceBomb ? "STOP Forcing Bombs" : "START Forcing Bombs", 
                GUILayout.Height(BUTTON_HEIGHT)))
            {
                _continuousForceBomb = !_continuousForceBomb;
                _continuousAvoidBomb = false;
                
                if (_continuousForceBomb)
                {
                    ExecuteForceBombContinuous(cheatService);
                }
                else
                {
                    cheatService.ClearForcedSlice();
                }
            }
            
            GUI.backgroundColor = _continuousAvoidBomb ? Color.green : originalColor;
            if (GUILayout.Button(_continuousAvoidBomb ? "STOP Avoiding Bombs" : "START Avoiding Bombs", 
                GUILayout.Height(BUTTON_HEIGHT)))
            {
                _continuousAvoidBomb = !_continuousAvoidBomb;
                _continuousForceBomb = false;
                
                if (_continuousAvoidBomb)
                {
                    ExecuteAvoidBombContinuous(cheatService);
                }
                else
                {
                    cheatService.ClearForcedSlice();
                }
            }
            
            GUI.backgroundColor = originalColor;
            
            if (_continuousForceBomb || _continuousAvoidBomb)
            {
                EditorGUILayout.HelpBox("Continuous mode active. All spins will follow the selected behavior.", MessageType.Warning);
            }
        }

        private void ExecuteForceBomb(IWheelGameService gameService, ICheatService cheatService)
        {
            if (!CanExecuteCheat(gameService))
            {
                return;
            }

            WheelSliceConfig[] slices = gameService.GetWorkingSlices();
            if (slices == null || slices.Length == 0)
            {
                return;
            }

            bool forced = cheatService.TryForceBombSlice();
            
            if (!forced)
            {
                return;
            }

            gameService.RequestSpin();
        }

        private void ExecuteAvoidBomb(IWheelGameService gameService, ICheatService cheatService)
        {
            if (!CanExecuteCheat(gameService))
            {
                return;
            }

            WheelSliceConfig[] slices = gameService.GetWorkingSlices();
            if (slices == null || slices.Length == 0)
            {
                return;
            }

            bool forced = cheatService.TryForceRandomNonBombSlice();
            
            if (!forced)
            {
                return;
            }

            gameService.RequestSpin();
        }

        private void ExecuteForceSpecificSlice(IWheelGameService gameService, ICheatService cheatService, int sliceIndex)
        {
            if (!CanExecuteCheat(gameService))
            {
                return;
            }

            cheatService.ForceNextSlice(sliceIndex);
            gameService.RequestSpin();
        }

        private void ExecuteRandomSpin(IWheelGameService gameService, ICheatService cheatService)
        {
            if (!CanExecuteCheat(gameService))
            {
                return;
            }

            cheatService.ClearForcedSlice();
            gameService.RequestSpin();
        }

        private void ExecuteForceBombContinuous(ICheatService cheatService)
        {
            cheatService.SetBombChance(1.0f);
        }

        private void ExecuteAvoidBombContinuous(ICheatService cheatService)
        {
            cheatService.SetBombChance(0.0f);
        }

        private bool CanExecuteCheat(IWheelGameService gameService)
        {
            if (gameService == null)
            {
                return false;
            }

            if (!gameService.CanSpin())
            {
                return false;
            }

            return true;
        }

        private void InitializeStyles()
        {
            if (EditorStyles.boldLabel != null)
            {
                _headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter
                };

                _subHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 12
                };
            }
            else
            {
                _headerStyle = new GUIStyle()
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };

                _subHeaderStyle = new GUIStyle()
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                };
            }

            _successButtonStyle = new GUIStyle("button");
            _dangerButtonStyle = new GUIStyle("button");
            _warningButtonStyle = new GUIStyle("button");
        }
    }
}
