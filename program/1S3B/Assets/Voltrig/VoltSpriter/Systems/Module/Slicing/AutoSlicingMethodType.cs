#if UNITY_EDITOR

namespace Voltrig.VoltSpriter
{
    public enum AutoSlicingMethodType
    {
        DeleteAll = 0,

        /// <summary>
        /// Whenever we overlap, we just modify the existing rect and keep its other properties
        /// </summary>
        Smart = 1,

        /// <summary>
        /// We only add new rect if it doesn't overlap existing one
        /// </summary>
        Safe = 2
    }
}
#endif
