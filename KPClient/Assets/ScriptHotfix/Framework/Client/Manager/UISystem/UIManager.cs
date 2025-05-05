// ReSharper disable all 
// ReSharper disable once CheckNamespace

#nullable enable
using System;
using System.Data;
using System.Reflection;
using System.Threading;
using NexCore.DI;
using NexCore.UnityEnvironment;
using BoysheO.Collection2;
using BoysheO.Extensions;
using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using Extensions;
using Hotfix.FrameworkSystems.GameManagerSystem;
using Hotfix.ResourceMgr.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using UISystem.Abstractions;
using UISystem.Implements.UIContainers;
using UnityEngine;
using UnityEngine.EventSystems;
using YooAsset;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;


namespace UISystem
{
    /// <summary>
    /// 设计note:
    /// 1.制作UI逻辑
    ///     1.快速制作普通UI逻辑
    ///         继承<see cref="Hotfix.UIScripts.Common.UIBase"/>，然后开梭UI脚本即可。可以直接将UI脚本挂在Prefab根节点上。
    ///     2.常驻UI的制作
    ///         需要自定义加载流程和自定义销毁逻辑。
    ///         1.实现IUILoader并在Loader中保有一个UI实例。仅在UI实例没有创建时创建一次
    ///         2.创建UI脚本，继承MonoBehaviour和<see cref="IView"/>
    ///         3.实现<see cref="IUIContainerItem.OnRemovedFromUIContainer"/>函数，在这里将GameObject隐藏
    ///     3.浮动文字UI制作
    ///         
    /// 
    /// 2.加载UI
    ///     1.调用方调用UIManager，资源管理器加载prefab并以Inactived状态克隆到<see cref="IUIContainer"/>的预挂载节点<see cref="IUIContainer.ParentForInstantiate"/>
    ///         *鉴于实践情况，需要向开发者强调一点，UI逻辑实现类请勿使用分部类。对于逻辑居多的UI，可使用组合模式创建多个功能实例而不是统统塞到UI实例代码里。
    ///     2.如果是自定义的Loader加载流程，则在这一步会给prefab上的UI对象（继承了<see cref="IView"/>接口的<see cref="MonoBehaviour"/>对象）注入属性<see cref="IView.Loader"/>。使用无Loader加载流程是不会进行注入。
    ///     3.然后通过SetActive(true)+SetActive(false)触发Unity执行克隆节点的Awake()函数
    ///         注意如果有OnEnable和OnDisable逻辑，则在这一步会触发Unity执行OnEnable+OnDisable，特别注意这个时候Start函数仍未运行，开发者有义务保证OnEnable+OnDisable在此情况下能正确运行;
    ///         最佳实践是使用<see cref="IUIContainerItem.OnUIPushed"/>/<see cref="IUIContainerItem.OnUIPopped"/>来处理而不是OnEnable/OnDisable
    ///     4.执行UI对象的<see cref="IView.OnUILoaded"/>方法
    ///     5.添加到容器<see cref="IUIContainer"/>内。容器对象被注入到UI对象属性<see cref="IUIContainerItem.UIContainer"/>。
    ///         由于注入先于Start执行，因此可以在MonoBehaviour.Start()里使用这些注入的值
    ///         添加到容器内并不会立即显示UI，此刻UI在场景中是Deactive状态，直到调用方调用UI上的<see cref="IUIContainer.PushUI"/>方法（在UIBase里，它被封装成Show()），这是为了预加载UI的功能准备的。提前加载好ui，战斗里就必临时加载了
    ///     6.返回UI对象（<see cref="IView"/>）给调用方
    ///         到此，UI已经加载好，调用方也拿到了UI的引用。此时，如果UI需要一些临时参数，可以在此时对IView实例强转为具体实现，然后调用实例上合适的成员来进行传入参数。注意，此时UI如未调用<see cref="IUIContainer.PushUI"/>方法，则Start()尚未执行
    ///         要显示UI(SetActive(true))，调用<see cref="IUIContainer.PushUI"/> 隐藏ui调用<see cref="IUIContainer.PopUI"/>  注意，UI的active状态现在是被UIContainer管理的，不要越过UIContainer去改变UI对象的状态
    ///
    /// 3.自定义加载流程
    ///     1.实现<see cref="IUILoader"/>接口。
    ///         *命名规范上，Loader应与对应UI同名，例如UIMainView的Loader是UIMainViewLoader
    ///         *UILoader应与UIPanel放在同一个文件中，以方便查阅、防止出现多次实现UILoader的意外；
    ///     2.调用方使用<see cref="UIManager.LoadUIViaLoaderAsync"/>来加载UI
    /// 标准无Loader加载流程是全新加载，即调用1次加载API就加载1次，可以创建出多个界面。
    /// 如果加载UI需要拉取网络数据，就需要自定义加载流程，一般流程是这样的：
    ///     发起者异步调用加载API（LoadUIViaLoaderAsync）→ UIManager调用Loader上的加载流程         UIManager返回UI对象→UI与Loader交互获取相关状态
    ///                                                                 ↓                           ↑
    ///                                                             Loader请求网络->请求成功，状态储存到Loader中，执行加载流程
    ///                                                                 ↓
    ///                                                      请求网络失败，抛异常中断发起者的流程
    /// 如果UI需要池化/常驻/单例等需要自己管理生命周期，例如背包、浮动文字等，也可通过Loader实现。执行加载流程时，返回池中或单例的对象即可。
    /// UI本身的状态如果需要全局常驻，请储存到Loader中。Loader兼具这个UI的管理器职责。例如，某个界面需要记忆上次打开时的分页状态/滚动条位置，应当记忆到Loader中
    ///
    /// 4.销毁UI
    /// 调用<see cref="IUIContainer.Remove"/>即可
    /// UIContainer没有销毁GameObject的职责，因此销毁UI的GameObject是自行管理的。Remove流程会执行UI对象的<see cref="IUIContainerItem.OnRemovedFromUIContainer"/>，可在此自行管理GameObject的生命周期。
    /// 如果UI继承了<see cref="Hotfix.UIScripts.Common.UIBase"/>，UIBase的<see cref="IUIContainerItem.OnRemovedFromUIContainer"/>实现会执行<see cref="UnityEngine.Object.Destroy(UnityEngine.Object)"/>
    /// 如果UI没有继承，则在Remove流程结束后，会有一个脱离了UIContainer管理的GameObject要自己管理。常驻UI往往需要自己管理，多不继承UIBase
    ///
    /// 5.置顶UI
    /// 调用ui.UIContainer.Push(ui);时，会将UI置顶。也可手动置顶ui.UIContainer.Top(ui); 一般只用push就行了。置顶只限于容器顶层，不能越过容器。
    /// 
    /// 一些废弃的实现思路
    ///    上个版本的实现思路是由UILoader创建UI，UIManager只做一次薄的加载loader的逻辑，然后大部分无状态逻辑由UIHelper承载，各个UILoader
    /// 使用UIHelper来实现默认加载流程。这个版本的UIManager还支持传递形参到UI实例上。但是
    /// 实践中发现UILoader创建太多了，每个UI都要创建一个UILoader，而真正有定制化需求的情况其实是很少的，这浪费了一些工时。所以这个版本一开始就是要
    /// 思考抛弃掉UILoader 这里说下UILoader以前的设计理念，UILoader要处理好加载流程的异步加载（特别是网络数据，要实现网络数据没加载到就不让ui资源的加载发生，造成性能浪费）
    /// 和通过UILoader这一层分离开UI实例和后台逻辑的耦合（想法是如果要改UI，只要改动UILoader的加载逻辑就行了）。落地时，
    /// 对网络数据的处理确实效果不错，但是分离耦合这一点做得并不好。事实上后台逻辑强耦合UILoader和强耦合UI实例也许并没有什么不同，增加UILoader这
    /// 一层只不过是浪费工时而已。现实游戏开发里很多UI的业务都是相互紧密耦合的，解耦只能解一点点，所以不能过度追求解耦。在去掉UILoader这个基础思路
    /// 上，我本想设计一条静态函数约定，约定UI实例如果实现这个静态函数就可以实现和UILoader一样的功能，而不用单独写一个UILoader。但是担心这样的话
    /// 静态成员会泛滥，而且这个约定并没有语法支持，要加写Roslyn，对我不友好😒。所以这个方案先搁置吧。C#11 .NET 7 有static abstract
    /// 接口成员支持，到Unity支持的时候再来看看。见"https://learn.microsoft.com/zh-cn/dotnet/csharp/fundamentals/types/interfaces"。
    /// 现在的方案是Loader变为可选实现的了。
    /// </summary>
    [Service(typeof(UIManager))]
    public sealed class UIManager
    {
        private const bool IsDEBUG = BuildInConstConfig.IsDebug;
        private const bool IsLogUIOpenCall = IsDEBUG && false;
        private const bool IsLogStandardUIContainerFallback = IsDEBUG && false;
        private const bool IsEnableInject = true;

