using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Data.Xml.Xsl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace Megatokyo.Client.Helpers
{
    /// <summary>
    /// Usage: 
    /// 1) In a XAML file, declare the above namespace, e.g.:
    ///    xmlns:common="using:WinRT_RichTextBlock.Html2Xaml"
    ///     
    /// 2) In RichTextBlock controls, set or databind the Html property, e.g.:
    /// 
    ///    <RichTextBlock common:Properties.Html="{Binding ...}"/>
    ///    
    ///    or
    ///    
    ///    <RichTextBlock>
    ///       <common:Properties.Html>
    ///         <![CDATA[
    ///             <p>This is a list:</p>
    ///             <ul>
    ///                 <li>Item 1</li>
    ///                 <li>Item 2</li>
    ///                 <li>Item 3</li>
    ///             </ul>
    ///         ]]>
    ///       </common:Properties.Html>
    ///    </RichTextBlock>
    /// </summary>
    public class RichTextBlockProperties : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached("Html", typeof(string), typeof(RichTextBlockProperties), new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        private static async void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBlock richText) return;
            string xhtml = string.Format("<div>{0}</div>", e.NewValue as string);
            xhtml = xhtml.Replace("\r", "").Replace("\n", "<br />").Replace("&", "&amp;");
            RichTextBlock newRichText;
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                string xaml = "";
                try
                {
                    xaml = await ConvertHtmlToXamlRichTextBlock(xhtml);
                    newRichText = (RichTextBlock)XamlReader.Load(xaml);
                }
                catch (Exception ex)
                {
                    string errorxaml = string.Format(@"
                        <RichTextBlock 
                         xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                        >
                            <Paragraph>An exception occurred while converting HTML to XAML: {0}</Paragraph>
                            <Paragraph />
                            <Paragraph>HTML:</Paragraph>
                            <Paragraph>{1}</Paragraph>
                            <Paragraph />
                            <Paragraph>XAML:</Paragraph>
                            <Paragraph>{2}</Paragraph>
                        </RichTextBlock>",
                        ex.Message,
                        EncodeXml(xhtml),
                        EncodeXml(xaml)
                    );
                    newRichText = (RichTextBlock)XamlReader.Load(errorxaml);
                }
            }
            else
            {
                try
                {
                    string xaml = await ConvertHtmlToXamlRichTextBlock(xhtml);
                    newRichText = (RichTextBlock)XamlReader.Load(xaml);
                }
                catch (Exception)
                {
                    string errorxaml = string.Format(@"
                        <RichTextBlock 
                         xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                        >
                            <Paragraph>Cannot convert HTML to XAML. Please ensure that the HTML content is valid.</Paragraph>
                            <Paragraph />
                            <Paragraph>HTML:</Paragraph>
                            <Paragraph>{0}</Paragraph>
                        </RichTextBlock>",
                        EncodeXml(xhtml)
                    );
                    newRichText = (RichTextBlock)XamlReader.Load(errorxaml);
                }
            }
            richText.Blocks.Clear();
            if (newRichText != null)
            {
                for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                {
                    Block b = newRichText.Blocks[i];
                    newRichText.Blocks.RemoveAt(i);
                    richText.Blocks.Insert(0, b);
                }
            }
        }

        private static string EncodeXml(string xml)
        {
            string encodedXml = xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
            return encodedXml;
        }

        private static XsltProcessor Html2XamlProcessor;

        private static async Task<string> ConvertHtmlToXamlRichTextBlock(string xhtml)
        {
            XmlDocument xhtmlDoc = new();
            xhtmlDoc.LoadXml(xhtml);
            if (Html2XamlProcessor == null)
            {
                Assembly assembly = typeof(RichTextBlockProperties).GetTypeInfo().Assembly;
                using Stream stream = assembly.GetManifestResourceStream("Megatokyo.Client.Helpers.RichTextBlockHtml2Xaml.xslt");
                StreamReader reader = new(stream);
                string content = await reader.ReadToEndAsync();
                XmlDocument html2XamlXslDoc = new();
                html2XamlXslDoc.LoadXml(content);
                Html2XamlProcessor = new XsltProcessor(html2XamlXslDoc);
            }
            string xaml = Html2XamlProcessor.TransformToString(xhtmlDoc.FirstChild);
            return xaml;
        }

    }
}
