namespace Voltrig.VoltSpriter
{
    public static class VSErrors 
    {
        public const string REFLECTION_ERROR = "[VoltSpriter] Reflection error. The current version of Unity is not supported.";
        public const string INSTALLATION_ERROR = "[VoltSpriter] Detected corruption of the data files. To fix please reimport the asset.";
        public const string LOADCONF_WRONG_ID = "[VoltSpriter] wrong LoadConfiguration function parameter. ID has to be between 0 and 9, inclusive.";
        public const string INDEX_OUTOFBOUNDS = "[VoltSpriter] ID is out of bounds. ID has to be between 0 and 9, inclusive.";
        public const string WRONG_ASSET_PATH = "[VoltSpriter] wrong asset path. Does the asset exist in the assets folder?";
        public const string NULL_ERROR = "[VoltSpriter] Used a null parameter when not-null was expected.";
        public const string SPRITE_EDITOR_MISSING = "[VoltSpriter] Missing 2D Sprite, which is a sprite editor package. Please make sure that this package is installed to use Volt Spriter.";
    }
}

