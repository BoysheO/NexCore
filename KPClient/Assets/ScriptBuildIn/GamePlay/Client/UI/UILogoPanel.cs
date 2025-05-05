using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Engine.Client.Extensions;
using NormalScripts.BearMonoScript.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptBuildIn.GamePlay.Client.UI
{
    public class UILogoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject txt_title;
        [SerializeField] private Image img_logo;
        private AudioSource _audio;

        private void Awake()
        {
            _audio = gameObject.GetRequireComponent<AudioSource>();
        }

        private void Start()
        {
            txt_title.GetComponent<Graphic>().SetAlpha(0);
            img_logo.SetAlpha(0);
        }

        public async UniTask FadeIn()
        {
            txt_title.GetRequireComponent<Graphic>().DOFade(1f, 0.5f);
            img_logo.DOFade(1f, 0.5f);
            _audio.Play(1000);
            await 1f;
        }
    }
}