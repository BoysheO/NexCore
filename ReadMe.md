# 欢迎使用NexCore框架

本框架本质上是前后端分离框架，但如果后端选用C#的话，一些代码是可以共享使用的。共享使用代码的话，要求开发者精通并熟知前后端的代码环境区别。
请直接下载代码库的zip使用

## 快速部署

1. 准备好环境：
   - 安装net9.0环境
   - 安装git环境
   - 安装Unity（不同平台需要安装不同模块）：
     - Win上开发：安装Win的IL2CPP模块、安卓IL2CPP模块、Ios开发模块
     - Mac上开发：安装Mac的IL2CPP模块、安卓IL2CPP模块、Ios的IL2CPP模块
   - 保证能连上github（因为有一些插件是直连github的）

2. 将工程下载到本地，打开`environmentconfig.toml`，根据工程本地环境编辑  
   - `KPClient`、`KPSolution`两个文件夹可以自由重命名，只要`environmentconfig.toml`信息正确即可
   - 在快速部署阶段建议先保持默认名称

3. 编辑`environmentconfig.toml`后打开Solution（如果已经打开，则需要重新打开以确保EnvironmentConfig更新）

4. 依次编译执行：
   - `BuildDirectoryBuildProps`
   - `RunTableGen`
     - Mac注意事项：
       - 需要管理员权限（会提示输入锁屏密码）
       - 需要给`unityprotocwithodin/macosx_arm64/protoc`赋予运行权限：
         1. `chmod +x protoc`
         2. 运行并在系统设置中允许其执行
       - 如果在授权前运行过RunTableGen，需要清理工程缓存
     - Windows注意事项：
       - 会跳UAC提示
   - 策划独立打表工具暂未包含在框架中，后续可能补充

5. 打开Unity工程：
   - 可能会提示编译失败，忽略并继续进入工程
   - 进入后会自动修改相关宏命令，修改完毕会重新编译
   - 编译完成后即可正常使用

## 第一次出包（以安卓为例）

1. 用Unity打开KPClient工程，确认编译成功（如失败请到群里反馈）
2. 安装HCLR：
   - 点击菜单`HybridCLR/Installer`
   - 点击`Install`安装
3. HCLR生成：
   - 点击菜单`HybridCLR/Generate/All`
4. 复制HCLR生成文件到资源目录：
   - 点击菜单`Engine/HCLRHelper/GenPatchBytesAll`
5. 复制热更代码到资源目录：
   - 点击菜单`Engine/HCLRHelper/CompileHotDllBytes/Current`
6. 打空资源包：
   - 点击菜单`YooAsset/AssetBundleBuilder`
   - 在弹出窗口中：
     - `CopyBuildinFileOption`选择`OnlyCopyByTags`
     - `CopyBuildinFileParam`填任意数（示例填111）
   - 点击`ClickBuild`
7. 查看本地ip：
   - 打开hfs，会显示电脑ip
8. 修改配置：
   - 打开`StreamingAssets/Appsetting.json`
   - 修改`ResourceHost`/`ResourceHostFallback`/`HelloWorldService`的url为上面显示的ip
9. 连接安卓手机：
   - 点击Unity菜单`File/BuildAndRun`
   - 等待构建完成（首次运行APP会报错连接服务器失败，这是正常的）
10. 重新出资源包：
    - 点击`YooAsset/AssetBundleBuilder`
    - `CopyBuildinFileOption`设置为`ClearAndCopyAll`
    - 点击`ClickBuild`构建
11. 架设资源服务器：
    - 在hfs控制面板中：
      - 将目录`StreamingAssets/yoo/DefaultPackage`挂载为`http://<ip>/CDN/Android`
12. 运行HelloWorld服务器：
    - 打开`KPSolution/KPSolution.sln`
    - 编译运行`Server/Services/HelloWorldService.csproj`
13. 重新执行APK：
    - 现在APK应该会提示更新
    - 进入热更逻辑后会显示当前时间

## 快速出热更代码包

1. 改写Hotfix代码
2. 执行菜单`Engine/HCLRHelper/CompileHotDllBytes/Current`
3. 使用`YooAsset/AssetBundleBuilder`出一包资源包
4. 重启安卓手机应用APK，会提示代码更新

## 使用规范

### 代码生成规范

1. 生成的代码中必须有注释，说明：
   - 代码是脚本生成的
   - 由哪个脚本生成
   - 提示开发者手动修改是否合适

2. 生成的文件使用UTF8编码

### Editor菜单代码编写原则

1. 执行第一行打印代码所在位置（`DebugUtil.GetCallerContext`）
2. 长时间执行的逻辑/IO操作完成后打印日志"Done"
3. 涉及IO操作的要打印相关日志

### 代码命名规范

- 事件使用小写`on`开头，例如`onUpdate`，`onCompeted`等

### 宏使用规范

1. 克制使用宏，不要放到函数外（不利于代码优化期追踪）
2. 优先使用Const常量处理（编译器会对Const条件分支进行裁剪）

### Json序列化规范

- 所有POCO类应以`Model`为后缀，例如`AppsettingModel`等

## 为框架增加通用功能/修复bug的步骤

1. 先在github上提issue（以github上为准）
2. 如果能自己解决，提PR（会合到主干上）
   - 可以学习如何提PR

