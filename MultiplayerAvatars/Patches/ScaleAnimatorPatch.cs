using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Tweening;
using UnityEngine;

namespace MultiplayerAvatars.Patches
{
    // Harmony patches that are required so that the avatar IK doesn't freak out.
    // The original animator scales the avatar to and from 0, which is what makes it freak out.
    // Solution: change the animator's parameters so that it scales to 0.05 instead.

    [HarmonyPatch(typeof(ScaleAnimator), nameof(ScaleAnimator.InitIfNeeded))]
    internal static class ScaleAnimatorPatch
    {
        private static readonly FieldInfo _scaleUpTweenField = typeof(ScaleAnimator).GetField("_scaleUpTween", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo _scaleDownTweenField = typeof(ScaleAnimator).GetField("_scaleDownTween", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo _constructScaleUpTween = SymbolExtensions.GetMethodInfo(() => ConstructScaleUpTween(0f, 0f, null!, 0f, 0, 0f));
        private static readonly MethodInfo _constructScaleDownTween = SymbolExtensions.GetMethodInfo(() => ConstructScaleDownTween(0f, 0f, null!, 0f, 0, 0f));
        private static readonly ConstructorInfo _constructFloatTween = typeof(FloatTween).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(Action<float>), typeof(float), typeof(EaseType), typeof(float) });

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Stfld, _scaleUpTweenField)) // Find where the scale up tween is set
                .MatchBack(false, new CodeMatch(OpCodes.Newobj, _constructFloatTween)) // Find where the scale up tween is created (should be before where it's set)
                .Set(OpCodes.Callvirt, _constructScaleUpTween) // Replace constructor with ours
                .MatchForward(false, new CodeMatch(OpCodes.Stfld, _scaleDownTweenField)) // Find where the scale down tween is set
                .MatchBack(false, new CodeMatch(OpCodes.Newobj, _constructFloatTween)) // Find where the scale down tween is created (should be before where it's set)
                .Set(OpCodes.Callvirt, _constructScaleDownTween) // Replace constructor with ours
                .InstructionEnumeration(); // Enumerate

        private static FloatTween ConstructScaleUpTween(float fromValue, float toValue, Action<float> onUpdate, float duration, EaseType easeType, float delay)
            => new FloatTween(0.05f, toValue, onUpdate, duration, easeType, delay);

        private static FloatTween ConstructScaleDownTween(float fromValue, float toValue, Action<float> onUpdate, float duration, EaseType easeType, float delay)
            => new FloatTween(fromValue, 0.05f, onUpdate, duration, easeType, delay);
    }

    [HarmonyPatch(typeof(ScaleAnimator), nameof(ScaleAnimator.HideInstant))]
    internal static class ScaleInstantPatch
    {
        private static readonly MethodInfo _setLocalScaleAttacher = SymbolExtensions.GetMethodInfo(() => SetLocalScaleAttacher(null!, Vector3.one));
        private static readonly MethodInfo _setLocalScaleMethod = typeof(Transform).GetProperty("localScale").SetMethod;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Callvirt, _setLocalScaleMethod)) // Find where setLocalScale is called
                .Set(OpCodes.Callvirt, _setLocalScaleAttacher) // Replace that call with ours
                .InstructionEnumeration(); // Enumerate

        private static void SetLocalScaleAttacher(Transform transform, Vector3 scale)
            => transform.localScale = Vector3.one * 0.05f;
    }
}