        public GameObject UIManagerRoot
        {
            get
            {
                if (_uiManagerRoot) return _uiManagerRoot;
                _uiManagerRoot = GetUIManagerRoot();
                return _uiManagerRoot;
            }
        }

        public Canvas UICanvas
        {
            get
            {
                if (!_uiCanvas)
                {
                    _uiCanvas = UIManagerRoot.transform.Find("Canvas")
                        .GetRequireComponent<Canvas>();
                }

                return _uiCanvas;
            }
        }

        public EventSystem EventSystem
        {
            get
            {
                if (!_eventSystem)
                {
                    _eventSystem = UIManagerRoot.transform.Find("EventSystem")
                        .GetRequireComponent<EventSystem>();
                }

                return _eventSystem;
            }
        }

        public Camera UICamera
        {
            get
            {
                if (!_uiCamera)
                {
                    _uiCamera = UICanvas.worldCamera.ThrowIfNullOrFakeNull();
                }

                return _uiCamera;
            }
        }

        private GameObject _uiManagerRoot = null!;
        private Canvas _uiCanvas = null!;
        private EventSystem _eventSystem = null!;
        private Camera _uiCamera = null!;
        private bool _isLayerInitialed;
        private readonly IGameServiceProvider _serviceProvider;
        private readonly IUILoaderProvider _uiLoaderProvider;
        private readonly IUnityEnvironment _unityEnvironment;

