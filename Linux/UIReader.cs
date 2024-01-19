namespace UI_Mimic.Linux {
    internal class UIReader : UIController {

        private event LocalKeyEventHandler KeyDown;
        private event LocalKeyEventHandler KeyUp;
        private event ErrorEventHandler OnError;
        private event LocalMouseMoveHandler OnMouseMove;
        private event LocalMouseEventHandler OnMouseClick;


        public UIReader(bool Global, string[] LoggingWindows, Windows.UIReader.HookTypePub Hook = Windows.UIReader.HookTypePub.Keyboard) :
            base(Global, LoggingWindows, Hook) {

        }


        //Converted from C++ Types to C# Types
        public struct input_event {
            public timeval time;
            public ushort type;
            public ushort code;
            public uint value;
        };
        public struct timeval {
            public uint time_t;
            public uint suseconds_t;

            public timeval(uint time_t) {
                this.time_t = time_t;
                suseconds_t = time_t/10;
            }
            public timeval(uint time_t,uint suseconds_t) {
                this.time_t = time_t;
                this.suseconds_t = suseconds_t;
            }
        }
    }

}
