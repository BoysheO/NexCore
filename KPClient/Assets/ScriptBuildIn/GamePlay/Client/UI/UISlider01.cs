using BoysheO.Extensions.Unity3D.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptBuildIn.GamePlay.Client.UI
{
    public class UISlider01 : MonoBehaviour
    {
        private const float startValue = 1f; //Slider组件的初始值
        [SerializeField] private TextMeshProUGUI txt_value;
        [SerializeField] private TextMeshProUGUI txt_info;

        [ShowInInspector]
        [PropertyRange(0, 1)]
        public float Progress
        {
            get
            {
                var raw = gameObject.GetRequireComponent<Slider>().value;
                var v = (raw - startValue) / (100 - startValue);
                return v;
            }
            set
            {
                //[0,1]重映射到[startValue,100]
                var v = startValue + value * (100 - startValue);
                gameObject.GetRequireComponent<Slider>().value = v;
                var txt = txt_value.GetRequireComponent<TextMeshProUGUI>();
                txt.SetText("{0:0.00}%", value * 100);
            }
        }

        [ShowInInspector]
        public string Title
        {
            get => txt_info.GetRequireComponent<TextMeshProUGUI>().text;
            set => txt_info.GetRequireComponent<TextMeshProUGUI>().text = value;
        }
    }
}