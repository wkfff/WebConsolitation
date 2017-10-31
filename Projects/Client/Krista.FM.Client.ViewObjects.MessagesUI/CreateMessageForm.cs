using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.MessagesUI.AddressBook;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    public enum MessageActualUnit
    {
        [Description("без ограничения")]
        Unlimited,
        [Description("минут")]
        Minutes,
        [Description("часов")]
        Hours,
        [Description("дней")]
        Days
    }

    public partial class CreateMessageForm : Form
    {
        private readonly IScheme scheme;
        private List<AddressBookElement> listTo = new List<AddressBookElement>();
        private byte[] attachment;
        private string fileName;
        private string filePath;

        public CreateMessageForm(IScheme scheme)
        {
            this.scheme = scheme;

            InitializeComponent();

            textBoxTo.TextChanged += TextBoxTo_TextChanged;

            textBoxTo.Focus();
            textBoxTo.SelectionStart = 0;
            textBoxTo.ScrollToCaret();
        }

        public CreateMessageForm(
            IScheme scheme,
            bool sendAll,
            string textMessage,
            MessageActualUnit messageActualUnit,
            string timeActual,
            MessageImportance importance)
            : this(scheme)
        {
            checkBoxAll.Checked = sendAll;
            textBoxTo.Enabled = !sendAll;
            textBox.ForeColor = Color.Black;
            textBox.Text = textMessage;
            maskedTextBox.Enabled = (messageActualUnit != MessageActualUnit.Unlimited);
            maskedTextBox.Text = timeActual;
            comboBoxActual.SelectedItem = GetEnumDescription(messageActualUnit);
            comboBoxImpotance.SelectedItem = GetEnumDescription(importance).ToLower();
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }


        private void SetCaret()
        {
            textBoxTo.SelectionStart = textBoxTo.Text.Length;
            textBoxTo.ScrollToCaret();
        }

        private void textBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox.Text.Equals("Введите текст сообщения"))
            {
                textBox.Text = String.Empty;
                textBox.ForeColor = Color.Black;
            }
        }

        private void CreateMessageForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void TextBoxTo_TextChanged(object sender, EventArgs e)
        {
            int h = textBoxTo.Height;
           //Amount of padding to add
            const int padding = 10;
            //get number of lines (first line is 0)
            int numLines = textBoxTo.GetLineFromCharIndex(textBoxTo.TextLength) + 1;
            //get border thickness
            int border = textBoxTo.Height - textBoxTo.ClientSize.Height;
            //set height (height of one line * number of lines + spacing)
            textBoxTo.Height = textBoxTo.Font.Height * numLines + padding + border;
            MaximumSize = MinimumSize = new Size(Width, Height + (textBoxTo.Height - h));
            Height = Height + (textBoxTo.Height - h);

            groupBoxTo.Height += textBoxTo.Height - h;
        }

        private void TextBoxTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.K && e.Modifiers == Keys.Control)
            {
                OpenAddressBook();
            }
        }

        private void OpenAddressBook()
        {
            AddressBookForm addressBookForm = new AddressBookForm(new SchemeAddressBook(scheme), listTo);
            addressBookForm.Location = new Point(textBoxTo.ClientRectangle.Width + textBoxTo.Location.X,
                                                 textBoxTo.ClientRectangle.Height + textBoxTo.Location.Y);
            if (addressBookForm.ShowDialog(this) == DialogResult.OK)
            {
                listTo.Clear();
                textBoxTo.Text = String.Empty;

                foreach (var item in addressBookForm.SelectedItem)
                {
                    textBoxTo.Text += string.Format("{0};", item.FullName);
                    if (!listTo.Contains(item))
                    {
                        listTo.Add(item);
                    }

                    SetCaret(); 
                }
            }
        }

        private AddressBookElement FindInList(string name)
        {
            return listTo.FirstOrDefault(addressBookElement => addressBookElement.FullName.ToLower().Equals(name.ToLower()));
        }

        private void BtnAttachment_Click(object sender, EventArgs e)
        {
            Stream myStream;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = String.IsNullOrEmpty(filePath) 
                ? "c:\\" 
                : filePath;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2 ;
            openFileDialog.RestoreDirectory = true ;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            attachment = ReadToEnd(myStream);
                            fileName = openFileDialog.SafeFileName;
                            filePath = Path.GetDirectoryName(openFileDialog.FileName);
                            pictureBox1.Image = Properties.Resources.clip2;
                            SetLabelAttachmentText(fileName);
                            buttonDeleteAttachment.Location = new Point(labelFile.Location.X + labelFile.Width + 10, labelFile.Location.Y);
                            buttonDeleteAttachment.Visible = true;
                            labelFile.Visible = true;
                            pictureBox1.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void SetLabelAttachmentText(string s)
        {
            labelFile.MaximumSize = new Size(205, 300);
            labelFile.AutoSize = true;
            int h = labelFile.Height;
            labelFile.Text = s;

            groupBoxAttachment.Height = groupBoxAttachment.Height + (labelFile.Height - h);
            MaximumSize = MinimumSize = new Size(Width, Height + (labelFile.Height - h));
            Height = Height + (labelFile.Height - h);
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateEnterData();
            }
            catch (ValidateEnterException validateEnterException)
            {
                MessageBox.Show(String.Format("Ошибка ввода данных. {0}", validateEnterException.Message),
                                "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            MessageAttachmentWrapper messageAttachmentWrapper = GetMessageAttachment();
            DateTime actualTime = GetMessageActual();
            MessageImportance importance = GetMessageImportance();

            if (checkBoxAll.Checked)
            {
                MessageWrapper messageWrapper = new MessageWrapper();
                messageWrapper.Subject = textBox.Text;
                messageWrapper.SendAll = true;
                messageWrapper.RefUserSender = scheme.UsersManager.GetCurrentUserID();
                messageWrapper.RefMessageAttachment = messageAttachmentWrapper;
                messageWrapper.DateTimeOfActual = actualTime;
                messageWrapper.MessageImportance = importance;
                try
                {
                    scheme.MessageManager.SendMessage(messageWrapper);
                    DialogResult = DialogResult.OK;
                    MessageBox.Show("Сообщение успешно отправлено всем пользователям.", "Отправка сообщения",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    throw new Exception(String.Format("При отправке сообщения возникло исключение: {0}", exception.Message));
                }
            }
            else
            {
                string[] recipients = textBoxTo.Text.Split(';');
                foreach (var recipient in recipients)
                {
                    if (String.IsNullOrEmpty(recipient.Trim()))
                    {
                        break;
                    }

                    AddressBookElement addressBookElement = FindInList(recipient.Trim());
                    if (addressBookElement != null)
                    {
                        MessageWrapper messageWrapper = new MessageWrapper();
                        messageWrapper.Subject = textBox.Text;
                        messageWrapper.DateTimeOfActual = actualTime;
                        messageWrapper.MessageImportance = importance;
                        messageWrapper.RefGroupRecipient = (addressBookElement is GroupWrapper)
                                                               ? addressBookElement.Id
                                                               : (int?) null;
                        messageWrapper.RefUserRecipient = (addressBookElement is UserWrapper)
                                                              ? addressBookElement.Id
                                                              : (int?) null;
                        messageWrapper.RefUserSender = scheme.UsersManager.GetCurrentUserID();
                        messageWrapper.RefMessageAttachment = messageAttachmentWrapper;
                        try
                        {
                            bool isSend = scheme.MessageManager.SendMessage(messageWrapper);
                            if (!isSend)
                            {
                                throw new Exception("Сообщение не было отправлено. См. лог сервера.");
                            }
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(String.Format("При отправке сообщения возникло исключение: {0}", exception.Message));
                        }
                    }
                }

                DialogResult = DialogResult.OK;

                MessageBox.Show("Сообщение успешно отправлено выбранным пользователям.", "Отправка сообщения",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Проверка ввода обязательных для заполнения полей
        /// </summary>
        private void ValidateEnterData()
        {
            if (String.IsNullOrEmpty(textBoxTo.Text) && !checkBoxAll.Checked)
            {
                throw new ValidateEnterException("Не указан ни один получатель и не выставлена опция \"Отправить всем\"");
            }

            if (string.IsNullOrEmpty(textBox.Text) || textBox.Text.Equals("Введите текст сообщения"))
            {
                throw new ValidateEnterException("Не указана тема сообщения.");
            }

            if (comboBoxActual.Text.ToLower() != "без ограничения" && maskedTextBox.Text =="")
            {
                throw new ValidateEnterException("Не указано время актуальности сообщения.");
            }
        }

        private MessageAttachmentWrapper GetMessageAttachment()
        {
            if (attachment == null)
            {
                return null;
            }

            MessageAttachmentWrapper messageAttachmentWrapper = new MessageAttachmentWrapper();
            messageAttachmentWrapper.Document = attachment;
            messageAttachmentWrapper.DocumentFileName = fileName;

            return messageAttachmentWrapper;
        }

        private DateTime GetMessageActual()
        {
            switch (comboBoxActual.Text.ToLower())
            {
                case "без ограничения":
                    {
                        return DateTime.MaxValue;
                    }
                case "минут":
                    {
                        return DateTime.Now.AddMinutes(Convert.ToInt32(maskedTextBox.Text));
                    }
                case "часов":
                    {
                        return DateTime.Now.AddHours(Convert.ToInt32(maskedTextBox.Text));
                    }
                case "дней":
                    {
                        return DateTime.Now.AddDays(Convert.ToInt32(maskedTextBox.Text));
                    }
            }

            throw new Exception("Не удалось определить время актуальности сообщения");
        }

        private MessageImportance GetMessageImportance()
        {
            switch (comboBoxImpotance.Text.ToLower())
            {
                case "высокая важность":
                    {
                        return MessageImportance.HighImportance;
                    }
                case "важное":
                    {
                        return MessageImportance.Importance;
                    }
                case "неважное":
                    {
                        return MessageImportance.Unimportant;
                    }
            }

            return MessageImportance.Regular;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenAddressBook();
        }

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTo.Enabled = !checkBoxAll.Checked;
            buttonAddressBook.Enabled = !checkBoxAll.Checked;
        }

        private void buttonDeleteAttachment_Click(object sender, EventArgs e)
        {
            SetLabelAttachmentText("");
            attachment = null;
            buttonDeleteAttachment.Visible = false;
            labelFile.Visible = false;
            pictureBox1.Visible = false;
        }

        private void comboBoxActual_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox d = sender as ComboBox;

            if (d != null)
            {
                switch (d.SelectedItem.ToString())
                {
                    case "без ограничения":
                        {
                            maskedTextBox.Enabled = false;
                            break;
                        }
                    default:
                        {
                            maskedTextBox.Enabled = true;
                            break;
                        }
                }
            }
        }
    }
}
