using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AgrServer
{
    public partial class MessagesForm : Form
    {
        private readonly List<string> _messages;
        public MessagesForm(List<string> messages)
        {
            InitializeComponent();
            _messages = messages;
        }

        private void MessagesFormLoad(object sender, EventArgs e)
        {
            foreach (string message in _messages)
            {
                list.Items.Add(message);
            }
        }
    }
}
