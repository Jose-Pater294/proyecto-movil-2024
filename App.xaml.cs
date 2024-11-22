namespace PlayField
{
    public partial class App : Application
    {
        public static int UserID { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginView());
        }
    }
}
