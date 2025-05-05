using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptBuildIn.GamePlay.Client.UI
{
    public class UIMessageBox: MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI txt_title;

        [SerializeField]
        private TextMeshProUGUI txt_ok;

        [SerializeField]
        private TextMeshProUGUI txt_no;

        [SerializeField]
        private Button btn_ok;

        [SerializeField]
        private Button btn_no;

        public enum Result
        {
            No,
            Ok,
        }

        private Result? _res;

        private void Start()
        {
            this.btn_ok.onClick.AddListener(() => { this._res = Result.Ok; });
            this.btn_no.onClick.AddListener(() => { this._res = Result.No; });
        }

        public async UniTask<Result> Go(string content, [NotNull] string yes, [CanBeNull] string no)
        {
#if DEBUG
            Debug.Log($"[{nameof(UIMessageBox)}]{content}?{yes}:{no}");
#endif
            var go = this.gameObject;
            if (yes == null)
            {
                throw new ArgumentNullException(nameof (yes));
            }

            this.txt_title.text = content;
            this.txt_ok.text = yes;
            if (no != null)
            {
                this.txt_no.gameObject.SetActive(true);
                this.txt_no.text = no;
            }
            else
            {
                this.txt_no.gameObject.SetActive(false);
            }

            while (this._res == null)
            {
                if (go == null) throw new OperationCanceledException();
                await UniTask.Yield();
            }

            if (go == null) throw new OperationCanceledException();
            Destroy(this.gameObject);
            var r = this._res.Value;
#if DEBUG
            Debug.Log($"[{nameof(UIMessageBox)}]answer:{r}");
#endif
            return r;
        }
    }
}