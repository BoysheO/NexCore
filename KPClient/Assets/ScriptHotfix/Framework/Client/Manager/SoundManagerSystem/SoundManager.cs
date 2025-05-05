using System;
using System.Collections.Generic;
using NexCore.DI;
using BoysheO.Extensions.Unity3D.Extensions;
using HotScripts.Hotfix.SharedCode.SharedCode.Extensions;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NexCore.UnityEnvironment;
using BoysheO.Collection2;
using BoysheO.Extensions;
using Cysharp.Threading.Tasks;
using Hotfix.ContentSystems.LocalDbSystem;
using Hotfix.FrameworkSystems.GameManagerSystem;
using Hotfix.ResourceMgr.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SharedCode.Define;
using Sirenix.OdinInspector;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;

namespace HotScripts.Hotfix.GamePlay.FrameworkSystems.SoundManagerSystem
{
    [Service(typeof(SoundManager))]
    public sealed class SoundManager
    {
        private const bool IsDEBUG = BuildInConstConfig.IsDebug;
        private const float FallbackVolume = 0.5f;//初始化音量

        private class SoundTrack
        {
            private const float fadeSpeed = 1 / 0.2f;
            public string RawAudioKey;
            public ResourceId AudioSourceToken;
            public float LocalVolume;
            public AudioSource UsingAudioSource;
            [ShowInInspector, ReadOnly] private Guid _fadeRequestId;
            [ShowInInspector, ReadOnly] private float _fadeProgress = 0;
            private readonly SoundManager _soundManager;
            public bool ReleaseRequesting;

            public SoundTrack(SoundManager soundManager)
            {
                _soundManager = soundManager;
            }

            public void OnGlobalVolumeUpdate()
            {
                if (_fadeRequestId == Guid.Empty)
                {
                    if (_soundManager._muteId2Tracking.Count > 0)
                    {
                        UsingAudioSource.enabled = false;
                    }
                    else
                    {
                        UsingAudioSource.enabled = true;
                        UsingAudioSource.volume = LocalVolume * _soundManager._volume;
                    }
                }
            }

            public void FadeOut()
            {
                var id = Guid.NewGuid();
                _fadeRequestId = id;
                Go().Forget();

                async UniTaskVoid Go()
                {
                    await _soundManager._resMgr.WaitAsync(AudioSourceToken);
                    while (_fadeRequestId == id)
                    {
                        _fadeProgress = Mathf.Max(0, _fadeProgress - fadeSpeed * Time.deltaTime);
                        var volume = _fadeProgress * _soundManager.Volume * LocalVolume;
                        if (UsingAudioSource == null) throw new OperationCanceledException();//UnityEditor退出播放模式
                        UsingAudioSource.volume = _soundManager.IsMute ? 0 : Mathf.Clamp01(volume);
                        if (volume == 0)
                        {
                            _fadeRequestId = Guid.Empty;
                            if (ReleaseRequesting) Release();
                            return;
                        }

                        await UniTask.Yield();
                    }
                }

                // Observable.EveryUpdate()
                //     .Timeout(TimeSpan.FromMinutes(1)) //预防死锁
                //     .SkipWhile(_ => AudioSourceToken.Progress < 1)
                //     .TakeWhile(_ => _fadeRequestId == id)
                //     .Subscribe(_ =>
                //     {
                //         _fadeProgress = Mathf.Max(0, _fadeProgress - fadeSpeed * Time.deltaTime);
                //         var volume = _fadeProgress * _soundManager.Volume * LocalVolume;
                //         UsingAudioSource.volume = _soundManager.IsMute ? 0 : Mathf.Clamp01(volume);
                //         if (volume == 0)
                //         {
                //             _fadeRequestId = Guid.Empty;
                //             if (ReleaseRequesting) Release();
                //         }
                //     });
            }

