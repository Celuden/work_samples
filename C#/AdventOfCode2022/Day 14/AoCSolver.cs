public class AoCSolver
{
    private Dictionary<int, List<string[]>> _inputDict;

    public AoCSolver ()
    {
        _inputDict = new();
    }

    public void SolveDay14(string inputFile)
    {
        // Load in input
        LoadInput(inputFile);

        // Output solution to console
        Console.WriteLine($"D14 - Part One: {SolvePart(1)}");
        Console.WriteLine($"D14 - Part Two: {SolvePart(2)}");
    }

    private void LoadInput(string inputFile)
    {
        int index = 0;
        using (StreamReader sr = new StreamReader(inputFile))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] array = line.Split("->");

                _inputDict[index] = new();
                for (int i = 0; i < array.Length; i++)
                {
                    string[] split = array[i].Split(',');
                    _inputDict[index].Add(split);
                }

                index++;
            }
        }
    }

    private int SolvePart(int partNumber)
    {
        Dictionary<int, List<string>> dict = CreateInputDict(partNumber == 2 ? true : false);
    
        int sandCount = 0;
        int[] sandStart = {500, 0};
        int[] sandPos = (int[])(sandStart.Clone());
        bool sandInVoid = false;
        bool sandIsResting = false;

        while (!sandInVoid)
        {
            // Abort if in void
            if (partNumber == 1)
            {
                if (sandPos[1] > 270)
                {
                    sandInVoid = true;
                    break;
                }
            }
            else
            {
                if (dict[sandStart[1]][sandStart[0]] == "O")
                {
                    sandCount++;
                    sandInVoid = true;
                    break;
                }
            }

            // Pop a new sand if resting
            if (sandIsResting)
            {
                sandPos = (int[])(sandStart.Clone());
                sandCount++;
                sandIsResting = false;
            }
            
            // Sand movement
            if (dict[sandPos[1] + 1][sandPos[0]] == ".")
            {
                sandPos[1]++;
            }
            else if (dict[sandPos[1] + 1][sandPos[0]] == "#" || dict[sandPos[1] + 1][sandPos[0]] == "O")
            {
                // Attempt left
                if (dict[sandPos[1] + 1][sandPos[0] - 1] == ".")
                {
                    sandPos[0]--;
                    sandPos[1]++;
                }
                else if (dict[sandPos[1] + 1][sandPos[0] - 1] == "#" || dict[sandPos[1] + 1][sandPos[0] - 1] == "O")
                {
                    // Attempt right
                    if (dict[sandPos[1] + 1][sandPos[0] + 1] == ".")
                    {
                        sandPos[0]++;
                        sandPos[1]++;
                    }
                    else if (dict[sandPos[1] + 1][sandPos[0] + 1] == "#" || dict[sandPos[1] + 1][sandPos[0] + 1] == "O")
                    {
                        // Sand can't go anywhere, set O and rest.
                        dict[sandPos[1]][sandPos[0]] = "O";
                        sandIsResting = true;
                    }
                }
            }
        }

        return sandCount;
    }
    private Dictionary<int, List<string>> CreateInputDict(bool partTwoDict)
    {
        Dictionary<int, List<string>> result = new();

        // Determine highest y if PartTwo
        int highest_y = 0;
        if (partTwoDict)
        {
            foreach (var kvp in _inputDict)
                foreach (var array in kvp.Value)
                    if (int.Parse(array[1]) > highest_y)
                        highest_y = int.Parse(array[1]);
        }

        // Create empty field
        for (int i = 0; i < 300; i++) // rows
        {
            result[i] = new();
            for (int j = 0; j < 800; j++) // columns
                result[i].Add(".");
        }

        // Fill in with input
        for (int i = 0; i < _inputDict.Count(); i++)
        {
            for (int j = 1; j < _inputDict[i].Count(); j++)
            {
                // Get necessary arrays
                int[] previousArray = {int.Parse(_inputDict[i][j-1][0]), int.Parse(_inputDict[i][j-1][1])};
                int[] currentArray = {int.Parse(_inputDict[i][j][0]), int.Parse(_inputDict[i][j][1])};

                // Fill in start points
                result[previousArray[1]][previousArray[0]] = "#";
                result[currentArray[1]][currentArray[0]] = "#";

                int distanceX = currentArray[0] - previousArray[0];
                if (distanceX < 0) // flip distance if negative
                    distanceX = -distanceX;

                int distanceY = currentArray[1] - previousArray[1];
                if (distanceY < 0) // flip distance if negative
                    distanceY = -distanceY;

                if (distanceX == 0)
                {
                    if (previousArray[1] > currentArray[1])
                        for (int length = previousArray[1]; length >= currentArray[1]; length--)
                            result[length][currentArray[0]] = "#";
                    else
                        for (int length = previousArray[1]; length <= currentArray[1]; length++)
                            result[length][currentArray[0]] = "#";
                }
                else
                {
                    if (previousArray[0] > currentArray[0])
                        for (int length = previousArray[0]; length >= currentArray[0]; length--)
                            result[currentArray[1]][length] = "#";
                    else
                        for (int length = previousArray[0]; length <= currentArray[0]; length++)
                            result[currentArray[1]][length] = "#";
                }
            }
        }

        // Create floor if part two
        if (partTwoDict)
        {
            int floorY = 2 + highest_y;
            for (int i = 0; i < result[floorY].Count(); i++)
                result[floorY][i] = "#";
        }

        return result;
    }
}