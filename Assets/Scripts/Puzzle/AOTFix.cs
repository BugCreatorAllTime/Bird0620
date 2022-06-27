namespace Puzzle
{
    public class AOTFix
    {
        private static void OnlyForIL2CPPAOT() {
            new BranchInvokable(4.0f, "");
        }
    }
    
    class BranchInvokable : InvokableCallback<int, Branch> {
        public BranchInvokable(object target, string methodName) : base(target, methodName) { }
    }
}