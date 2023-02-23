// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using System.IO;
using System.Collections.Generic;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

internal class CutsceneManager
{
    private List<Sprite> _dialogueSprites;
    private RectangleShape backGroundRect;
    private Font font;
    private uint characterSize;
    private GameState _gameState;
    private int currentIndex;
    private int windowSizeX;
    private int windowSizeY;
    private List<Text> level01Dialogue;
    private List<Text> level02Dialogue;
    private List<Text> level03Dialogue;
    private List<Text> level04Dialogue;
    private List<Text> level05Dialogue;
    private List<Text> creditsDialogue;
    private List<Text> currentDialogue;
    private bool endCutscene;
    private Color infoColor;
    private bool fadeColor;
    private bool creditsOn;
    private int fadeCounter;
    private int fadeInterval;
    private int sfxInterval;
    private Text infoText1;
    private Text infoText2;
    private Sound nextText;

    // Color
    private Color maroColor;
    private Color veniaColor;
    private Color volvoColor;
    public CutsceneManager()
    {
        maroColor = new Color(200, 0, 0);
        veniaColor = new Color(255, 102, 102);
        volvoColor = new Color(215, 215, 215);
    }
    public void Initialize()
    {
        currentIndex = 0;
        characterSize = 20;
        endCutscene = false;
        windowSizeX = 600;
        windowSizeY = 900;
        currentDialogue = new();
        _dialogueSprites = new();
        font = AssetManager.Instance.fonts["Font"];
        infoColor = new Color(255, 255, 255, 0);
        fadeColor = true;
        fadeCounter = 0;
        fadeInterval = 20;
        sfxInterval = 0;
        creditsOn = false;
        nextText = AssetManager.Instance.sounds["TextSFX"];

        // Sprites
        Sprite maroFace = new Sprite(AssetManager.Instance.textures["MaroFace"]);
            maroFace.Scale = new Vector2f(0.5f, 0.5f);
            maroFace.Origin = new Vector2f(maroFace.TextureRect.Width /2 , maroFace.TextureRect.Height / 2);
            maroFace.Position = new Vector2f(0, -100);
        Sprite veniaFace = new Sprite(AssetManager.Instance.textures["VeniaFace"]);
            veniaFace.Scale = new Vector2f(0.5f, 0.5f);
            veniaFace.Origin = new Vector2f(veniaFace.TextureRect.Width /2 , veniaFace.TextureRect.Height / 2);
            veniaFace.Position = new Vector2f(0, -100);
        Sprite volvoFace = new Sprite(AssetManager.Instance.textures["VolvoFace"]);
            volvoFace.Scale = new Vector2f(0.5f, 0.5f);
            volvoFace.Origin = new Vector2f(volvoFace.TextureRect.Width /2 , volvoFace.TextureRect.Height / 2);
            volvoFace.Position = new Vector2f(0, -100);
        _dialogueSprites.Add(maroFace); _dialogueSprites.Add(veniaFace); _dialogueSprites.Add(volvoFace);

        // Set Background
        backGroundRect = new RectangleShape(new Vector2f(windowSizeX, windowSizeY));
        backGroundRect.Position = new Vector2f(-windowSizeX / 2, -windowSizeY / 2);
        backGroundRect.FillColor = Color.Black;


        // Set Dialogue for Level01
        level01Dialogue = new();
        LoadDialogueList(level01Dialogue, "cutscenes/level01.txt");

            // Modifying Level 01 Dialogue
            level01Dialogue[1].CharacterSize = characterSize + 5;
            level01Dialogue[1].Origin = new Vector2f(level01Dialogue[1].GetGlobalBounds().Width / 2, level01Dialogue[1].GetGlobalBounds().Height / 2);
            level01Dialogue[1].Position = new Vector2f(0, 0);

        // Set Dialogue for Level02
        level02Dialogue = new();
        LoadDialogueList(level02Dialogue, "cutscenes/level02.txt");

            // Modifying Level 02 Dialogue
            level02Dialogue[1].CharacterSize = characterSize + 5;
            level02Dialogue[1].Origin = new Vector2f(level02Dialogue[1].GetGlobalBounds().Width / 2, level02Dialogue[1].GetGlobalBounds().Height / 2);
            level02Dialogue[1].Position = new Vector2f(0, 0);

        // Set Dialogue for Level03
        level03Dialogue = new();
        LoadDialogueList(level03Dialogue, "cutscenes/level03.txt");

        // Set Dialogue for Level04
        level04Dialogue = new();
        LoadDialogueList(level04Dialogue, "cutscenes/level04.txt");

        // Set Dialogue for Level05
        level05Dialogue = new();
        LoadDialogueList(level05Dialogue, "cutscenes/level05.txt");

        // Set Dialogue for Credits
        creditsDialogue = new();
        LoadDialogueList(creditsDialogue, "cutscenes/credits.txt");

            // Modifying Credit Dialogue
            creditsDialogue[1].CharacterSize = characterSize + 5;
            creditsDialogue[1].Origin = new Vector2f(creditsDialogue[1].GetGlobalBounds().Width / 2, creditsDialogue[1].GetGlobalBounds().Height / 2);
            creditsDialogue[1].Position = new Vector2f(0, 0);

            creditsDialogue[46].CharacterSize = characterSize + 10;
            creditsDialogue[46].Origin = new Vector2f(creditsDialogue[46].GetGlobalBounds().Width / 2, creditsDialogue[46].GetGlobalBounds().Height / 2);
            creditsDialogue[46].Position = new Vector2f(0, 0);

            creditsDialogue[77].CharacterSize = characterSize + 10;
            creditsDialogue[77].Origin = new Vector2f(creditsDialogue[77].GetGlobalBounds().Width / 2, creditsDialogue[77].GetGlobalBounds().Height / 2);
            creditsDialogue[77].Position = new Vector2f(0, 0);

            creditsDialogue[78].CharacterSize = characterSize + 10;
            creditsDialogue[78].Origin = new Vector2f(creditsDialogue[78].GetGlobalBounds().Width / 2, creditsDialogue[78].GetGlobalBounds().Height / 2);
            creditsDialogue[78].Position = new Vector2f(0, 0);

        // Initialize Info-Text
        infoText1 = new Text("E / Enter to continue", font, characterSize);
        infoText1.Origin = new Vector2f(infoText1.GetGlobalBounds().Width / 2, infoText1.GetGlobalBounds().Height / 2);
        infoText1.Position = new Vector2f(0, windowSizeY / 2 - 100);

        infoText2 = new Text("Space to skip", font, characterSize);
        infoText2.Origin = new Vector2f(infoText2.GetGlobalBounds().Width / 2, infoText2.GetGlobalBounds().Height / 2);
        infoText2.Position = new Vector2f(0, infoText1.Position.Y + characterSize);
    }
    public void Update(float deltaTime)
    {
        Inputs();
        InfoTextFade();
        CutsceneEnd();
    }
    public void Draw(RenderWindow window)
    {
        window.Draw(backGroundRect);
        if (currentDialogue[currentIndex].FillColor == maroColor)
        {
            window.Draw(_dialogueSprites[0]);
            DrawObjects.MenuSelectorOutline(_dialogueSprites[0].Position, _dialogueSprites[0].TextureRect.Width / 2, _dialogueSprites[0].TextureRect.Height / 2, 3, maroColor, window);
        }
        else if (currentDialogue[currentIndex].FillColor == veniaColor)
        {
            window.Draw(_dialogueSprites[1]);
            DrawObjects.MenuSelectorOutline(_dialogueSprites[1].Position, _dialogueSprites[1].TextureRect.Width / 2, _dialogueSprites[1].TextureRect.Height / 2, 3, veniaColor, window);
        }
        else if (currentDialogue[currentIndex].FillColor == volvoColor)
        {
            window.Draw(_dialogueSprites[2]);
            DrawObjects.MenuSelectorOutline(_dialogueSprites[2].Position, _dialogueSprites[2].TextureRect.Width / 2, _dialogueSprites[2].TextureRect.Height / 2, 3, volvoColor, window);
        }
        
        window.Draw(currentDialogue[currentIndex]);
        window.Draw(infoText1);
        window.Draw(infoText2);
    }
    public void LoadGamestate(GameState gameState)
    {
        _gameState = gameState;
        switch (gameState)
        {
            case GameState.Level01:
                currentDialogue = level01Dialogue;
                break;
            case GameState.Level02:
                currentDialogue = level02Dialogue;
                break;
            case GameState.Level03:
                currentDialogue = level03Dialogue;
                break;
            case GameState.Level04:
                currentDialogue = level04Dialogue;
                break;
            case GameState.Level05:
                currentDialogue = level05Dialogue;
                break;
            case GameState.Credits:
                creditsOn = true;
                currentDialogue = creditsDialogue;
                break;
            default:
                currentDialogue = level01Dialogue;
                break;
        }
    }
    private void Inputs()
    {
        if (InputManager.Instance.GetKeyDown(Keyboard.Key.E) && !endCutscene)
        {
            if (currentIndex < currentDialogue.Count - 1)
            {
                if (sfxInterval == 0)
                {
                    nextText.Play();
                    sfxInterval = 5;
                }
                sfxInterval--;
                currentIndex++;
            }
        }
        else if (InputManager.Instance.GetKeyDown(Keyboard.Key.Enter) && !endCutscene)
        {
            if (currentIndex < currentDialogue.Count - 1)
            {
                if (sfxInterval == 0)
                {
                    nextText.Play();
                    sfxInterval = 5;
                }
                sfxInterval--;
                currentIndex++;
            }
        }
        else if (InputManager.Instance.GetKeyDown(Keyboard.Key.Space))
        {
            nextText.Play();
            currentIndex = currentDialogue.Count - 1;
        }
        InputManager.Instance.Update();
    }
    private void CutsceneEnd()
    {
        if (currentIndex == currentDialogue.Count - 1)
        {
            endCutscene = true;
            currentIndex = 0;
            sfxInterval = 0;
        }
    }
    private Color GetTextColor(string[] splitLine)
    {
        if (splitLine[0] == "maro")
        {
            return maroColor;
        }
        else if (splitLine[0] == "venia")
        {
            return veniaColor;
        }
        else if (splitLine[0] == "volvo")
        {
            return volvoColor;
        }
        else if (splitLine[0] == "cyan")
        {
            return Color.Cyan;
        }
        else if (splitLine[0] == "red")
        {
            return Color.Red;
        }
        else if (splitLine[0] == "green")
        {
            return Color.Green;
        }
        else
        {
            return Color.White;
        }
    }
    private void LoadDialogueList(List<Text> dialogueList, string levelTextPath)
    {
        using(StreamReader sr = new StreamReader(levelTextPath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitLine = line.Split('\\');
                Text text = new Text(splitLine[1], font, characterSize);
                text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);
                text.Position = new Vector2f(0, 10);
                text.FillColor = GetTextColor(splitLine);
                dialogueList.Add(text);
            }
        }
    }
    private void InfoTextFade()
    {
        infoText1.FillColor = infoColor;
        infoText2.FillColor = infoColor;

        if (infoColor.A > 0 && fadeColor == true && fadeCounter % fadeInterval == 0)
        {
            infoColor.A -= 1;
        }
        else if (infoColor.A <= 255 && fadeColor == false  && fadeCounter % fadeInterval == 0)
        {
            infoColor.A += 1;
        }
        else if (infoColor.A == 0 && fadeColor == true)
        {
            fadeColor = false;
            fadeCounter = 0;
        }
        else if (infoColor.A == 255 && fadeColor == false)
        {
            fadeColor = true;
            fadeCounter = 0;
        }
        fadeCounter++;
    }
    public bool GetEndCutscene()
    {
        return endCutscene;
    }
    public void SetEndCutscene(bool value)
    {
        endCutscene = value;
    }
    public bool GetCreditsOn()
    {
        return creditsOn;
    }
    public void SetCreditsOn(bool value)
    {
        creditsOn = value;
    }
}