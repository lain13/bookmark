using System.Windows;

namespace Bookmark
{
    public partial class AddBookmarkDialog : Window
    {
        public string BookmarkTitle { get; set; }
        public string BookmarkUrl { get; set; }

        public AddBookmarkDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || string.IsNullOrWhiteSpace(UrlTextBox.Text))
            {
                MessageBox.Show("제목과 URL을 모두 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BookmarkTitle = TitleTextBox.Text;
            BookmarkUrl = UrlTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