            public void FadeIn()
            {
                var id = Guid.NewGuid();
                _fadeRequestId = id;
                Go().Forget();

                async UniTaskVoid Go()
                {
                    await _soundManager._resMgr.WaitAsync(AudioSourceToken);
                    if (_fadeRequestId != id) return; //request is changed
                    if (UsingAudioSource.clip == null)
                    {
                        var clip = _soundManager._resMgr.GetResource<AudioClip>(AudioSourceToken);
                        if (clip == null)
                        {
                            var k = _soundManager._resMgr.GetKey(AudioSourceToken);
                            Debug.LogWarning($"clip={k.Value} is missing");
                        }

                        UsingAudioSource.clip = clip;
                        UsingAudioSource.loop = true;
                        UsingAudioSource.Play();
                    }

                    while (_fadeRequestId == id)
                    {
                        _fadeProgress = Mathf.Min(1, _fadeProgress + fadeSpeed * Time.deltaTime);
                        var volume = _fadeProgress * _soundManager.Volume * LocalVolume;
                        UsingAudioSource.volume = _soundManager.IsMute ? 0 : Mathf.Clamp01(volume);
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (volume == 1)
                        {
                            _fadeRequestId = Guid.Empty;
                            return;
                        }

                        await UniTask.Yield();
                    }
                }

                // Observable.EveryUpdate()
                //     .Timeout(TimeSpan.FromMinutes(1)) //预防死锁
                //     .SkipWhile(_ => AudioSourceToken.Progress < 1)
                //     .TakeWhile(_ => _fadeRequestId == id)
                //     .Subscribe(_ =>
                //     {
                //         if (UsingAudioSource.clip == null)
                //         {
                //             var clip = AudioSourceToken.GetAssetObject<AudioClip>();
                //             if (clip == null)
                //             {
                //                 Debug.LogWarning($"clip={AudioSourceToken.}");
                //             }
                //
                //             UsingAudioSource.clip = clip;
                //             UsingAudioSource.loop = true;
                //             UsingAudioSource.Play();
                //         }
                //
                //         _fadeProgress = Mathf.Min(1, _fadeProgress + fadeSpeed * Time.deltaTime);
                //         var volume = _fadeProgress * _soundManager.Volume * LocalVolume;
                //         UsingAudioSource.volume = _soundManager.IsMute ? 0 : Mathf.Clamp01(volume);
                //         // ReSharper disable once CompareOfFloatsByEqualityOperator
                //         if (volume == 1) _fadeRequestId = Guid.Empty;
                //     });
            }

            private void Release()
            {
                UsingAudioSource.Stop();
                _soundManager.PushAudioSource(UsingAudioSource);
                UsingAudioSource = null;
                _fadeRequestId = Guid.Empty;
                AudioSourceToken.Dispose();
                _soundManager._bgmTrack.Remove(this);
            }
        }

        private class BGMRequest1
        {
            public Guid Id;
            public string RawAudioKey;
            public float VolumeFac;
        }

        private readonly List<AudioSource> _audioSource = new(2);
        [ShowInInspector] private readonly List<SoundTrack> _bgmTrack = new();
        [ShowInInspector] private readonly List<BGMRequest1> _bgmRequest1s = new();
        [ShowInInspector] private readonly Dictionary<Guid, string> _muteId2Tracking = new();

        public event Action onGlobalVolumeChanged;

        private readonly IResourceManager _resMgr;
        private readonly PlayerPrefsKeysManager _keysManager;

        public SoundManager( IResourceManager resMgr, IUnityEnvironment environment, PlayerPrefsKeysManager keysManager)
        {
            _resMgr = resMgr;
            _keysManager = keysManager;
        }

        public UniTask InitAsync()
        {
            var gm = DIContext.ServiceProvider.GetRequiredService<GameManager>();
            _ = gm.Root;
            var exist = Camera.main.ThrowIfNullOrFakeNull().GetComponents<AudioSource>();
            _audioSource.AddRange(exist);

            if (PlayerPrefs.HasKey(_keysManager.GlobalVolumeKey))
            {
                _volume = PlayerPrefs.GetFloat(_keysManager.GlobalVolumeKey).Clamp01();
            }
            else
            {
                _volume = FallbackVolume;
            }
            
            return UniTask.CompletedTask;
        }

        private float _volume;

