using Microsoft.SPOT;


namespace HERO_Code {
    class Utils {
        public static void Delay(int delay) {
            //Delay in milliseconds
            System.Threading.Thread.Sleep(delay);
        }

        public static void Print(string output) {
            Debug.Print(output);
        }

        public static void Print(int output) {
            Debug.Print(output.ToString());
        }

        public static void Print(double output) {
            Debug.Print(output.ToString());
        }

        public static void Print(bool output) {
            Debug.Print(output.ToString());
        }
    }
}

