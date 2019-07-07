using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models.Serializer.Image
{
    public class MatReader : IResourceReader<Mat>
    {
        #region fields

        #endregion fields


        #region constructor

        public MatReader()
        {
        }

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public Mat Read(string path)
        {
            return CvInvoke.Imread(path, Emgu.CV.CvEnum.ImreadModes.Unchanged);
        }

        #endregion methods
    }
}