        /// <summary>
        /// 获取layer对应的节点
        /// *有些特殊UI不是通过Container管理显示的，例如全局遮罩覆盖在所有UIContainer上方。这些ui需要查询具体节点的位置以确定自己的位置
        /// </summary>
        public GameObject GetLayerNode(UILayer layer)
        {
            if (!_isLayerInitialed)
            {
                var layers = Enum.GetValues(typeof(UILayer));
                // var names = Enum.GetNames(typeof(UILayer));
                foreach (UILayer lay in layers)
                {
                    var go = new GameObject(lay.ToString());
                    go.transform.SetParent(UICanvas.gameObject.transform);
                    var rect = go.GetOrAddComponent<RectTransform>();
                    rect.AnchorCornersAndFull();
                    rect.transform.SetLocalPositionZ(0);

                    #region 添加容器类

                    switch (lay)
                    {
                        //暂时已知的所有都按StandardContainer处理
                        case UILayer.ScenePanel:
                        case UILayer.TopBar:
                        case UILayer.Panel:
                        {
                            var c = go.GetOrAddComponent<StandardUIContainer>();
                            c.ServiceProvider = _serviceProvider.ServiceProvider;
                            break;
                        }

                        //todo Cursor层容器需要跟随鼠标移动。
                        case UILayer.Window:
                        {
                            var c = go.GetOrAddComponent<WindowUIContainer>();
                            c.ServiceProvider = _serviceProvider.ServiceProvider;
                            break;
                        }
                        //     //todo 有bug，待完善
                        // case UILayer.Popup:
                        // {
                        //     var c = go.GetOrAddComponent<MaskedContainer>();
                        //     c.ServiceProvider = _serviceProvider;
                        //     break;
                        // }

                        default:
                        {
                            if (IsLogStandardUIContainerFallback)
                            {
                                Debug.Log($"{lay} fallback to {nameof(StandardUIContainer)}");
                            }

                            var c = go.GetOrAddComponent<StandardUIContainer>();
                            c.ServiceProvider = _serviceProvider.ServiceProvider;
                            break;
                        }
                    }

                    #endregion
                }

                _isLayerInitialed = true;
            }

            var layerName = layer.ToString();
            return UICanvas.gameObject.RequireChild(layerName);
        }

