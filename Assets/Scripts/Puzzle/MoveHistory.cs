using System.Collections.Generic;

namespace Puzzle
{
    public class MoveHistory
    {
        public Branch start;
        public Branch end;

        public Stack<Bird> startSnapshot;
        public Stack<Bird> endSnapshot;
        public MoveHistory(Branch start, Branch end)
        {
            this.start = start;
            this.end = end;

            startSnapshot = start.Snapshot();
            endSnapshot = end.Snapshot();
        }
    }
}