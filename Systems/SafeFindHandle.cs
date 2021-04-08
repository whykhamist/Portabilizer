using Microsoft.Win32.SafeHandles;
using System.Security;

namespace Systems
{
    [SecurityCritical]
    internal class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityCritical]
        public SafeFindHandle() : base(true)
        { }

        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.FindClose(base.handle);
        }
    }
}
