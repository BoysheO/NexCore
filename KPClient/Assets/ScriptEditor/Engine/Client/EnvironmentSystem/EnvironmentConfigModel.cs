// ReSharper disable once CheckNamespace

using Nett;

public sealed class EnvironmentConfigModel
{
    [TomlMember(Key = "root")]
    public string Root { get; set; }
    [TomlMember(Key = "client")]
    public string Client { get; set; }
    [TomlMember(Key = "solution")]
    public string Solution { get; set; }
    [TomlMember(Key = "unity_editor_dlls")]
    public string UnityEditorDlls { get; set; }
}