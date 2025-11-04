using System.Windows.Forms;

namespace CtrlElevator
{
    class Elevator
    {
        public void Timer_up(PictureBox pictureElevator)
        {
            if (pictureElevator.Top >= 82)
            {
                pictureElevator.Top -= 1;
            }
        }

        public void Timer_down(PictureBox pictureElevator)
        {
            if (pictureElevator.Top <= 431)
            {
                pictureElevator.Top += 1;
            }
        }
    }
}
