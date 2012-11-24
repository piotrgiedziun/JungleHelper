using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace WindowsFormsApplication3
{
    public partial class AppForm : Form
    {
        public static SpeechSynthesizer speaker;
        public Dictionary<int, object> hookBuff;
        GlobalKeyboardHook gHook;

        public AppForm()
        {
            InitializeComponent();
            speaker = new SpeechSynthesizer();
            speaker.Rate = 1;
            speaker.Volume = 100;

        }

        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (hookBuff.ContainsKey((int)e.KeyCode))
            {
                speaker.SpeakAsyncCancelAll();

                object t = hookBuff[(int)e.KeyCode];
                if (t is AppForm)
                {
                    ((AppForm)t).say();
                }
                else if (t is Buff)
                {
                    ((Buff)t).say();
                }
            }
        }

        public void say()
        {
            List<Buff> inProgress = new List<Buff>();
            foreach (var v in hookBuff)
            {
                if (v.Value is Buff)
                {
                    Buff buff = (Buff)v.Value;
                    if (buff.time != 0)
                    {
                        inProgress.Add(buff);
                    }
                }
            }
            if (inProgress.Count == 0)
            {
                speaker.SpeakAsync("stack is empty");
            }
            else
            {
                List<Buff> inProgressSorted = inProgress.OrderBy(c => c.timeLeft).ToList();

                foreach (var v in inProgressSorted)
                {
                    ((Buff)v).say(true);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Output information about all of the installed voices. 
            foreach (InstalledVoice voice in speaker.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex = 0;

            //init  buff array
            hookBuff = new Dictionary<int, object>();

            //load file
            if (!System.IO.File.Exists("settings.txt"))
            {
                MessageBox.Show("Setting file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] lines = System.IO.File.ReadAllLines("settings.txt");

            foreach (string line in lines)
            {
                string[] data = line.Split('|');
                if ((line.ToCharArray())[0].Equals('#')) continue;
                hookBuff.Add(Convert.ToInt32(data[0]), new Buff(data[1], Convert.ToInt32(data[2])));
            }

            hookBuff.Add(192, this);

            gHook = new GlobalKeyboardHook();

            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                if (hookBuff.ContainsKey((int)key))
                    gHook.HookedKeys.Add(key);

            gHook.hook();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gHook != null)
            {
                speaker.SpeakAsyncCancelAll();
                gHook.unhook();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String message = "Binds:\n";

            foreach (var v in hookBuff)
            {
                if (v.Value is Buff)
                {
                    Buff buff = (Buff)v.Value;
                    message += ((System.Windows.Forms.Keys)v.Key) + " -> " + buff.name + "\n";
                }
            }
            MessageBox.Show(message, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox1.Text.Equals(""))
                speaker.SelectVoice(comboBox1.Text);
        }
    }
}
