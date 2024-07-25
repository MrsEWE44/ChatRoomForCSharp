
namespace WindowsFormsApp2
{
    class TCPServerWindow : TCPWindow
    {
        public static int PORT = 45465;
        public static string IP = "127.0.0.1";
        public TCPServerWindow()
        {
            this.Mode = 1;
            init();
        }
    }
}
