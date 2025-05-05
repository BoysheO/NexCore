namespace ScriptEngine.BuildIn.ShareCode.Manager.UnityUpdateExtraSystem
{
    /// <summary>
    /// IUpdateManager和IUnityEnvironment中的Update时间信息
    /// </summary>
    public interface IUnityTime
    {
        /// <summary>获取上一帧到当前帧的时间间隔（秒）。</summary>
        float DeltaTime { get; }

        /// <summary>获取自游戏开始以来的时间（秒）。</summary>
        float Time { get; }

        /// <summary>获取自游戏开始以来未受时间缩放影响的时间（秒）。</summary>
        float UnscaledTime { get; }

        /// <summary>获取上一帧到当前帧的时间间隔（秒），未受时间缩放影响。</summary>
        float UnscaledDeltaTime { get; }

        /// <summary>获取当前时间缩放比例。</summary>
        float TimeScale { get; set; }

        /// <summary>获取固定更新的时间步长（秒）。</summary>
        float FixedDeltaTime { get; set; }

        /// <summary>获取上一帧到当前帧的平滑时间间隔（秒）。</summary>
        float SmoothDeltaTime { get; }

        /// <summary>获取自游戏开始以来的固定更新次数。</summary>
        int FrameCount { get; }

        /// <summary>获取当前实时运行时（秒），此值不受时间缩放影响。</summary>
        double RealtimeSinceStartup { get; }

        /// <summary>获取当前时间是否处于暂停状态。</summary>
        bool IsPaused { get; }

        /// <summary>暂停游戏（将时间缩放比例设置为 0）。</summary>
        void Pause();

        /// <summary>恢复游戏（将时间缩放比例设置为 1）。</summary>
        void Resume();
    }
}