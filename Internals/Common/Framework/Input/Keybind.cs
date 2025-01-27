﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using WiiPlayTanksRemake.Internals.Common;
using WiiPlayTanksRemake.Internals.Common.Utilities;

namespace WiiPlayTanksRemake.Internals.Common.Framework.Input
{
    public class Keybind
    {
        public static List<Keybind> AllKeybinds { get; internal set; } = new();

        public Keybind(string name, Keys defaultKey = Keys.None) {
            Name = name;
            AssignedKey = defaultKey;
            AllKeybinds.Add(this);
        }

        public bool JustReassigned { get; private set; }

        public bool JustPressed => Common.Input.KeyJustPressed(AssignedKey) && !PendKeyReassign;
        public bool IsPressed => Common.Input.CurrentKeySnapshot.IsKeyDown(AssignedKey) && !PendKeyReassign;
        public bool PendKeyReassign { get; set; } = false;

        public Action<Keys> OnKeyReassigned;

        public bool onalssign;

        public Keys AssignedKey { get; internal set; } = Keys.None;
        public string Name { get; set; } = "Not Named";

        /// <summary>
        /// This is an extra tool for people who use this. Used for displaying messages after a key is recently rebound.
        /// </summary>
        public bool RecentlyBound => _rebindAlertTime > 0;
        private int _rebindAlertTime;

        public Action<Keybind> KeybindPressAction { get; set; } = null;

        private void PollReassign()
        {
            if (Common.Input.CurrentKeySnapshot.GetPressedKeys().Length > 0)
            {
                var firstKey = Common.Input.CurrentKeySnapshot.GetPressedKeys()[0];
                if (Common.Input.KeyJustPressed(firstKey) && firstKey == AssignedKey)
                {
                    OnKeyReassigned?.Invoke(AssignedKey);
                    PendKeyReassign = false;
                    return;
                }
                else if (Common.Input.KeyJustPressed(firstKey) && firstKey == Keys.Escape)
                {
                    AssignedKey = Keys.None;
                    OnKeyReassigned?.Invoke(AssignedKey);
                    PendKeyReassign = false;
                    return;
                }
                AssignedKey = firstKey;
                OnKeyReassigned?.Invoke(AssignedKey);
                PendKeyReassign = false;
                return;
            }
        }

        internal void Update()
        {
            if (PendKeyReassign)
                PollReassign();

            if (_rebindAlertTime > 0)
                _rebindAlertTime--;

            JustReassigned = false;

            if (IsPressed)
                KeybindPressAction?.Invoke(this);
        }

        public override string ToString() {
            return Name + " = {" + $"Key: {AssignedKey.ParseKey()} | Pressed: {IsPressed} | ReassignPending: {PendKeyReassign} " + "}";
        }
    }
}