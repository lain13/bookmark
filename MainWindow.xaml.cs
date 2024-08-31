using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media;
using ZXing;
using ZXing.QrCode;
using System.IO;
using System.Text.Json;

namespace Bookmark
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<BookmarkItem> Bookmarks { get; set; }
        private Point startPoint;
        private const string BookmarkFilePath = "bookmarks.json";

        public MainWindow()
        {
            InitializeComponent();
            Bookmarks = new ObservableCollection<BookmarkItem>();
            DataContext = this;

            LoadBookmarks();
        }

        private void LoadBookmarks()
        {
            if (File.Exists(BookmarkFilePath))
            {
                string json = File.ReadAllText(BookmarkFilePath);
                var bookmarks = JsonSerializer.Deserialize<List<BookmarkItem>>(json);
                if (bookmarks != null)
                {
                    foreach (var bookmark in bookmarks)
                    {
                        bookmark.QrCodeImage = GenerateQrCode(bookmark.Url);
                        Bookmarks.Add(bookmark);
                    }
                }
            }
        }

        private void SaveBookmarks()
        {
            var bookmarksToSave = Bookmarks.Select(b => new { b.Url, b.Title }).ToList();
            string json = JsonSerializer.Serialize(bookmarksToSave, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(BookmarkFilePath, json);
        }

        private void AddBookmark_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBookmarkDialog();
            if (dialog.ShowDialog() == true)
            {
                string url = dialog.BookmarkUrl;
                string title = dialog.BookmarkTitle;

                var qrCodeImage = GenerateQrCode(url);
                if (qrCodeImage != null)
                {
                    Bookmarks.Add(new BookmarkItem { Url = url, Title = title, QrCodeImage = qrCodeImage });
                    SaveBookmarks();
                }
                else
                {
                    MessageBox.Show("QR 코드 생성에 실패했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditBookmark_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var bookmarkItem = (BookmarkItem)button.DataContext;

            var dialog = new AddBookmarkDialog
            {
                Title = "북마크 수정",
                BookmarkTitle = bookmarkItem.Title,
                BookmarkUrl = bookmarkItem.Url
            };

            if (dialog.ShowDialog() == true)
            {
                bookmarkItem.Title = dialog.BookmarkTitle;
                bookmarkItem.Url = dialog.BookmarkUrl;
                bookmarkItem.QrCodeImage = GenerateQrCode(dialog.BookmarkUrl);

                bookmarkItem.OnPropertyChanged(nameof(BookmarkItem.Title));
                bookmarkItem.OnPropertyChanged(nameof(BookmarkItem.Url));
                bookmarkItem.OnPropertyChanged(nameof(BookmarkItem.QrCodeImage));

                SaveBookmarks();
            }
        }

        private void DeleteBookmark_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var bookmarkItem = (BookmarkItem)button.DataContext;

            var result = MessageBox.Show($"'{bookmarkItem.Title}' 북마크를 삭제하시겠습니까?", "북마크 삭제", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Bookmarks.Remove(bookmarkItem);
                SaveBookmarks();
            }
        }

        private void ToggleFocusMode_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var bookmarkItem = (BookmarkItem)button.DataContext;

            bookmarkItem.IsFocused = !bookmarkItem.IsFocused;

            foreach (var item in Bookmarks)
            {
                if (item != bookmarkItem)
                {
                    item.Opacity = bookmarkItem.IsFocused ? 0.3 : 1.0;
                }
                else
                {
                    item.Opacity = 1.0;
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"URL을 열 수 없습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage GenerateQrCode(string url)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 0
                }
            };

            var pixelData = writer.Write(url);
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    ListBoxItem listBoxItem = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                    if (listBoxItem != null)
                    {
                        BookmarkItem bookmarkItem = (BookmarkItem)listBoxItem.DataContext;
                        DragDrop.DoDragDrop(listBoxItem, bookmarkItem, DragDropEffects.Move);
                    }
                }
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(BookmarkItem)))
            {
                BookmarkItem droppedData = e.Data.GetData(typeof(BookmarkItem)) as BookmarkItem;
                BookmarkItem target = ((FrameworkElement)e.OriginalSource).DataContext as BookmarkItem;

                int removedIdx = Bookmarks.IndexOf(droppedData);
                int targetIdx = Bookmarks.IndexOf(target);

                if (removedIdx < targetIdx)
                {
                    Bookmarks.Insert(targetIdx + 1, droppedData);
                    Bookmarks.RemoveAt(removedIdx);
                }
                else
                {
                    int remIdx = removedIdx + 1;
                    if (Bookmarks.Count + 1 > remIdx)
                    {
                        Bookmarks.Insert(targetIdx, droppedData);
                        Bookmarks.RemoveAt(remIdx);
                    }
                }

                SaveBookmarks();
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }

    public class BookmarkItem : INotifyPropertyChanged
    {
        public string Url { get; set; }
        public string Title { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public BitmapImage QrCodeImage { get; set; }

        private bool _isFocused;
        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused != value)
                {
                    _isFocused = value;
                    OnPropertyChanged(nameof(IsFocused));
                }
            }
        }

        private double _opacity = 1.0;
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    OnPropertyChanged(nameof(Opacity));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}