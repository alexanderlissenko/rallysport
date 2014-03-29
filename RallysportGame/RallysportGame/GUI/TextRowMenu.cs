﻿using OpenTK.Graphics;
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
        private int selected = 0;
        private const String fontFile = @"..\\..\\Fonts\\Calibri.ttf";
        private QFont font;
        private QFont highlightFont;
        private int maxWidth;
        private OpenTK.Vector2 position;
        private String menuText = ""; //the menu in text form

        public TextRowMenu(int posX, int posY, int fontSize, int maxWidth, float lineSpaceHeight)
        {
            buttonList = new List<TextButton>();
            font = new QFont(fontFile, fontSize);
            highlightFont = new QFont(fontFile, fontSize, new QFontBuilderConfiguration(true));
            font.Options.LineSpacing = lineSpaceHeight;
            highlightFont.Options.LineSpacing = lineSpaceHeight;

            highlightFont.Options.Colour = new Color4(133, 232, 90, 200); //highlight colour

            this.maxWidth = maxWidth;
            position = new OpenTK.Vector2(posX, posY);
        }
        
        public void AddTextButton(String text, Action f)
        {
            buttonList.Add(new TextButton(text, f));

            StringBuilder builder = new StringBuilder();
            foreach (var button in buttonList)
            {
                builder.AppendLine(button.getText());
            }
            menuText = builder.ToString();
        }

        public void ClickSelected()
        {
            buttonList[selected].click();
        }

        public void Select(int index)
        {
            if (index > buttonList.Count || index < 0)
            {
                throw new ArgumentException("bad argument passed to TextRowMenu");
            }
            selected = index;
        }

        public void SelectUp()
        {
            selected = (selected - 1) % buttonList.Count;
        }

        public void SelectDown()
        {
            selected = (selected + 1) % buttonList.Count;
        }

        public void Render()
        {
            QFont.Begin();
            font.Print(menuText, maxWidth, QFontAlignment.Centre, position);
            
            QFont.End();

        }
    }
}
