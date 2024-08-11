namespace UI_Mimic.Linux
{
    class Example
    {
        #define INPUT_QUEUE "/dev/input/event0"
        #define EVENT_LEN 16
        //https://cplusplus.com/forum/unices/8206/
        void readEventLine(FILE* in, char* data) { //read input key stream
            int i;
            for (i = 0; i <= 15; i++) { //each key press will trigger 16 characters of data, describing the event
                data[i] = (char)fgetc(in);
            }
        }

        int main() {
            FILE* input;
            char data[EVENT_LEN];

            input = fopen(INPUT_QUEUE, "r+");
            readEventLine(input, data);
        }
    }
}
