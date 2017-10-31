using System;
using System.Windows.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
    public partial class TransferDBToNewYearForm : Form
    {
        public TransferDBToNewYearForm()
        {
            InitializeComponent();
        }

        public IScheme Scheme { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Scheme == null)
            {
                MessageBox.Show("Scheme");
                return;
            }

            if (String.IsNullOrEmpty(cbYear.Text))
            {
                MessageBox.Show("Параметр год не указан.", "Ошибка в параметрах", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TransferDBToNewYearState state;
            Operation operation = new Operation();
            try
            {
                operation.Text = "Выполнение функции перевода базы на новый год";
                operation.StartOperation();
                state = Scheme.TransferDBToNewYear(Convert.ToInt32(cbYear.Text));
            }
            finally
            {
                operation.StopOperation();
            }

            switch (state)
            {
                case TransferDBToNewYearState.Error:
                    MessageBox.Show("Функция перевода базы на новый год завершена с ошибками. См. протокол «Перевод базы на новый год».",
                                    "Завершено с ошибками", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case TransferDBToNewYearState.Successfully:
                    MessageBox.Show("Функция перевода базы на новый год завершена успешно. См. протокол «Перевод базы на новый год».", "Выполнено успешно",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case TransferDBToNewYearState.SuccessfullyWithWarning:
                    MessageBox.Show("Функция перевода базы на новый год завершена успешно с предупреждениями. См. протокол «Перевод базы на новый год»", "Выполнено успешно с предупреждениями",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

    }
}
