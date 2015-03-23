namespace UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification="Zero value is never used")]
    public enum ResizeDirection
    {
        Bottom = 6,
        BottomLeft = 7,
        BottomRight = 8,
        Left = 1,
        Right = 2,
        Top = 3,
        TopLeft = 4,
        TopRight = 5
    }
}

