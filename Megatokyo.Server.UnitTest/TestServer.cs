using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    internal class TestServer
    {
        private static APIClient? _apiClient;

        [AssemblyInitialize()]
        public static void Init(TestContext context)
        {
            _apiClient = new("/api/1.0");
        }

        public static APIClient Client()
        {
            if (_apiClient != null)
            {
                return _apiClient;
            }
            return _apiClient = new("/api/1.0");
        }
    }
}
