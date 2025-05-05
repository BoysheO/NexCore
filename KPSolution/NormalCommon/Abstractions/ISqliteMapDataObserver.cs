using System;

namespace Hotfix.ContentSystems.LocalDbSystem;

public interface ISqliteMapDataObserver
{
    void OnData(int x, int y, int col, DateTimeOffset timeStamp);
    void OnDone();
}