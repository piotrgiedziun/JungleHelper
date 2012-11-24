using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication3
{
    public class Buff
    {
        public String name;
        public int duration;
        public int time;
        public int timeLeft;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Buff(String name, int duration)
        {
            this.name = name;
            this.duration = duration;
            this.timer.Interval = 1000;
            this.timer.Tick += new EventHandler(TimerEventProcessor);
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            this.time++;

            if (this.duration == this.time)
            {
                AppForm.speaker.SpeakAsync(this.name + " spowned");
                this.timer.Stop();
                this.timer.Enabled = false;
                this.time = 0;
            }
            else if (this.duration - this.time == 1 * 60)
            {
                this.say();
            }
        }

        private String getTime()
        {
            this.timeLeft = this.duration - this.time; ;
            String respone;

            if (this.timeLeft >= 60)
            {
                respone = (this.timeLeft / 60) + " minute" + (((this.timeLeft / 60) != 1) ? "s" : "");

                if ((this.timeLeft - ((this.timeLeft / 60) * 60)) != 0)
                {
                    respone += " " + (this.timeLeft - ((this.timeLeft / 60) * 60)) + " second" + (((this.timeLeft - ((this.timeLeft / 60) * 60)) != 1) ? "s" : "");
                }
            }
            else
            {
                respone = this.timeLeft + " second" + ((this.timeLeft != 1) ? "s" : "");
            }
            return respone;
        }

        public void start()
        {
            this.time = 0;
            this.timer.Start();
            this.timer.Enabled = true;
        }

        public void say()
        {
            say(false);
        }

        public void say(bool isShort)
        {
            if (this.time == 0)
            {
                // start timer
                this.start();
                AppForm.speaker.SpeakAsync(this.name + " timer started");
            }
            else
            {
                if (isShort)
                {
                    AppForm.speaker.SpeakAsync(this.name + " in " + this.getTime());
                }
                else
                {
                    AppForm.speaker.SpeakAsync(this.name + " spawn in " + this.getTime());
                }
            }
        }
    }
}
