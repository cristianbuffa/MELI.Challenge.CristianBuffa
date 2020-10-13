using System;
using System.Collections.Generic;
using System.Text;

namespace GeometricsShapes.Calculations
{
    public class Circumference: Base
    {
        public float Radio { get; set; }

        public Circumference(float radio, string nombre, Coordinates coordenadas)
        {
            this.Radio = radio;
            this.Name = nombre;
            this.Coordinates = coordenadas;
        }
    }
}
