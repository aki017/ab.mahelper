using System.Collections;
using System.Collections.Generic;
using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDKBase;


[assembly: ExportsPlugin(typeof(Ab.MAHelper.AbModularAvatarTogglePlugin))]

namespace Ab.MAHelper
{
    public class AbModularAvatarTogglePlugin : Plugin<AbModularAvatarTogglePlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("AbModularAvatarTogglePlugin", ctx =>
                {
                    foreach (var toggle in ctx.AvatarRootObject.GetComponentsInChildren<AbModularAvatarToggle>())
                    {
                        if (!toggle.enabled)
                        {
                            continue;
                        }
                        var go = toggle.gameObject;
                        go.SetActive(toggle.ShowDefault);
                        var controller = new AnimatorController();

                        var layerName = controller.MakeUniqueLayerName(go.name);
                        var parameterName = "toggle_flag";
                        controller.AddLayer(layerName);
                        var clip1 = new AnimationClip();
                        clip1.name = "off";
                        clip1.SetCurve("", typeof(GameObject), "m_IsActive", AnimationCurve.Linear(0f, 0f, 1f, 0f));
                        var clip2 = new AnimationClip();
                        clip2.name = "on";
                        clip2.SetCurve("", typeof(GameObject), "m_IsActive", AnimationCurve.Linear(0f, 1f, 1f, 1f));

                        var state1 = controller.AddMotion(clip1);
                        var state2 = controller.AddMotion(clip2);

                        controller.AddParameter(parameterName, AnimatorControllerParameterType.Bool);

                        var animatorControllerLayer = controller.layers[controller.layers.Length - 1];
                        var tra1 = state1.AddTransition(state2, false);
                        var tra2 = state2.AddTransition(state1, false);
                        tra1.AddCondition(AnimatorConditionMode.If, 0, parameterName);
                        tra2.AddCondition(AnimatorConditionMode.IfNot, 0, parameterName);
                        tra1.hasExitTime = false;
                        tra2.hasExitTime = false;
                        tra1.duration = 0;
                        tra2.duration = 0;
                        animatorControllerLayer.defaultWeight = 1;
                        animatorControllerLayer.blendingMode = AnimatorLayerBlendingMode.Additive;

                        var animatorControllerParameter = controller.parameters[controller.parameters.Length - 1];
                        var mam = go.AddComponent<ModularAvatarMergeAnimator>();
                        mam.animator = controller;
                        var map = go.AddComponent<ModularAvatarParameters>();
                        map.parameters.Add(new ParameterConfig()
                        {
                            nameOrPrefix = parameterName,
                            internalParameter = true,
                            syncType = ParameterSyncType.Bool,
                            defaultValue = toggle.ShowDefault ? 1 : 0
                        });
                        var mai = go.AddComponent<ModularAvatarMenuItem>();
                        mai.name = toggle.DisplayLabel;
                        mai.Control = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control()
                        {
                            icon = toggle.DisplayTexture,
                            type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle,
                            parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter()
                            {
                                name = parameterName
                            },
                            value = 1f
                        };
                    }
                });
        }
    }
}