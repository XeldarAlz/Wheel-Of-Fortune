using UnityEngine;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Infrastructure.Interfaces;
using Random = System.Random;

namespace WheelOfFortune.Core.Services
{
    public sealed class SpinRandomizerService : ISpinRandomizer
    {
        private const int INVALID_SLICE_INDEX = -1;
        private const float MINIMUM_CHANCE = 0.0f;
        private const float MAXIMUM_CHANCE = 1.0f;

        private readonly float _baseBombChance;
        private readonly float _bombChanceIncrement;
        private readonly float _maximumBombChance;
        private readonly int _incrementIntervalZones;

        private Random _random;
        private float _runtimeBombChance;
        private float _currentBombChance;
        private bool _hasForcedSlice;
        private bool _hasRuntimeBombChance;
        private int _forcedSliceIndex;

        public SpinRandomizerService(float baseBombChance, float bombChanceIncrement, float maximumBombChance,
            int incrementIntervalZones, bool useCustomSeed, int customSeed)
        {
            _baseBombChance = Mathf.Clamp(baseBombChance, MINIMUM_CHANCE, MAXIMUM_CHANCE);
            _bombChanceIncrement = Mathf.Clamp(bombChanceIncrement, MINIMUM_CHANCE, MAXIMUM_CHANCE);
            _maximumBombChance = Mathf.Clamp(maximumBombChance, MINIMUM_CHANCE, MAXIMUM_CHANCE);
            _incrementIntervalZones = incrementIntervalZones;

            if (useCustomSeed)
            {
                SetSeed(customSeed);
            }
            else
            {
                UseRandomSeed();
            }
        }

        public void SetSeed(int seed)
        {
            _random = new Random(seed);
        }

        public void UseRandomSeed()
        {
            _random = new Random();
        }

        public int ResolveSliceIndex(WheelSliceConfig[] slices, int currentZoneIndex)
        {
            if (slices == null || slices.Length == 0)
            {
                return 0;
            }

            if (_hasForcedSlice)
            {
                int forcedIndex = Mathf.Clamp(_forcedSliceIndex, 0, slices.Length - 1);
                _hasForcedSlice = false;
                return forcedIndex;
            }

            int bombSliceIndex = FindBombSliceIndex(slices);

            if (bombSliceIndex < 0)
            {
                return _random.Next(0, slices.Length);
            }

            float bombChance = GetBombChance(currentZoneIndex);
            double roll = _random.NextDouble();

            if (roll < bombChance)
            {
                return bombSliceIndex;
            }

            int nonBombIndex = GetRandomNonBombSliceIndex(slices, bombSliceIndex);

            return nonBombIndex >= 0 
                ? nonBombIndex 
                : _random.Next(0, slices.Length);
        }

        public void ForceNextSlice(int sliceIndex)
        {
            if (sliceIndex < 0)
            {
                ClearForcedSlice();
                return;
            }

            _forcedSliceIndex = sliceIndex;
            _hasForcedSlice = true;
        }

        public void ClearForcedSlice()
        {
            _forcedSliceIndex = INVALID_SLICE_INDEX;
            _hasForcedSlice = false;
        }

        public void SetBombChance(float chance)
        {
            float clampedChance = Mathf.Clamp(chance, MINIMUM_CHANCE, MAXIMUM_CHANCE);
            _runtimeBombChance = clampedChance;
            _currentBombChance = clampedChance;
            _hasRuntimeBombChance = true;
        }

        private float GetBombChance(int currentZoneIndex)
        {
            if (_hasRuntimeBombChance)
            {
                _currentBombChance = _runtimeBombChance;
                return _currentBombChance;
            }

            if (currentZoneIndex <= 1)
            {
                _currentBombChance = MINIMUM_CHANCE;
                return _currentBombChance;
            }

            int interval = Mathf.Max(1, _incrementIntervalZones);
            int zoneIndex = Mathf.Max(1, currentZoneIndex);
            int increments = (zoneIndex - 1) / interval;
            float progressiveChance = _baseBombChance + (increments * _bombChanceIncrement);
            _currentBombChance = Mathf.Clamp(progressiveChance, _baseBombChance, _maximumBombChance);

            return _currentBombChance;
        }

        private static int FindBombSliceIndex(WheelSliceConfig[] slices)
        {
            if (slices == null)
            {
                return INVALID_SLICE_INDEX;
            }

            for (int index = 0; index < slices.Length; index++)
            {
                if (slices[index].IsBomb)
                {
                    return index;
                }
            }

            return INVALID_SLICE_INDEX;
        }

        private int GetRandomNonBombSliceIndex(WheelSliceConfig[] slices, int bombSliceIndex)
        {
            if (slices == null || slices.Length == 0)
            {
                return bombSliceIndex;
            }

            int nonBombCount = 0;

            for (int index = 0; index < slices.Length; index++)
            {
                if (!slices[index].IsBomb)
                {
                    nonBombCount++;
                }
            }

            if (nonBombCount <= 0)
            {
                return bombSliceIndex;
            }

            int target = _random.Next(0, nonBombCount);

            for (int index = 0; index < slices.Length; index++)
            {
                if (slices[index].IsBomb)
                {
                    continue;
                }

                if (target == 0)
                {
                    return index;
                }

                target--;
            }

            return bombSliceIndex;
        }
    }
}
