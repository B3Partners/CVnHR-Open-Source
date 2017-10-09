using StructureMap;

namespace QNH.Overheid.KernRegister.BatchProcess
{
    public static class Structuremap
    {
        public static void Start()
        {
            var container = IocConfig.Container;
        }
    }
}