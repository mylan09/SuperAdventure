using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public bool AddExtraNewLine { get; private set; }
        public Color TextColor { get; private set; }

        public MessageEventArgs(string message, bool addExtraNewLine, Color textColor)
        {
            Message = message;
            AddExtraNewLine = addExtraNewLine;
            TextColor = textColor;
        }
    }
}
