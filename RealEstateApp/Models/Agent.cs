namespace RealEstateApp.Models
{
    public class Agent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FacebookProfile { get; set; }
        public string ImageUrl { get; set; }

        private ImageSource _imageSource;
        public ImageSource ImageSource => _imageSource ??= ImageUtil.GetSourceOrDefault(ImageUrl);
    }
}
