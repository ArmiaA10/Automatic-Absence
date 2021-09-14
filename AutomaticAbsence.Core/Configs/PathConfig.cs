namespace AutomaticAbsence.Core.Configs
{
    public static class PathConfig
    {
        public static string CascadeClassifierPath { get; set; } = "Resources\\haarcascade_frontalface_default.xml";
        public static string PersonPath { get; set; } = "Source\\Faces\\";
        public static string PersonListPath { get; set; } = "Source\\FaceList.txt";
        public static string ImageFileExtension { get; set; } = ".bmp";
    }
}
