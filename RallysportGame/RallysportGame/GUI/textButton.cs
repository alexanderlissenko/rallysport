using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame.GUI
{
    class TextButton
    {
        private Action f;
        private String text;
        private Vector2 position;

        public TextButton(String text, Action f,Vector2 position)
        {
            this.f = f;
            this.text = text;
            this.position = position;

        }

        public String getText()
        {
            return text;
        }
        public void click()
        {
            f();
        }

        public Vector2 getPosition()
        {
            return position;
        }

    }
}
