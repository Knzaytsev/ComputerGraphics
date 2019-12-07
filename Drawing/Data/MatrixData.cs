using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Data
{
    class MatrixData
    {
        private double phi;
        private double theta;
        private double zc;

        public double SinPhi
        {
            get
            {
                return MakeSin(phi);
            }
            set { phi = value; }
        }

        public double SinTheta
        {
            get
            {
                return MakeSin(theta);
            }
            set { theta = value; }
        }

        public double CosPhi
        {
            get
            {
                return MakeCos(phi);
            }
            set { phi = value; }
        }

        public double CosTheta
        {
            get
            {
                return MakeCos(theta);
            }
            set { theta = value; }
        }

        public double Zc
        {
            get { return zc; }
            set { zc = value; }
        }

        public MatrixData(double phi, double theta, double zc)
        {
            this.phi = phi;
            this.theta = theta;
            this.zc = zc;
        }

        private double MakeSin(double angle)
        {
            var rad = angle * Math.PI / 180;
            return Math.Sin(rad);
        }

        private double MakeCos(double angle)
        {
            var rad = angle * Math.PI / 180;
            return Math.Cos(rad);
        }
    }
}
