using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame.GUI
{
    class AlternativesButton : TextButton
    {
        private int selected = 0;
        private List<String> alternatives = new List<String>();

        public AlternativesButton(String defaultString, OpenTK.Vector2 position)
            : base(defaultString, null, position)
        {
        }

        public void AddAlternative(String s)
        {
            alternatives.Add(s);
        }

        public override void Click()
        {
            selected = (selected + 1) % alternatives.Count;
            text = alternatives[selected];
        }

        public int getSelected()
        {
            return selected;
        }
    }
}
