using System;

namespace Nagih
{
    public static class Enum
    {
        #region ENUM
        public enum AudioSource
        {
            OneShot = 0,
            SFX = 1,
            BGM = 2
        }

        public enum Sound
        {
            MainBGM = 0,
            ClickButton = 1,
            CloseButton = 2
        }

        public enum Scene
        {
            Splash = 0,
            Game = 1
        }

        public enum Font
        {
            Calibri = 0,
            Arial = 1
        }

        public enum Icon
        {
            NagihCoin
        }

        public enum AnalyticsEvent
        {
            
        }

        public enum TutorialTrigger
        {

        }

        public enum TutorialStatus
        {
            NotYet = 0,
            Done = 1
        }

        public enum PlayerPref
        {
            User,
            Device
        }

        public enum SelectDirection
        {
            Left = 1,
            Right = 2,
            Up = 3,
            Down = 4
        }
        #endregion

        #region METHOD
        #endregion
    }
}