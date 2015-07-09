using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace keyboardmasher
{
    public partial class Form1 : Form
    {
        int keytoclick;
        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Maximum = int.MaxValue; //see line 69 and 70 to understand why
            numericUpDown1.Value = timer1.Interval; //so it can be modified from form view
            numericUpDown2.Value = timer2.Interval;
            keytoclick = Convert.ToInt32(ConvertCharToVirtualKey("w".ToCharArray()[0])); //a bit too long, eh?
        }

        #region keyclick

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public static Keys ConvertCharToVirtualKey(char ch) //http://stackoverflow.com/questions/2898806/how-to-convert-a-character-to-key-code#
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }

        //these are virtual key codes for keys
        const uint KEYEVENTF_KEYUP = 0x0002; //http://tksinghal.blogspot.com/2011/04/how-to-press-and-hold-keyboard-key.html
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        int press(int keycode) //left as int because 1) it might be important. 2) because why not?
        {
            //Press the key
            keybd_event((byte)keycode, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            return 0;

        }
        int release(int keycode) 
        {
            //Release the key
            keybd_event((byte)keycode, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            return 0;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(numericUpDown1.Value);
            timer2.Interval = Convert.ToInt32(numericUpDown2.Value);
            keytoclick = Convert.ToInt32(ConvertCharToVirtualKey(textBox1.Text.ToCharArray()[0]));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //for ex: click every 1 sec.
            press(keytoclick);
            timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //for ex: stop holding after 0.1 sec.
            release(keytoclick);
            timer2.Enabled = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //timer2's interval must be shorter than timer1. else = bug...
            numericUpDown2.Maximum = numericUpDown1.Value - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true; //2 enables automatically
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = true; //run once more, so it'll disable
            release(keytoclick); //just in case
        }
    }
}
