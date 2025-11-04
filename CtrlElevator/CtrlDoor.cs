using System.Windows.Forms;

namespace CtrlElevator
{
    class CtrlDoor
    {
        public void Timer_door_close_down(PictureBox doorLeftdown, PictureBox doorRightdown)
        {
            if (doorLeftdown.Left <= 165 && doorRightdown.Left >= 150)
            {
                doorLeftdown.Left += 1;
                doorRightdown.Left -= 1;
            }
        }

        public void Timer_door_close_up(PictureBox doorLeftup, PictureBox doorRightup)
        {
            if (doorLeftup.Left <= 165 && doorRightup.Left >= 150)
            {
                doorLeftup.Left += 1;
                doorRightup.Left -= 1;
            }
        }

        public void Timer_door_open_down(PictureBox doorLeftdown, PictureBox doorRightdown)
        {
            if (doorLeftdown.Left >= 108 && doorRightdown.Left <= 310)
            {
                doorLeftdown.Left -= 1;
                doorRightdown.Left += 1;
            }
        }

        public void Timer_door_open_up(PictureBox doorLeftup, PictureBox doorRightup)
        {
            if (doorLeftup.Left >= 108 && doorRightup.Left <= 310)
            {
                doorLeftup.Left -= 1;
                doorRightup.Left += 1;
            }
        }
    }
}
