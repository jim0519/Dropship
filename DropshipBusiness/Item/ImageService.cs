using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;
using DropshipData;
using DropshipCommon;
using System.IO;
using LINQtoCSV;

namespace DropshipBusiness.Item
{
    public class ImageService:IImageService
    {
        private IRepository<D_Image> _imageRepository;

        public ImageService(IRepository<D_Image> imageRepository)
        {
            _imageRepository = imageRepository;
        }



        public D_Image InsertImage(D_Image image)
        {
            if (image != null)
                _imageRepository.Insert(image);

            return image;
        }

        public D_Image UpdateImage(D_Image image)
        {
            if (image != null)
                _imageRepository.Update(image);

            return image;
        }

        public void DeleteImage(D_Image image)
        {
            if (image != null)
                _imageRepository.Delete(image);
        }
    }
}
