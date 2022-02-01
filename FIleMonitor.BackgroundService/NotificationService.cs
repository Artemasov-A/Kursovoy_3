using System;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace FIleMonitor.BackgroundService
{
    public class NotificationService
    {
        public void SendMessageAboutDeleteFolder(string path)
        {
            XmlDocument template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);

            var textNodes = template.GetElementsByTagName("text").ToList();

            textNodes[0].AppendChild(template.CreateTextNode("Увага! Було видалено папку з файлової системи."));
            textNodes[1].AppendChild(template.CreateTextNode($"Папку було видалено за наступним шляхом"));
            textNodes[2].AppendChild(template.CreateTextNode(path));

            SendMessage(template);
        }

        public void SendMessageAboutChangeFile(string filepath)
        {
            XmlDocument template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);

            var textNodes = template.GetElementsByTagName("text").ToList();

            textNodes[0].AppendChild(template.CreateTextNode("Увага! Файл був зміненний."));
            textNodes[1].AppendChild(template.CreateTextNode($"Файл за наступним шляхом було зміненно"));
            textNodes[2].AppendChild(template.CreateTextNode(filepath));

            SendMessage(template);
        }

        public void SendMessageAboutDeleteFile(string path)
        {
            XmlDocument template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);

            var textNodes = template.GetElementsByTagName("text").ToList();

            textNodes[0].AppendChild(template.CreateTextNode("Увага! Зверніть увагу файл було видалено!"));
            textNodes[1].AppendChild(template.CreateTextNode($"Файл за наступним шляхом було видалено"));
            textNodes[2].AppendChild(template.CreateTextNode(path));

            SendMessage(template);
        }

        private void SendMessage(XmlDocument template)
        {
            var toast = new ToastNotification(template);
            toast.Tag = "FileMonitor.BackgroundService";
            toast.Group = "C#";
            toast.ExpirationTime = DateTimeOffset.Now.AddMinutes(5);

            var notifier = ToastNotificationManager.CreateToastNotifier("FileMonitor.BackgroundService");
            notifier.Show(toast);
        }
    }
}
