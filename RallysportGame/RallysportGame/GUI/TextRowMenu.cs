using QuickFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame.GUI
{
    class TextRowMenu
    {
        private List<TextButton> buttonList;
        private int selected;
        //private int textInterval;
        private const String fontFile = @"..\\..\\Fonts\\Calibri.ttf";
        private QFont font;
        private int maxWidth;
        private OpenTK.Vector2 position;

        public TextRowMenu(int posX, int posY, int textInterval, int fontSize, int maxWidth)
        {
            buttonList = new List<TextButton>();
            font = new QFont(fontFile, fontSize);
            this.maxWidth = maxWidth;
            position = new OpenTK.Vector2(posX, posY);
        }
        
        public void AddTextButton(String text, Action f)
        {
            buttonList.Add(new TextButton(text, f));
        }
        public void ClickSelected()
        {

        }
        public void Select(int index)
        {

        }
        public void SelectUp()
        {

        }
        public void SelectDown()
        {

        }
        public void Render()
        {

            StringBuilder builder = new StringBuilder();
            foreach (var button in buttonList)
            {
                builder.AppendLine(button.getText());
            }
            QFont.Begin();
            //font.Print(builder.ToString(), maxWidth, QFontAlignment.Right); //add position
            font.Print(builder.ToString(),maxWidth, QFontAlignment.Right, position);
            QFont.End();

        }
    }
}
