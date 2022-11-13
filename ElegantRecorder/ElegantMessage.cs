using System;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class ElegantMessage : Form
    {
        public ElegantMessage()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string message, string caption = "Elegant Message")
        {
            var elegantMessage = new ElegantMessage();
            elegantMessage.Text = caption;
            elegantMessage.labelMessage.Text = message;
            elegantMessage.TopMost = true;
            elegantMessage.labelMessage.Left = (elegantMessage.Width - elegantMessage.labelMessage.Width) / 2;
            return elegantMessage.ShowDialog();
        }

        public static DialogResult Info(string message, string caption = "Elegant Message")
        {
            var elegantMessage = new ElegantMessage();
            elegantMessage.Text = caption;
            elegantMessage.labelMessage.Text = message;
            elegantMessage.TopMost = true;
            elegantMessage.labelMessage.Left = (elegantMessage.Width - elegantMessage.labelMessage.Width) / 2;

            elegantMessage.buttonOk.Left = (elegantMessage.Width - elegantMessage.buttonOk.Width) / 2;
            elegantMessage.Controls.Remove(elegantMessage.buttonCancel);
            return elegantMessage.ShowDialog();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
