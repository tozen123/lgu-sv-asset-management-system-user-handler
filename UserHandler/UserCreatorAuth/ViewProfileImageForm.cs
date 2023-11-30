using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserCreatorAuth
{
    public partial class ViewProfileImageForm : Form
    {
        public ViewProfileImageForm(Image _image)
        {
            InitializeComponent();
            pictureBox1.Image = _image;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
