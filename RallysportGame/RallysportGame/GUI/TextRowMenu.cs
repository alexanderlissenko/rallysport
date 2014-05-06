using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private OpenTK.Vector2 previousPosition; // position of the last added Text
        private float lineSpaceHeight;
        private List<Rectangle> hitBoxList;
        private const QFontAlignment ALIGNMENT= QFontAlignment.Left; //due to the incompletedness of Quickfont. only alignment.Left is working..
        private MouseDevice mouse;
        private Point previousMousePoint = new Point(0, 0);

        public TextRowMenu(int posX, int posY, int fontSize, int maxWidth, float lineSpace, MouseDevice mouse)
        {
            buttonList = new List<TextButton>();
            hitBoxList = new List<Rectangle>();
            font = new QFont(fontFile, fontSize);
            highlightFont = new QFont(fontFile, fontSize, new QFontBuilderConfiguration(true));

            highlightFont.Options.Colour = new Color4(133, 232, 90, 200); //highlight colour
            this.lineSpaceHeight = fontSize * lineSpace;
            this.maxWidth = maxWidth;
            this.mouse = mouse;

            previousPosition = new OpenTK.Vector2(posX, posY);
        }
        
        public void AddTextButton(String text, Action f)
        {
            OpenTK.Vector2 newPosition = new OpenTK.Vector2(previousPosition.X, previousPosition.Y + lineSpaceHeight);
            previousPosition = newPosition;
            TextButton newTextButton = new TextButton(text, f, newPosition);
            buttonList.Add(newTextButton);
            newHitBox(newTextButton);
        }

        public AlternativesButton AddAlternativesButton(List<String> alternatives)
        {
            OpenTK.Vector2 newPosition = new OpenTK.Vector2(previousPosition.X, previousPosition.Y + lineSpaceHeight);
            previousPosition = newPosition;
            AlternativesButton newButton = new AlternativesButton(alternatives[0],newPosition);

            foreach (String s in alternatives)
            {
                newButton.AddAlternative(s);
            }

            buttonList.Add(newButton);
            newHitBox(newButton); // causes a collision bug since text can change in a alternatives button
            return newButton;
        }

        private void newHitBox(TextButton buttonText)
        {
            SizeF size = font.Measure(buttonText.getText(), maxWidth, ALIGNMENT);
            
            int x = (int)buttonText.getPosition().X;
            int y = (int)buttonText.getPosition().Y;
            int width = (int)size.Width;
            int height = (int)size.Height;
            hitBoxList.Add(new Rectangle(x,y, width, height));
        }

        public void clearAllHitboxes()
        {
            hitBoxList.Clear();
        }

        public void ClickSelected()
        {
            buttonList[selected].Click();
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

            int select = (selected - 1);
            if (select < 0)
            {
                select = buttonList.Count-1;
            }

            selected = select % buttonList.Count;

        }

        public void SelectDown()
        {
            selected = (selected + 1) % buttonList.Count;
        }

        public void Render()
        {
            QFont.Begin();
            for(int i = 0; i < buttonList.Count ; i++) {
                var buttonText = buttonList[i];
                if (i == selected)
                {
                    highlightFont.Print(buttonText.getText(), maxWidth, ALIGNMENT, buttonText.getPosition());
                }
                else
                {
                    font.Print(buttonText.getText(), maxWidth, ALIGNMENT, buttonText.getPosition());
                    //float yOffset = 0;
                    //PrintWithBounds(font, buttonText.getText(), hitBoxList[i], ALIGNMENT, yOffset);
                }
            }
            QFont.End();


        }
        private void UpdateMouse() {

            Point point = new Point(mouse.X, mouse.Y);
            //System.Console.WriteLine(point.ToString());
            if (!point.Equals(previousMousePoint))
            {
                for (int i = 0; i < hitBoxList.Count; i++)
                {
                    Rectangle rect = hitBoxList[i];
                    if (rect.Contains(point))
                    {
                        Select(i);
                    }
                }
                previousMousePoint = point;
            }
        }
        public void Update()
        {
            UpdateMouse();
        }

        //for debugging
        private void PrintWithBounds(QFont font, string text, Rectangle bounds, QFontAlignment alignment, float yOffset)
        {

            GL.Disable(EnableCap.Texture2D);
            GL.Color4(1.0f, 0f, 0f, 1.0f);


            float maxWidth = bounds.Width;
            float height = bounds.Height;
            //float height = font.Measure(text, maxWidth, alignment).Height;

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex3(bounds.X, bounds.Y, 0f);
            GL.Vertex3(bounds.X + bounds.Width, bounds.Y, 0f);
            GL.Vertex3(bounds.X + bounds.Width, bounds.Y + height, 0f);
            GL.Vertex3(bounds.X, bounds.Y + height, 0f);
            GL.End();

            font.Print(text, maxWidth, alignment, new Vector2(bounds.X, bounds.Y));

            yOffset += height;

        }
    }

}
