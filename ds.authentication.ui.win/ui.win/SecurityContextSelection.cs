using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ds.authentication.ui.win
{
    public partial class SecurityContextSelection : Form
    {
        public SecurityContextSelection()
        {
            InitializeComponent();
        }

        public void Initialize(List<string> _securityContext)
        {
            m_securityContextComboBox.Items.AddRange(_securityContext.ToArray());
        }

        public string SelectedContext { get { return (string)m_securityContextComboBox.SelectedItem; }
            set {
                m_securityContextComboBox.SelectedItem = value; }
        }

        private void m_okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
