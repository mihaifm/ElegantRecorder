using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class EncryptionPassword : Form
    {
        public string Password { get; set; }

        public EncryptionPassword(bool set)
        {
            InitializeComponent();

            if (set)
                labelPrompt.Text = "Set encryption password:";
            else
                labelPrompt.Text = "Enter encryption password:";
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Password = textBoxPassword.Text;
                Close();
            }
        }

        private void buttonOk_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Password = textBoxPassword.Text;
            Close();
        }
    }
}
