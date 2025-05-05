using System;

namespace WorkspaceConfigReaderSystem
{
    public class WorkspaceConfigModel
    {
        public Secret Secret { get; init; }
        public ClientSetting ClientSetting { get; init; }
    }

    public class Secret
    {
        [Obsolete("use Cert_File instead")] public string PublicKey { get; init; }
        [Obsolete("use Pfx_File instead")] public string PrivateKey { get; init; }

        public string Cert_File { get; init; }

        public string Pfx_File { get; init; }
    }

    public class ClientSetting
    {
        public bool EnableSetting { get; init; }
    }
}