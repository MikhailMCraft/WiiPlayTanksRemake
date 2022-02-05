using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using WiiPlayTanksRemake.Internals.Common.GameInput;

namespace WiiPlayTanksRemake.Internals.Common
{
    public class TextInput
    {
        private TextInput() { }

        public static bool trackingInput;

        public static string InputtedText { get; private set; } = string.Empty;

        // starts the tracking of keys, only when input is not already being tracked
        public static void BeginInput() {
            trackingInput = true;
            TextInputEXT.TextInput += TextInputEXT_TextInput;
        }

        private static void TextInputEXT_TextInput(char obj)
        {
            Console.WriteLine(obj);
            /*bool isBack = obj.Key == Keys.Back;
            bool isSpace = e.Key == Keys.Space;
            bool ignoreable = e.Key == Keys.Enter;

            if (ignoreable)
                return;

            if (isSpace)
            {
                InputtedText += " ";
                return;
            }

            if (isBack && InputtedText.Length > 0)
            {
                InputtedText = InputtedText.Remove(InputtedText.Length - 1);
                return;
            }

            InputtedText += e.Character;*/
        }

        // stops tracking keys
        public static void EndInput() {
            trackingInput = false;
            InputtedText = string.Empty;
            TextInputEXT.TextInput -= TextInputEXT_TextInput;
        }
    }
}