using ModuleCapture.ViewModels;
using Xunit;

namespace ModuleCapture.Test
{
    public class ViewCaptureTest
    {
        private readonly ViewCaptureViewModel _vm = new();

        [Fact]
        public void GetDeviceTest()
        {
            Assert.Equal(_vm.NameDevice, _vm.ListDevice[_vm.SelectedDevice]);
        }
    }
}
