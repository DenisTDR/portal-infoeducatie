using System.Drawing;

namespace InfoEducatie.Main.InfoEducatieAdmin.Diplomas
{
    public class ImageTextModel
    {
        public ImageTextModel()
        {
        }

        public ImageTextModel(string text, float x, float y)
        {
            Text = text;
            X = x;
            Y = y;
        }

        public string Text { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Font Font { get; set; }
    }
}