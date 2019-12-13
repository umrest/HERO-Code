using System;
using Microsoft.SPOT;

namespace HERO_Code_2019.Controls {
    class ControlAlgorithms {
        public static float P_Loop(float pos, float unitPos, float desiredPos = 0) {
            float P = 1.0f / (unitPos - desiredPos);

            pos = TypeConverter.Constrain(pos, -unitPos + desiredPos, unitPos + desiredPos);

            //Debug.Print(P.ToString() + ", " + pos.ToString() + ", " + ((pos - desiredPos) * P).ToString());

            return (pos - desiredPos) * P;
        }


    }
}