        /// <summary>
        /// 获取预置UILayer的UIContainer
        /// </summary>
        public IUIContainer GetLayerContainer(UILayer layer)
        {
            var node = GetLayerNode(layer);
            return node.GetRequireComponent<IUIContainer>();
        }

        private GameObject GetUIManagerRoot()
        {
            var go = GameObject.FindWithTag("UIManager");
            if (go) return go;
            var gm = _serviceProvider.ServiceProvider.GetRequiredService<GameManager>();
            var uiMgrRoot = gm.Root.RequireChild("UIManager");
            return uiMgrRoot.gameObject;
        }

        /// <summary>
        /// see<see cref="GetUILoader"/>
        /// </summary>
        public TLoader GetUILoader<TLoader>() where TLoader : class, IUILoader
        {
            return (TLoader)GetUILoader(typeof(TLoader));
        }

        /// <summary>
        ///*有些UI需要一些UI层面上的全局数据管理，例如记忆上次列表滚动位置，这类数据存在Loader中
        /// </summary>
        public IUILoader GetUILoader(Type type)
        {
            if (!type.IsInheritsFrom(typeof(IUILoader)))
                throw new ArgumentException($"{type.Name} not implement {nameof(IUILoader)}");
            if (!Application.isPlaying) throw new OperationCanceledException();
            var ins = _uiLoaderProvider.GetRequiredService(type);
            return (IUILoader)ins;
        }

        /// <summary>
        /// 使用loader直接指定的layer，在容器上加载UI
        /// </summary>
        /// <param name="cancellationToken">没有明确需求，不要使用默认值以外的值以免浪费gc。已做了默认Unity生命周期管理，不要传UnityEnvironment的Token进来</param>
        public async UniTask<IView> LoadUIViaLoaderAsync(Type uiLoaderType,
            CancellationToken cancellationToken = default)
        {
            if (!uiLoaderType.IsInheritsFrom(typeof(IUILoader))) throw new Exception($"not support {uiLoaderType}");
            var loader = GetUILoader(uiLoaderType);
            var container = GetLayerContainer(loader.Layer);
            var ui = await loader.LoadUIAsync(container, cancellationToken);
            return ui;
        }

        /// <summary>
        /// see<see cref="LoadUIAsync(System.Type,UISystem.Abstractions.UILayer,System.Threading.CancellationToken)"/>
        /// </summary>
        public UniTask<TView> LoadUIAsync<TView>(UILayer uiLayer, CancellationToken cancellationToken = default)
            where TView : IView
        {
            var container = GetLayerContainer(uiLayer);
            return UILoaderLoadUIAsUsualAsync<TView>(null, container, cancellationToken);
        }

        /// <summary>
        /// 使用无loader流程在对应的layer容器上加载UI
        /// *大部分ui不需要自定义loader，按标准流程加载即可。
        /// </summary>
        /// <param name="cancellationToken">没有明确需求，不要使用默认值以外的值以免浪费gc。已做了默认Unity生命周期管理，不要传UnityEnvironment的Token进来</param>
        public UniTask<IView> LoadUIAsync(Type uiType, UILayer uiLayer, CancellationToken cancellationToken = default)
        {
            var container = GetLayerContainer(uiLayer);
            return UILoaderLoadUIAsUsualAsync(uiType, null, container, cancellationToken);
        }

        public async UniTask<TView> LoadUIAsync<TView>(CancellationToken cancellationToken = default)
            where TView : class, IView
        {
            var type = typeof(TView);
            var ui = await LoadUIAsync(type, cancellationToken);
            return ui as TView ?? throw new NullReferenceException("missing ui");
        }

