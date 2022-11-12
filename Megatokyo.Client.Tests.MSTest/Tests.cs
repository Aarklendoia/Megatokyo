using Megatokyo.Client.ViewModels;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Client.Tests.MSTest
{
    // TODO: Add appropriate tests
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        // TODO: Add tests for functionality you add to MainViewModel.
        [TestMethod]
        public void TestMainViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new MainViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to SettingsViewModel.
        [TestMethod]
        public void TestSettingsViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new SettingsViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to StripGalleryViewModel.
        [TestMethod]
        public void TestStripGalleryViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new StripGalleryViewModel();
            Assert.IsNotNull(vm);
        }
    }
}
