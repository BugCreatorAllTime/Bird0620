using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class SaveGameData
{
    public int selectedBranch = 0;
    public List<int> unlockedBranchs = new List<int>(5) {0};

    public int selectedBackground = 0;
    public List<int> unlockBackgrounds = new List<int>(5) {0};

    public int freeBranch = ConfigGame.FreeBranch;
    public int freeShuffle = ConfigGame.FreeShuffle;
    public int freeUndo = ConfigGame.FreeUndo;

    public int currentStage = 0;
}
