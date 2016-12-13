using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;


namespace DropshipBusiness.Item
{
    public interface IImageService
    {
        D_Image InsertImage(D_Image image);

        D_Image UpdateImage(D_Image image);

        void DeleteImage(D_Image image);
    }
}
