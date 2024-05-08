namespace UI_Mimic.Linux {
    internal class UIReader : InputReader {
        public UIReader(bool Global, string[] LoggingWindows) :
            base(Global, LoggingWindows) {

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