        [ShowInInspector]
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value.Clamp01();
                PlayerPrefs.SetFloat(_keysManager.GlobalVolumeKey,_volume);
                NotifyGlobalVolumeChanged();
            }
        }

        [ShowInInspector]
        public bool IsMute
        {
            get => _muteId2Tracking.Count > 0;
        }

        /// <summary>
        /// 设置Bgm播放
        /// </summary>
        [Button]
        public Guid PlayBGM(string key, float extVolumeFac = 1)
        {
            var request = new BGMRequest1
            {
                RawAudioKey = key,
                Id = Guid.NewGuid(),
                VolumeFac = extVolumeFac,
            };
            _bgmRequest1s.Add(request);
            UpdateBGMTracks();
            return request.Id;
        }

        [Button]
        public void CancelBGM(Guid requestId)
        {
            var request = _bgmRequest1s.FirstOrDefault(v => v.Id == requestId);
            if (request == null) return;
            _bgmRequest1s.Remove(request);
            UpdateBGMTracks();
        }

        /// <summary>
        /// 轻量化地播放1次声音;没有中途改变声音大小的功能
        /// </summary>
        public async UniTask PlaySound(AudioClip clip, VolumeLayer volumeLayer, CancellationToken competedToken = default,
            Action<TimeSpan>? onProgress = null)
        {
            var src = PopAudioSource();
            var volume = PlayerPrefs.GetFloat(_keysManager.GetSoundVolumeLayerKey(volumeLayer));
            src.volume = volume.Clamp01();
            if (onProgress == null)
            {
                src.PlayOneShot(clip);
            }
            else
            {
                //这种方式才能获取到播放进度
                src.clip = clip;
                src.loop = false;
                src.Play();
            }

            try
            {
                while (src.isPlaying && !competedToken.IsCancellationRequested)
                {
                    if (onProgress != null)
                    {
                        var time = src.time;
//                        Debug.Log(time);
                        var sec = TimeSpan.FromSeconds(time);
                        onProgress(sec);
                    }

                    await UniTask.Yield();
                }
            }
            finally
            {
                //如果是突然关闭游戏，则src会被释放。此时src为null
                if (src != null)
                {
                    if (src.isPlaying) src.Stop();
                    PushAudioSource(src);
                }
            }
        }

        private void UpdateBGMTracks()
        {
            using var buffer = VList<SoundTrack>.Rent();
            //渐隐
            {
                for (int i = 0; i < _bgmRequest1s.Count - 2; i++)
                {
                    var audioKey = _bgmRequest1s[i].RawAudioKey;
                    var track = _bgmTrack.FirstOrDefault(v => v.RawAudioKey == audioKey);
                    if (track != null)
                    {
                        track.FadeOut();
                        buffer.Add(track);
                    }
                }
            }
            //渐入
            {
                var bgmReq = _bgmRequest1s.LastOrDefaultLgc();
                if (bgmReq != null)
                {
                    var audioKey = bgmReq.RawAudioKey;
                    var track = _bgmTrack.FirstOrDefault(v => v.RawAudioKey == audioKey);
                    if (track == null)
                    {
                        track = new SoundTrack(this)
                        {
                            LocalVolume = bgmReq.VolumeFac,
                            ReleaseRequesting = false,
                            AudioSourceToken = _resMgr.BeginLoad(new ResourceKey(audioKey, typeof(AudioClip)), default),
                            UsingAudioSource = PopAudioSource(),
                            RawAudioKey = audioKey
                        };
                    }

                    track.FadeIn();
                    _bgmTrack.Add(track);
                    buffer.Add(track);
                }
            }
            //清理bgm
            {
                foreach (var soundTrack in _bgmTrack.Except(buffer.InternalBuffer))
                {
                    if (!soundTrack.ReleaseRequesting)
                    {
                        soundTrack.ReleaseRequesting = true;
                        soundTrack.FadeOut();
                    }
                }
            }
        }

        AudioSource PopAudioSource()
        {
            if (_audioSource.Count > 0)
            {
                var last = _audioSource.Count - 1;
                var item = _audioSource[last];
                _audioSource.RemoveAt(last);
                return item;
            }
            else
            {
                var c = Camera.main.ThrowIfNullOrFakeNull().gameObject.AddComponent<AudioSource>();
                return c;
            }
        }

        void PushAudioSource(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            _audioSource.Add(source);
        }

        [Button]
        public Guid Mute([CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            var id = Guid.NewGuid();
            string tracking = IsDEBUG ? $"{memberName} at {sourceFilePath}:{sourceLineNumber}" : "";
            if (IsDEBUG) Debug.Log($"[{nameof(SoundManager)}]mute at {tracking}");
            _muteId2Tracking.Add(id, tracking);
            NotifyGlobalVolumeChanged();
            return id;
        }

        [Button]
        public void DisMute(Guid muteRequestId)
        {
            if (IsDEBUG)
            {
                var tracking = _muteId2Tracking.GetValueOrDefault(muteRequestId);
                Debug.Log($"[{nameof(SoundManager)}]dismute at {tracking ?? "(missing)"}");
            }

            _muteId2Tracking.Remove(muteRequestId);
            NotifyGlobalVolumeChanged();
        }

        private void NotifyGlobalVolumeChanged()
        {
            for (var index = 0; index < _bgmTrack.Count; index++)
            {
                var soundTrack = _bgmTrack[index];
                soundTrack.OnGlobalVolumeUpdate();
            }

            onGlobalVolumeChanged?.Invoke();
        }
    }
}