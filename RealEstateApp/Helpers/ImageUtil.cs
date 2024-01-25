global using RealEstateApp.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Helpers
{
    public static class ImageUtil
    {
        public static ImageSource GetSourceOrDefault( string uri )
        {
            ImageSource source;
            try
            {
                source = ImageSource.FromUri(new Uri(uri));
                return source;
            }
            catch( Exception ex)
            {
                source = ImageSource.FromFile("no_image.jpg");
                return source;
            }
        }
    }
}
