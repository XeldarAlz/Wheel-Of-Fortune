using System.Collections;
using UnityEngine;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.View.Haptics
{
    public sealed class HapticPlayer : MonoBehaviour, IHapticPlayer
    {
        private const long LIGHT_VIBRATION_MS = 10;
        private const long MEDIUM_VIBRATION_MS = 20;
        private const long HEAVY_VIBRATION_MS = 40;
        private const float TICK_VIBRATION_INTERVAL = 0.1f;

        private static readonly WaitForSeconds WarningDelay = new(0.1f);
        private static readonly WaitForSeconds SuccessDelay = new(0.05f);
        private static readonly WaitForSeconds ErrorDelay = new(0.15f);
        private static readonly WaitForSeconds ContinuousDelay = new(TICK_VIBRATION_INTERVAL);

        private bool _isVibratingContinuous;
        private Coroutine _continuousCoroutine;

        public void TriggerHaptic(long durationMs)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            TriggerAndroidVibration(durationMs);
            #elif UNITY_IOS && !UNITY_EDITOR
            TriggerIOSVibration(durationMs);
            #endif
        }

        public void PlayWarningPattern()
        {
            StartCoroutine(WarningPatternCoroutine());
        }

        public void PlaySuccessPattern()
        {
            StartCoroutine(SuccessPatternCoroutine());
        }

        public void PlayErrorPattern()
        {
            StartCoroutine(ErrorPatternCoroutine());
        }

        public void StartContinuousTicks()
        {
            if (_isVibratingContinuous)
            {
                return;
            }

            _isVibratingContinuous = true;
            _continuousCoroutine = StartCoroutine(ContinuousTickPatternCoroutine());
        }

        public void StopContinuousTicks()
        {
            _isVibratingContinuous = false;

            if (_continuousCoroutine == null)
            {
                return;
            }

            StopCoroutine(_continuousCoroutine);
            _continuousCoroutine = null;
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
        private void TriggerAndroidVibration(long durationMs)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                {
                    if (IsAndroidAPILevel26OrHigher())
                    {
                        using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                        {
                            int effectId = GetVibrationEffectForDuration(durationMs);
                            AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                                "createOneShot", durationMs, effectId);
                            vibrator.Call("vibrate", vibrationEffect);
                        }
                    }
                    else
                    {
                        vibrator.Call("vibrate", durationMs);
                    }
                }
            }
            catch (System.Exception)
            {
                Handheld.Vibrate();
            }
        }

        private bool IsAndroidAPILevel26OrHigher()
        {
            using (AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
                return sdkInt >= 26;
            }
        }

        private int GetVibrationEffectForDuration(long durationMs)
        {
            if (durationMs <= LIGHT_VIBRATION_MS)
            {
                return 1;
            }
            else if (durationMs <= MEDIUM_VIBRATION_MS)
            {
                return 0;
            }
            else if (durationMs <= HEAVY_VIBRATION_MS)
            {
                return 2;
            }
            else
            {
                return 5;
            }
        }
        #endif

        #if UNITY_IOS && !UNITY_EDITOR
        private void TriggerIOSVibration(long durationMs)
        {
            if (durationMs <= LIGHT_VIBRATION_MS)
            {
                TriggerIOSImpact(0);
            }
            else if (durationMs <= MEDIUM_VIBRATION_MS)
            {
                TriggerIOSImpact(1);
            }
            else if (durationMs <= HEAVY_VIBRATION_MS)
            {
                TriggerIOSImpact(2);
            }
            else
            {
                TriggerIOSNotification(1);
            }
        }

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _TriggerIOSImpactFeedback(int style);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _TriggerIOSNotificationFeedback(int type);

        private void TriggerIOSImpact(int style)
        {
            try
            {
                _TriggerIOSImpactFeedback(style);
            }
            catch
            {
                Handheld.Vibrate();
            }
        }

        private void TriggerIOSNotification(int type)
        {
            try
            {
                _TriggerIOSNotificationFeedback(type);
            }
            catch
            {
                Handheld.Vibrate();
            }
        }
        #endif

        private IEnumerator WarningPatternCoroutine()
        {
            TriggerHaptic(MEDIUM_VIBRATION_MS);
            yield return WarningDelay;
            TriggerHaptic(MEDIUM_VIBRATION_MS);
            yield return WarningDelay;
            TriggerHaptic(HEAVY_VIBRATION_MS);
        }

        private IEnumerator SuccessPatternCoroutine()
        {
            TriggerHaptic(LIGHT_VIBRATION_MS);
            yield return SuccessDelay;
            TriggerHaptic(MEDIUM_VIBRATION_MS);
        }

        private IEnumerator ErrorPatternCoroutine()
        {
            TriggerHaptic(HEAVY_VIBRATION_MS);
            yield return ErrorDelay;
            TriggerHaptic(HEAVY_VIBRATION_MS);
        }

        private IEnumerator ContinuousTickPatternCoroutine()
        {
            while (_isVibratingContinuous)
            {
                TriggerHaptic(LIGHT_VIBRATION_MS);
                yield return ContinuousDelay;
            }

            _continuousCoroutine = null;
        }

        private void OnDestroy()
        {
            StopContinuousTicks();
        }
    }
}
