using BoysheO.Toolkit;
using NexCore.DI;

namespace ScriptHotfix.GamePlay.Share.Manager.ExampleSystem
{
    [Service]
    public class HelloWorldManager
    {
        private SRandom random;

        public int[] CreatInitData()
        {
            random = new();
            var state = random.GetState();
            return state;
        }
        
        public void Init(int[] saveData)
        {
            random = new SRandom(saveData);
        }

        public int GetNext()
        {
            return random.NextInteger();
        }
    }
}