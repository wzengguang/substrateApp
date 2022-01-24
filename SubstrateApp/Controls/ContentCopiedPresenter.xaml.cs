using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SubstrateApp.Helper;
using ColorCode;
using ColorCode.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SubstrateApp.Controls
{
    public sealed partial class ContentCopiedPresenter : UserControl
    {
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Text", typeof(string), typeof(ContentCopiedPresenter),
            new PropertyMetadata("", OnDependencyPropertyChanged));
        public string Text
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public ContentCopiedPresenter()
        {
            this.InitializeComponent();
        }

        private static void OnDependencyPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (target is ContentCopiedPresenter presenter)
            {
                presenter.ReevaluateVisibility();
            }
        }

        private void ReevaluateVisibility()
        {
            if (Text.Length == 0)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;

                FormatAndRenderSampleFromString(Text, CodePresenter, Languages.Xml);
            }
        }

        private void ContentCopiedPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            ReevaluateVisibility();

        }

        private RichTextBlockFormatter GenerateRichTextFormatter()
        {
            var formatter = new RichTextBlockFormatter(ThemeHelper.ActualTheme);

            if (ThemeHelper.ActualTheme == ElementTheme.Dark)
            {
                UpdateFormatterDarkThemeColors(formatter);
            }

            return formatter;
        }

        private void UpdateFormatterDarkThemeColors(RichTextBlockFormatter formatter)
        {
            // Replace the default dark theme resources with ones that more closely align to VS Code dark theme.
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttribute]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeQuotes]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeValue]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.HtmlComment]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlDelimiter]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlName]);

            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttribute)
            {
                Foreground = "#FF87CEFA",
                ReferenceName = "xmlAttribute"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeQuotes)
            {
                Foreground = "#FFFFA07A",
                ReferenceName = "xmlAttributeQuotes"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeValue)
            {
                Foreground = "#FFFFA07A",
                ReferenceName = "xmlAttributeValue"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.HtmlComment)
            {
                Foreground = "#FF6B8E23",
                ReferenceName = "htmlComment"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlDelimiter)
            {
                Foreground = "#FF808080",
                ReferenceName = "xmlDelimiter"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlName)
            {
                Foreground = "#FF5F82E8",
                ReferenceName = "xmlName"
            });
        }
        private void ContentCopiedPresenter_ActualThemeChanged(FrameworkElement sender, object args)
        {
            // If the theme has changed after the user has already opened the app (ie. via settings), then the new locally set theme will overwrite the colors that are set during Loaded.
            // Therefore we need to re-format the REB to use the correct colors.
            GenerateSyntaxHighlightedContent();
        }

        private void GenerateSyntaxHighlightedContent()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                FormatAndRenderSampleFromString(Text, CodePresenter, Languages.Xml);
            }
        }

        private void FormatAndRenderSampleFromString(string sampleString, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            var sampleCodeRTB = new RichTextBlock { FontFamily = new FontFamily("Consolas") };

            var formatter = GenerateRichTextFormatter();
            formatter.FormatRichTextBlock(sampleString, highlightLanguage, sampleCodeRTB);
            presenter.Content = sampleCodeRTB;
        }

        private void CopyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText(Text);
            Clipboard.SetContent(package);

            VisualStateManager.GoToState(this, "ConfirmationDialogVisible", false);

            // Automatically close teachingtip after 1 seconds
            if (DispatcherQueue.GetForCurrentThread() != null)
            {
                DispatcherQueue.GetForCurrentThread().TryEnqueue(async () =>
                {
                    await Task.Delay(1000);
                    VisualStateManager.GoToState(this, "ConfirmationDialogHidden", false);
                });
            }
        }
    }
}
