using nadena.dev.modular_avatar.core;
using UnityEngine;

namespace Ab.MAHelper
{
    public class AbModularAvatarToggle : AvatarTagComponent
    {
        [Header("メニューの名前")]
        public string DisplayLabel = "名前";
        [Header("メニューのアイコン")]
        public Texture2D DisplayTexture;

        [Header("デフォルトの表示状態")]
        public bool ShowDefault;

        // ReSharper disable once Unity.RedundantEventFunction
        void Start()
        {
            // Ensure that unity generates an enable checkbox
        }
    }
}