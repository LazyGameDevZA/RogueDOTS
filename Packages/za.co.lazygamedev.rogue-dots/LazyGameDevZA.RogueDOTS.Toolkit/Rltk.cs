namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    public struct DisplayConsole<TConsole> where TConsole: unmanaged, IConsole
    {
        public TConsole Console;
    }
    
    public struct Rltk
    {
    }
}
