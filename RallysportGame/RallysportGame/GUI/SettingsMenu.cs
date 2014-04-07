using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame.GUI
{
    class SettingsMenu
    {
        private const int MAX_WIDTH = 500; //determines the max allowed width for the textRowMenu
        private const int TEXT_SIZE = 80;
        private const float LINE_SPACE = 1.3f;
        private const int VERTICAL_OFFSET = 0;

        //determines what resolution options are available
        private int[][] RESOLUTIONS = { new int[] { 1600, 800 }, new int[] { 800, 600 }, new int[] { 800, 200 } };

        private TextRowMenu textMenu;
        private GameWindow gameWindow;
        private AlternativesButton soundButton;
        private AlternativesButton resolutionButton;
        private List<String> resolutionList;

        public SettingsMenu(GameWindow gameWindow, Action returnToMainMenu)
        {
            textMenu = new TextRowMenu((SettingsParser.GetInt(Settings.WINDOW_WIDTH) / 11), VERTICAL_OFFSET, TEXT_SIZE, MAX_WIDTH * 2, LINE_SPACE, gameWindow.Mouse);
            this.gameWindow = gameWindow;


            resolutionList = new List<String>();

            String s;
            for(int i = 0; i < RESOLUTIONS.Length ; i++){

                resolutionList.Add("Resolution: " + RESOLUTIONS[i][0] + "x" + RESOLUTIONS[i][1]);
                    
            }
            resolutionButton = textMenu.AddAlternativesButton(resolutionList);

            List<String> enableSoundList = new List<String>();
            enableSoundList.Add("Sound: on");
            enableSoundList.Add("Sound: off");
            soundButton = textMenu.AddAlternativesButton(enableSoundList);

            Action SaveAndReturn = delegate {
                saveChanges();
                returnToMainMenu();
            };

            textMenu.AddTextButton("Done", SaveAndReturn);
        }

        private void saveChanges() {

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
            return RESOLUTIONS[resolutionButton.getSelected()];
        }
    }
}
