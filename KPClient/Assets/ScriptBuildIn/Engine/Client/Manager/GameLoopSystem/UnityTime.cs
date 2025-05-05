using NexCore.DI;
using ScriptEngine.BuildIn.ShareCode.Manager.UnityUpdateExtraSystem;

namespace ScriptEngine.BuildIn.ClientCode.Manager.UnityUpdateExtraSystem
{
    [Service(typeof(IUnityTime))]
    public class UnityTime : IUnityTime
    {
        /// <summary>获取上一帧到当前帧的时间间隔（秒）。</summary>
        public float DeltaTime => UnityEngine.Time.deltaTime;

        /// <summary>获取自游戏开始以来的时间（秒）。</summary>
        public float Time => UnityEngine.Time.time;

        /// <summary>获取自游戏开始以来未受时间缩放影响的时间（秒）。</summary>
        public float UnscaledTime => UnityEngine.Time.unscaledTime;

        /// <summary>获取上一帧到当前帧的时间间隔（秒），未受时间缩放影响。</summary>
        public float UnscaledDeltaTime => UnityEngine.Time.unscaledDeltaTime;

        /// <summary>获取当前时间缩放比例。</summary>
        public float TimeScale
        {
            get => UnityEngine.Time.timeScale;
            set => UnityEngine.Time.timeScale = value;
        }

        /// <summary>获取固定更新的时间步长（秒）。</summary>
        public float FixedDeltaTime
        {
            get => UnityEngine.Time.fixedDeltaTime;
            set => UnityEngine.Time.fixedDeltaTime = value;
        }

        /// <summary>获取上一帧到当前帧的平滑时间间隔（秒）。</summary>
        public float SmoothDeltaTime => UnityEngine.Time.smoothDeltaTime;

        /// <summary>获取自游戏开始以来的固定更新次数。</summary>
        public int FrameCount => UnityEngine.Time.frameCount;

        /// <summary>获取当前实时运行时（秒），此值不受时间缩放影响。</summary>
        public double RealtimeSinceStartup => UnityEngine.Time.realtimeSinceStartup;

        /// <summary>获取当前时间是否处于暂停状态。</summary>
        public bool IsPaused => UnityEngine.Time.timeScale == 0;

        /// <summary>暂停游戏（将时间缩放比例设置为 0）。</summary>
        public void Pause()
        {
            UnityEngine.Time.timeScale = 0;
        }

        /// <summary>恢复游戏（将时间缩放比例设置为 1）。</summary>
        public void Resume()
        {
            UnityEngine.Time.timeScale = 1;
        }
    }
}