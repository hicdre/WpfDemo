namespace UI.Native
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification="Not urgent")]
    public struct POINT
    {
        public int x;
        public int y;
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