        /// <summary>
        /// 加载并显示UI
        /// </summary>
        public async UniTask<TView> ShowUIAsync<TView>(CancellationToken cancellationToken = default)
            where TView : class, IView
        {
            _unityEnvironment.CancellationToken.ThrowIfCancellationRequested();
            var ui = await LoadUIAsync<TView>(cancellationToken);
            ui.UIContainer.PushUI(ui);
            return ui;
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="cancellationToken">没有明确需求，不要使用默认值以外的值以免浪费gc。已做了默认Unity生命周期管理，不要传UnityEnvironment的Token进来</param>
        public async UniTask<IView> LoadUIAsync(Type uiType, CancellationToken cancellationToken = default)
        {
            _unityEnvironment.ThrowIfUnityNotPlay();
            var atb = uiType.GetCustomAttribute<UILayerAttribute>();
            if (atb == null) throw new Exception($"type={uiType} missing {nameof(UILayerAttribute)}");
            var ui = await LoadUIAsync(uiType, atb.UILayer, cancellationToken);
            return ui;
        }

        // /// <summary>
        // /// 使用loader在对应的layer容器上加载UI(泛型版本） *API冲突，因此直接舍弃
        // /// </summary>
        // public UniTask<IView> LoadUIAsync<TLoader>(CancellationToken cancellationToken = default)
        //     where TLoader : IUILoader
        // {
        //     return LoadUIAsync(typeof(TLoader), cancellationToken);
        // }

        /// <summary>
        /// 获取UI根上对应的layer容器
        /// </summary>
        /// <returns></returns>
        public VList<IUIContainer> GetLayerContainers()
        {
            var lst = VList<IUIContainer>.Rent();
            var values = Enum.GetValues(typeof(UILayer));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                var layer = (UILayer)values.GetValue(i);
                var container = GetLayerContainer(layer);
                lst.Add(container);
            }

            return lst;
        }

        /// <summary>
        /// 获取UI根上的所有容器的所有UI
        /// </summary>
        /// <returns></returns>
        public VList<IUIContainerItem> GetUIsAllLayer()
        {
            VList<IUIContainerItem> result = VList<IUIContainerItem>.Rent();
            using var containers = GetLayerContainers();
            foreach (var uiContainer in containers)
            {
                using var uis = VList<IUIContainerItem>.Rent();
                uiContainer.GetUIList(uis.InternalBuffer);
                foreach (var uiContainerItem in uis)
                {
                    result.Add(uiContainerItem);
                }
            }

            return result;
        }

        /// <summary>
        /// 查找并返回UI根上已经存在的UI。
        /// *某些UI和业务是深度绑定的，例如HomePanel、BattlePanel这类，
        /// *还有某些UI需要检查场景中是否具有UI，没有就打开、有就置顶这样的逻辑
        /// *如果是简单的逻辑可以用这个直接实现
        /// *但是复杂的逻辑应与Manager或Loader交互，或从中取得这种深度绑定的Panel进行交互。而不是通过UIMgr。
        /// *原因是容器内的UI不一定是以IView的形式存在的，并且可能同名
        /// </summary>
        public IView? FindUI(string name)
        {
            _unityEnvironment.ThrowIfUnityNotPlay();
            // var actuallyName = string.Concat(name, "(Clone)");
            var actuallyName = name;
            using var uis = GetUIsAllLayer();
            using var res = uis.InternalBuffer.FindAll(v => v.GetType().Name == actuallyName);
            if (res.Count == 0)
            {
                return null;
            }
            else if (res.Count > 1)
            {
                Debug.LogError($"more than one ui match.ui={name}");
            }

            return res[0] as IView;
        }

        /// <summary>
        ///  UILoader使用的标准加载流程
        /// </summary>
        /// <param name="token">没有明确需求，不要使用默认值以外的值以免浪费gc。已做了默认Unity生命周期管理，不要传UnityEnvironment的Token进来</param>
        public async UniTask<T> UILoaderLoadUIAsUsualAsync<T>(IUILoader? loader,
            IUIContainer? containerOverrider = null,
            CancellationToken token = default) where T : IView
        {
            var ins = await UILoaderLoadUIAsUsualAsync(typeof(T), loader, containerOverrider, token);
            return (T)ins;
        }


