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

        public TextButton(String text, Action f)
        {
            this.f = f;
            this.text = text;
        }
        public String getText()
        {
            return text;
        }
        public void click()
        {
            f();
        }
    }
}
