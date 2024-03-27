#if COM_UNITY_2D_SPRITE
namespace Voltrig.VoltSpriter 
{
    public class VS
    {
        private static VS _inst;

        internal static VS inst
        {
            get 
            {
                if (_inst == null)
                {
                    _inst = new VS();
                }

                return _inst;
            }
        }

        private VSGUIDAsset<VSStyle> _style = new VSGUIDAsset<VSStyle>("VS_StyleGUID", "125cae4475b04a44984b333ef74c31d0");
        private VSModule _module;
        private VSWindow _window;

        internal VSStyle style { get => _style.asset; }

        internal VSModule module
        {
            get => _module;

            set
            {
                _module = value;
                OnModuleChangeE?.Invoke(_module);
            }
        }

        internal VSWindow window 
        {
            get => _window; 
            
            set
            {
                _window = value;
                OnWindowChangeE?.Invoke(_window);
            } 
        }

        internal event System.Action<VSModule> OnModuleChangeE;
        internal event System.Action<VSWindow> OnWindowChangeE;
    }
}
#endif