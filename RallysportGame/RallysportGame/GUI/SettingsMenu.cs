using OpenTK;
using OpenTK.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame.GUI
{
    class SettingsMenu
    {
        private class intPairComparer : IEqualityComparer<int[]>
        {

            public bool Equals(int[] x, int[] y)
            {
                return ((x[0] == y[0]) && (x[1] == y[1]));
            }

            public int GetHashCode(int[] obj)
            {
                return obj[0] * 17 + obj[1] * 71;
            }
        }
        private const int MAX_WIDTH = 600; //determines the max allowed width for the textRowMenu
        private const int TEXT_SIZE = 70;
        private const float LINE_SPACE = 1.3f;
        private const int VERTICAL_OFFSET = 0;
        //determines what resolution options are available
        //private int[][] RESOLUTIONS;
        private List<int[]> RESOLUTIONS;
        private TextRowMenu textMenu;
        private GameWindow gameWindow;
        private AlternativesButton soundButton;
        private AlternativesButton resolutionButton;

        private List<String> resolutionList;

        //saved state from previous entry to settingsMenu
        private bool previousSound;
        private int[] previousResolution;

        public SettingsMenu(GameWindow gameWindow, Action returnToMainMenu)
        {
            RESOLUTIONS = new List<int[]>();
            //RESOLUTIONS = { new int[] { SettingsParser.GetInt(Settings.WINDOW_WIDTH), SettingsParser.GetInt(Settings.WINDOW_HEIGHT) }, new int[] { 800, 600 }, new int[] { 1920, 1080} };
            RESOLUTIONS.Add(new int[] { SettingsParser.GetInt(Settings.WINDOW_WIDTH), SettingsParser.GetInt(Settings.WINDOW_HEIGHT) });
            RESOLUTIONS.Add(new int[] { 800, 600 });
            RESOLUTIONS.Add(new int[] { 1920, 1080 });

            RESOLUTIONS = RESOLUTIONS.Distinct(new intPairComparer()).ToList<int[]>();

            textMenu = new TextRowMenu((SettingsParser.GetInt(Settings.WINDOW_WIDTH) / 11), VERTICAL_OFFSET, TEXT_SIZE, MAX_WIDTH * 2, LINE_SPACE, gameWindow.Mouse);
            this.gameWindow = gameWindow;


            resolutionList = new List<String>();

            //for(int i = 0; i < RESOLUTIONS.Count ; i++){
            //
            //    resolutionList.Add("Resolution: " + RESOLUTIONS.ToList[i][0] + "x" + RESOLUTIONS[i][1]);
            //}
            foreach (int[] resArray in RESOLUTIONS.Distinct<int[]>())
            {
                resolutionList.Add("Resolution: " + resArray[0] + "x" + resArray[1]);
            }
            resolutionButton = textMenu.AddAlternativesButton(resolutionList);

            List<String> enableSoundList = new List<String>();
            enableSoundList.Add("Sound: on");
            enableSoundList.Add("Sound: off");
            soundButton = textMenu.AddAlternativesButton(enableSoundList);



            Action SaveAndReturn = delegate {
                if (isStateChanged())
                {
                    saveChanges();
                    StateHandler.Instance.restartGame();
                }
                else
                {
                    returnToMainMenu();
                }
            };

            textMenu.AddTextButton("Done", SaveAndReturn);
        }

        private void saveChanges() {
            SettingsParser.setAudioEnabled(isSoundEnabled());
            SettingsParser.setResolution(getResolution());
        }

        public TextRowMenu toTextMenu() {
            return textMenu;
        }

        public bool isSoundEnabled()
        {
            return soundButton.getSelected() == 0;
        }

        public int[] getResolution()
        {
            return RESOLUTIONS.ToArray()[resolutionButton.getSelected()];
        }

        public void prepareEntryToSettings()
        {
            previousResolution = getResolution();
            previousSound = isSoundEnabled();
        }
        private bool isStateChanged() {
            return !(previousResolution.Equals(getResolution()) && (previousSound == isSoundEnabled()));
        }

        public void undoStateChanges()
        {
            while (getResolution() != previousResolution)
            {
                resolutionButton.Click();
            }
            while (isSoundEnabled() != previousSound)
            {
                soundButton.Click();
            }    

        }
    }
}
