using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Services
{
    /// <summary>
    /// Класс инкапсулирует функционал вывода сообщений пользователю
    /// </summary>
    public static class MessageService
    {
        static Form mainForm;

        /// <summary>
        /// Родительская форма приложения
        /// </summary>
        public static Form MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }

        public static void ShowWarning(string message)
        {
            message = StringParser.Parse(message);
            LoggingService.Warn(message);
            MessageBox.Show(MessageService.MainForm,
                            message,
                            StringParser.Parse("${res:Global.WarningText}"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1,
                            0);
        }

        /// <summary>
        /// Отображает исключение.
        /// </summary>
        /// <param name="ex">Исключение</param>
        public static void ShowError(Exception ex)
        {
            ShowError(ex, null);
        }

        /// <summary>
        /// Отображает сообщение об ошибке.
        /// </summary>
        /// <param name="message"></param>
        public static void ShowError(string message)
        {
            ShowError(null, message);
        }

        public static void ShowErrorFormatted(string formatstring, params string[] formatitems)
        {
            ShowError(null, Format(formatstring, formatitems));
        }

        public static void ShowError(Exception ex, string message)
        {
            if (message == null) 
                message = string.Empty;

            if (ex != null)
            {
                LoggingService.Error(message, ex);
            }
            else
            {
                LoggingService.Error(message);
            }

            string msg = message + "\n\n";

            if (ex != null)
            {
                msg += "Exception occurred: " + ex.ToString();
            }

            MessageBox.Show(MessageService.MainForm, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Вызывает диалоговое окно с вопросом
        /// </summary>
        /// <param name="question">Вопрос</param>
        /// <param name="caption">Заголовок</param>
        /// <returns>Результат</returns>
        public static bool AskQuestion(string question, string caption)
        {
            return MessageBox.Show(MessageService.MainForm,
                                   question,
                                   caption,
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button1,
                                   0)
                == DialogResult.Yes;
        }

        public static bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
        {
            return AskQuestion(Format(formatstring, formatitems), caption);
        }

        public static bool AskQuestionFormatted(string formatstring, params string[] formatitems)
        {
            return AskQuestion(Format(formatstring, formatitems));
        }

        public static bool AskQuestion(string question)
        {
            return AskQuestion(question, "Question");
        }

        public static int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
        {
            using (CustomDialog messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts))
            {
                messageBox.ShowDialog(MessageService.MainForm);
                return messageBox.Result;
            }
        }

        public static int ShowCustomDialog(string caption, string dialogText, params string[] buttontexts)
        {
            return ShowCustomDialog(caption, dialogText, -1, -1, buttontexts);
        }

        public static string ShowInputBox(string caption, string dialogText, string defaultValue)
        {
            using (InputBox inputBox = new InputBox(dialogText, caption, defaultValue))
            {
                inputBox.ShowDialog(MessageService.MainForm);
                return inputBox.Result;
            }
        }

        static string defaultMessageBoxTitle = "MessageBox";
        static string productName = "Application Name";

        public static string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        public static string DefaultMessageBoxTitle
        {
            get { return defaultMessageBoxTitle; }
            set {defaultMessageBoxTitle = value; }
        }

        public static void ShowMessage(string message)
        {
            ShowMessage(message, DefaultMessageBoxTitle);
        }

        public static void ShowMessageFormatted(string formatstring, params string[] formatitems)
        {
            ShowMessage(Format(formatstring, formatitems));
        }

        public static void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
        {
            ShowMessage(Format(formatstring, formatitems), caption);
        }

        public static void ShowMessage(string message, string caption)
        {
            LoggingService.Info(message);
            MessageBox.Show(mainForm,
                            message,
                            caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            0);
        }

        static string Format(string formatstring, string[] formatitems)
        {
            try
            {
                return String.Format(formatstring, formatitems);
            }
            catch (FormatException)
            {
                StringBuilder b = new StringBuilder(formatstring);
                foreach (string formatitem in formatitems)
                {
                    b.Append("\nItem: ");
                    b.Append(formatitem);
                }
                return b.ToString();
            }
        }
    }
}
