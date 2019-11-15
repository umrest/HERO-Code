using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class DecodeDashboardState {

        private const byte ENABLE_KEY_START_VALUE = 20;

        private bool enabled;

        public void DecodeDashboardStateData(byte[] data) {

            enabled = true;

            for (int i = 0; i < 8; i++) {
                if (data[i + 1] != (ENABLE_KEY_START_VALUE + i)) {
                    enabled = false;
                    break;
                }
            }

        }

        //Returns robot enabled status
        public bool IsEnabled {
            get { return enabled; }
        }
    }
}
