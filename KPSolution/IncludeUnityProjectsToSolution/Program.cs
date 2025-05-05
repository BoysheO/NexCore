// See https://aka.ms/new-console-template for more information

using WorkFlow.Command;

Console.WriteLine("IncludeUnityProjectsToSolution!");
var ins = new SyncReference();
await ins.ExecuteAsync();
Console.WriteLine("done");