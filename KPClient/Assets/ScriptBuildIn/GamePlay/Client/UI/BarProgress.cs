using BoysheO.Extensions;
using BoysheO.Util;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptBuildIn.GamePlay.Client.UI
{
    public class BarProgress: MonoBehaviour
    {
        private float minAnchor;

        [ShowInInspector, PropertyRange(0, 1)]
        public float Progress
        {
            get => this._progresss;
            set
            {
                this._progresss = value;
                this.SetDirty();
            }
        }

        private float _progresss;
        private bool _isDirty;

        [SerializeField]
        private Text txt_loading;

        [SerializeField]
        private Image img_loading;

        [SerializeField]
        private Image img_barBoard;

        private void Start()
        {
            img_loading.useSpriteMesh = true;
            img_barBoard.useSpriteMesh = true;
            minAnchor = img_loading.rectTransform.anchorMax.x;
        }

        private void SetDirty()
        {
            if (this._isDirty) return;
            this._isDirty = true;
            this.Refresh().Forget();
        }

        private async UniTask Refresh()
        {
            try
            {
                var go = this.gameObject;
                await UniTask.Yield();
                if (!go) return;
                var anchor = MathLibrary.Remap(this._progresss, 0, 1, minAnchor, 1);
                var max = this.img_loading.rectTransform.anchorMax;
                max.x = anchor;
                this.img_loading.rectTransform.anchorMax = max;
            }
            finally
            {
                this._isDirty = false;
            }
        }

        private void Update()
        {
            if (this.txt_loading != null)
            {
                var dotsCount = Time.timeSinceLevelLoadAsDouble.FloorToInt() % 4;
                using var _ = zstring.Block();
                zstring s = "Loading";
                for (int i = 0; i < dotsCount; i++)
                {
                    s += ".";
                }

                this.txt_loading.text = s;
            }
        }
    }
}