        /// <summary>
        ///  UILoader使用的标准加载流程
        /// </summary>
        /// <param name="token">没有明确需求，不要使用默认值以外的值以免浪费gc。已做了默认Unity生命周期管理，不要传UnityEnvironment的Token进来</param>
        public async UniTask<IView> UILoaderLoadUIAsUsualAsync(Type viewType, IUILoader? loader,
            IUIContainer? containerOverrider = null,
            CancellationToken token = default)
        {
            if (viewType == null) throw new ArgumentNullException(nameof(viewType));
            if (!viewType.IsClassAndImplement(typeof(IView)))
                throw new Exception($"type dismatch sealed IView,type={viewType}");
            if (IsLogUIOpenCall)
            {
                Debug.Log($"[{nameof(UIManager)}]Load {viewType.Name}");
            }

            token = token == default ? _unityEnvironment.CancellationToken : token;
            using var handle = YooAsset.YooAssets.LoadAssetAsync<GameObject>(viewType.Name + ".prefab");
            await handle;
            token.ThrowIfCancellationRequested();

            if (containerOverrider == null)
            {
                if (loader == null)
                {
                    var atb = viewType.GetCustomAttribute<UILayerAttribute>();
                    if (atb == null)
                    {
                        // @formatter:off
                        throw new Exception("UILoader、Container、DefaultUILayer至少提供一个非null参数，否则uiManager不知道UI应挂载到什么容器下");
                        // @formatter:on
                    }

                    containerOverrider = GetLayerContainer(atb.UILayer);
                }
                else
                {
                    containerOverrider = GetLayerContainer(loader.Layer);
                }
            }

            var prefab = handle.GetAssetObject<GameObject>();
            bool isActivityPrefab = prefab.activeSelf;
            if (isActivityPrefab) prefab.SetActive(false); //防止唤醒
            var clone = UnityEngine.Object.Instantiate(prefab, containerOverrider.ParentForInstantiate.transform);
            clone.name = viewType.Name; //统一一下名字，方便观察、代码搜索
            if (isActivityPrefab) prefab.SetActive(true); //还原预制体状态以免影响Editor里的状态
            // var c = clone.AddComponent(typeof(T));//hclr变成自带的了
            var view = clone.GetComponent(typeof(IView)) as IView;
            if (view == null)
            {
                //由于历史原因，一些prefab没有挂脚本，这里要补充上
                view = clone.AddComponent(viewType) as IView;
                if (view == null)
                {
                    throw new Exception($"{viewType}所对应的预制体没有找到实现IView接口的脚本，需要自己另外实现UILoaderLoadUIAsUsualAsync流程");
                }
            }

            view.Loader = loader;
            //只对sealed类进行注入。这样可以节约一些消耗
            if (IsEnableInject && viewType.IsSealed)
            {
                InjectAttribute.Inject(_serviceProvider.ServiceProvider, view); //注入
            }

            view.GameObject.Show().Hide(); //awake
            try
            {
                await view.OnUILoaded();
            }
            catch (Exception ex)
            {
                if (view is MonoBehaviour uobj)
                {
                    Debug.Log(
                        $"由于{view.GetType().Name}.{nameof(view.OnUILoaded)}异常[{ex.Message}]，将销毁{view.GetType().Name}");
                    UnityEngine.Object.Destroy(uobj.gameObject);
                }

                throw;
            }

            containerOverrider.Add(view);
            return view;
        }

        public UIManager(IUILoaderProvider uiLoaderProvider,
            IUnityEnvironment unityEnvironment, IGameServiceProvider serviceProvider)
        {
            _uiLoaderProvider = uiLoaderProvider;
            _unityEnvironment = unityEnvironment;
            _serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// 将世界坐标转换为Hud空间的2D世界坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Vector2 WorldPosToHudWorldPos(Vector3 worldPos)
        {
            var uiMgr = DIContext.ServiceProvider.GetRequiredService<UIManager>();
            var mainCam = Camera.main.ThrowIfNullOrFakeNull();
            var screenPoint = mainCam.WorldToScreenPoint(worldPos);
            var uiCam = uiMgr.UICamera;
            var wPos = uiCam.ScreenToWorldPoint(screenPoint);
            return wPos;
        }
    }
}