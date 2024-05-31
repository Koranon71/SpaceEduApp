using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpaceEduApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class User
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public DateTime RegistrationDate { get; set; }

            public User(string login, string password, string email)
            {
                Login = login;
                Password = password;
                Email = email;
                RegistrationDate = DateTime.Now;
            }

            public override string ToString()
            {
                return $"{Login},{Password},{Email},{RegistrationDate}";
            }
        }

        public static class UserDatabase
        {
            private const string FilePath = @"C:\Users\zerro\source\repos\SpaceEduApp\SpaceEduApp\users.txt";

            public static void SaveUser(User user)
            {
                File.AppendAllText(FilePath, user.ToString() + Environment.NewLine);
            }

            public static List<User> LoadUsers()
            {
                List<User> users = new List<User>();
                if (File.Exists(FilePath))
                {
                    string[] lines = File.ReadAllLines(FilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        string login = parts[0];
                        string password = parts[1];
                        string email = parts[2];
                        DateTime registrationDate = DateTime.Parse(parts[3]);
                        users.Add(new User(login, password, email) { RegistrationDate = registrationDate });
                    }
                }
                return users;
            }

            public static bool UserExists(string login)
            {
                var users = LoadUsers();
                return users.Any(user => user.Login == login);
            }
        }

        private void RegisterSliderButton_Click(object sender, RoutedEventArgs e)
        {
            SlideToRegister();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SlideToLogin();
        }

        private void SlideToRegister()
        {
            var storyboard = new Storyboard();
            var slideOut = new ThicknessAnimation
            {
                From = new Thickness(0, 0, 0, 0),
                To = new Thickness(-Width, 0, Width, 0),
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(slideOut, LoginScreen);
            Storyboard.SetTargetProperty(slideOut, new PropertyPath(MarginProperty));
            var slideIn = new ThicknessAnimation
            {
                From = new Thickness(Width, 0, -Width, 0),
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(slideIn, RegisterScreen);
            Storyboard.SetTargetProperty(slideIn, new PropertyPath(MarginProperty));
            storyboard.Children.Add(slideOut);
            storyboard.Children.Add(slideIn);
            LoginScreen.Visibility = Visibility.Visible;
            RegisterScreen.Visibility = Visibility.Visible;
            storyboard.Completed += (s, e) =>
            {
                LoginScreen.Visibility = Visibility.Collapsed;
            };
            storyboard.Begin();
        }
        private void SlideToLogin()
        {
            var storyboard = new Storyboard();
            var slideOut = new ThicknessAnimation
            {
                From = new Thickness(0, 0, 0, 0),
                To = new Thickness(Width, 0, -Width, 0),
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(slideOut, RegisterScreen);
            Storyboard.SetTargetProperty(slideOut, new PropertyPath(MarginProperty));

            var slideIn = new ThicknessAnimation
            {
                From = new Thickness(-Width, 0, Width, 0),
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(slideIn, LoginScreen);
            Storyboard.SetTargetProperty(slideIn, new PropertyPath(MarginProperty));
            storyboard.Children.Add(slideOut);
            storyboard.Children.Add(slideIn);

            LoginScreen.Visibility = Visibility.Visible;
            RegisterScreen.Visibility = Visibility.Visible;
            storyboard.Completed += (s, e) =>
            {
                RegisterScreen.Visibility = Visibility.Collapsed;
            };
            storyboard.Begin();
        }
        private bool ValidateEmail(string email)
        {
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            return Regex.IsMatch(email, pattern);
        }
        private bool ValidatePassword(string password)
        {
            if (password.Length < 8)
                return false;
            if (!password.Any(char.IsDigit))
                return false;
            if (!password.Any(char.IsUpper))
                return false;
            if (!password.Any(char.IsLower))
                return false;
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return false;
            return true;
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegisterUsernameTextBox.Text;
            string password = RegisterPasswordTextBox.Text;
            string email = RegisterEmailTextBox.Text;
            if (UserDatabase.UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return;
            }
            if (!ValidateEmail(email))
            {
                MessageBox.Show("Некорректный формат электронной почты");
                return;
            }
            if (!ValidatePassword(password))
            {
                MessageBox.Show("Ненадежный пароль");
                return;
            }
            User newUser = new User(login, password, email);
            UserDatabase.SaveUser(newUser);
            RegisterUsernameTextBox.Text = null;
            RegisterPasswordTextBox.Text = null;
            RegisterEmailTextBox.Text = null;

            SlideToLogin();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Text;

            if (CheckUserCredentials(login, password))
            {
                ShowMainAppScreen();
            }
            else
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CheckUserCredentials(string login, string password)
        {
            var users = UserDatabase.LoadUsers();
            foreach (var user in users)
            {
                if (user.Login == login && user.Password == password)
                {
                    return true;
                }
            }
            return false;
        }
        private void ShowMainAppScreen()
        {
            SApp mainAppScreen = new SApp();
            mainAppScreen.Show();
            this.Close();
        }
    }
}